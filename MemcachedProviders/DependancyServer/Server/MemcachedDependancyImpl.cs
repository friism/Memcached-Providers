using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Amib.Threading;

namespace DependancyServer.Server
{
    internal class MemcachedDependancyImpl : IDependancyServer
    {
        #region Class members
        private TcpListener _objListener;
        private LinkedList<IMemcachedDependancy> _objDependancy;
        private object objLock = new object();
        private object objMemcachedDependancyLock = new object();
        private FileSystemWatcher _objFileSystemWatcher;
        private SmartThreadPool _objThreadPool;
        private Queue<TcpClient> _objClientQueue;
        private IDictionary<string, IMemcachedDependancy> _objMemcachedDependancy;
        public IDictionary<string, IMemcachedDependancy> MemcachedDependancy
        {
            get { return _objMemcachedDependancy; }
        }
        #endregion

        #region File events
        void _objFileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File deleted: " + e.FullPath);
        }

        void _objFileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine("File name changed:" + e.FullPath);
        }

        void _objFileSystemWatcher_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("Error File: ");
        }

        void _objFileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Changed File: " + e.FullPath + " ------- " + e.ChangeType + " ---- " + e.Name);
            this._objThreadPool.QueueWorkItem(new WorkItemCallback(ProcessFileEvent), e.Name);
        }

        #endregion

        #region Constructor
        public MemcachedDependancyImpl()
        {
            this._objListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 
                int.Parse(System.Configuration.ConfigurationManager.AppSettings["port"]));
            this._objDependancy = new LinkedList<IMemcachedDependancy>();
            this._objThreadPool = new SmartThreadPool(100, 5, 1);
            this._objClientQueue = new Queue<TcpClient>();
            this._objMemcachedDependancy = new Dictionary<string, IMemcachedDependancy>();
        }
        #endregion

        #region TCP/IP request response functions
        private void SendErrorMessage(NetworkStream objStream)
        {
            ArraySegment<byte> objResponse = GetCommandBuffer("ERROR");
            Write(objResponse.Array, objResponse.Offset, objResponse.Count, objStream);

            Console.WriteLine("Sending Error response....");
        }

        private void SendSuccessMessage(NetworkStream objStream)
        {
            ArraySegment<byte> objResponse = GetCommandBuffer("STORED");
            Write(objResponse.Array, objResponse.Offset,objResponse.Count, objStream);
            Console.WriteLine("Sending Success response....");
        }
                
        public static ArraySegment<byte> GetCommandBuffer(string value)
        {
            int valueLength = value.Length;
            byte[] data = new byte[valueLength + 2];

            Encoding.ASCII.GetBytes(value, 0, valueLength, data, 0);

            data[valueLength] = 13;
            data[valueLength + 1] = 10;

            return new ArraySegment<byte>(data);
        }

        public static void Write(byte[] data, int offset, int length, NetworkStream objStream)
        {
            objStream.Write(data, offset, length);
        }

        private string ReadLine(NetworkStream inputStream)
        {
            MemoryStream ms = new MemoryStream(50);

            bool gotR = false;
            byte[] buffer = new byte[1];

            int data;

            try
            {
                while (true)
                {
                    data = inputStream.ReadByte();
                    if (data == 13)
                    {
                        gotR = true;
                        continue;
                    }

                    if (gotR)
                    {
                        if (data == 10)
                            break;

                        ms.WriteByte(13);

                        gotR = false;
                    }

                    ms.WriteByte((byte)data);
                }
            }
            catch (IOException)
            {
                throw;
            }

            string retval = Encoding.ASCII.GetString(ms.GetBuffer(), 0, (int)ms.Length);

            return retval;
        }
        #endregion

        #region WorkItemCallback methods

        /// <summary>
        /// This function processes any file changes in the specified directory
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected object ProcessFileEvent(object obj)
        {
            string strFileName = (string)obj;

            // Lock the queue for processing
            lock (objMemcachedDependancyLock)
            {
                if (this._objMemcachedDependancy.ContainsKey(strFileName) == true)
                {
                    IMemcachedDependancy objDependancy = this._objMemcachedDependancy[strFileName];

                    //remove the following from memcached
                    Console.WriteLine("Memcached Key {0} removed", objDependancy.DependancyKey);
                    this._objMemcachedDependancy.Remove(strFileName);
                    Console.WriteLine("Item removed from Process Queue");
                }
            }

            return null;
        }

        /// <summary>
        /// This functions constantly check for new requests
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected object CheckForRequest(object obj)
        {
            Console.WriteLine("Checking TCP Client...");
            Console.WriteLine();

            byte[] objData = new byte[1024];
            TcpClient objClient = null;

            while (true)
            {
                if (this._objClientQueue.Count > 0)
                {
                    lock (objLock)
                    {
                        if (this._objClientQueue.Count > 0)
                        {
                            objClient = this._objClientQueue.Dequeue();

                            if ((objClient.Connected == true))
                            {
                                if (objClient.Available > 0)
                                {
                                    this._objThreadPool.QueueWorkItem(new WorkItemCallback(ProcessDependancyRequest), objClient, WorkItemPriority.Highest);
                                }
                                else
                                {
                                    this._objClientQueue.Enqueue(objClient);
                                    Thread.Sleep(DateTime.Now.Millisecond); // get a random number
                                }
                            }
                            // if the socket is not connected it just removes it from the queue
                        }
                        else
                        {
                            Thread.Sleep(DateTime.Now.Millisecond); // get a random number
                            continue;
                        }
                    }
                }
                else
                {
                    //Console.WriteLine("Putting Thread to Sleep.......");
                    Thread.Sleep(DateTime.Now.Millisecond); // get a random number
                }
            }
        }

        /// <summary>
        /// Processes client request
        /// </summary>
        /// <param name="obj">null</param>
        /// <returns>null</returns>
        protected object ProcessDependancyRequest(object obj)
        {
            #region Processing Request
            TcpClient objClient = (TcpClient)obj;
            NetworkStream objStream = objClient.GetStream();
            string str = string.Empty;

            try
            {
                str = this.ReadLine(objStream);

                try
                {
                    bool bResult = false;

                    // Obtaining locks and executing the request
                    lock (objMemcachedDependancyLock)
                    {
                        bResult = DependancyParser.ProcessCommand(str, this._objMemcachedDependancy);
                    }

                    if (bResult == true)
                    {
                        SendSuccessMessage(objStream);
                    }
                    else
                    {
                        SendErrorMessage(objStream);
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine(err);
                    SendErrorMessage(objStream);
                }

                objStream.Flush();

                Console.WriteLine();
                Console.WriteLine("Putting the TCP client back in queue.....");

                lock (objLock)
                {
                    this._objClientQueue.Enqueue(objClient);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("Exception occured....connection closed: " + err);
            }
            #endregion

            return null;
        }

        /// <summary>
        /// Initializes and starts the server
        /// </summary>
        /// <param name="obj">null</param>
        /// <returns>null</returns>
        protected object StartListener(object obj)
        {
            // Start the TCP/IP server
            this._objListener.Start();
            Console.WriteLine("Server Starting....port:" + System.Configuration.ConfigurationManager.AppSettings["port"]);

            #region Creating filesystem watcher
            this._objFileSystemWatcher = new FileSystemWatcher(System.Configuration.ConfigurationManager.AppSettings["directory"]);
            this._objFileSystemWatcher.Filter = System.Configuration.ConfigurationManager.AppSettings["fileTypeToMonitor"];
            this._objFileSystemWatcher.EnableRaisingEvents = true;
            this._objFileSystemWatcher.IncludeSubdirectories = false;
            this._objFileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;

            this._objFileSystemWatcher.Changed += new FileSystemEventHandler(_objFileSystemWatcher_Changed);
            this._objFileSystemWatcher.Error += new ErrorEventHandler(_objFileSystemWatcher_Error);
            this._objFileSystemWatcher.Renamed += new RenamedEventHandler(_objFileSystemWatcher_Renamed);
            this._objFileSystemWatcher.Deleted += new FileSystemEventHandler(_objFileSystemWatcher_Deleted);
            #endregion

            // Create a thread that constantly monitors the sockets if they have data
            this._objThreadPool.QueueWorkItem(new WorkItemCallback(CheckForRequest));

            while (true)
            {
                Console.WriteLine("Server is listening for client connections...");
                TcpClient objClient = this._objListener.AcceptTcpClient();

                // Locks the socket queue to add new connection
                lock (objLock)
                {
                    objClient.ReceiveTimeout = int.Parse(System.Configuration.ConfigurationManager.AppSettings["socketKeepAliveTime"]); //Timeout
                    this._objClientQueue.Enqueue(objClient);
                    Console.WriteLine("TCP Client count: " + this._objClientQueue.Count);
                }
            }
        }

        #endregion

        #region IDependancyServer Members

        public void Start()
        {
            this._objThreadPool.Start();
            this._objThreadPool.QueueWorkItem(new WorkItemCallback(StartListener), null);
        }

        public void Stop()
        {
            this._objListener.Stop();
            this._objThreadPool.Shutdown();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this._objDependancy.Clear();
            this._objListener = null;
        }

        #endregion
    }
}
