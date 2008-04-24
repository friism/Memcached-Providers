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
    public class MemcachedDependancyImpl : IDependancyServer
    {
        private TcpListener _objListener;
        private LinkedList<IMemcachedDependancy> _objDependancy;
        private object _object = new object();
        private SmartThreadPool _objThreadPool;
        private Queue<TcpClient> _objClientQueue;
        private object objLock = new object();

        public MemcachedDependancyImpl()
        {
            this._objListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5050);
            this._objDependancy = new LinkedList<IMemcachedDependancy>();
            this._objThreadPool = new SmartThreadPool(100,100,1);
            this._objClientQueue = new Queue<TcpClient>();
        }

        protected object StartListener(object obj)
        {            
            this._objListener.Start();
            Console.WriteLine("Server Starting....");

            while (true)
            {
                Console.WriteLine("Server is listening...");
                TcpClient objClient = this._objListener.AcceptTcpClient();

                lock (objLock)
                {
                    this._objClientQueue.Enqueue(objClient);
                }

                Console.WriteLine("Calling a WorkItemCallback ProcessRequest");
                this._objThreadPool.QueueWorkItem(new WorkItemCallback(ProcessRequest));
            }           
        }

        protected object ProcessRequest(object obj)
        {
            Console.WriteLine("Processing TCP Client...");
            Console.WriteLine();

            byte[] objData = new byte[256];
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

                            if (objClient.Connected == true)
                            {
                                break;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    Thread.Sleep(DateTime.Now.Millisecond); // get a random number
                }
            }

            NetworkStream objStream = objClient.GetStream();
            string str = string.Empty;
            
            while (!string.IsNullOrEmpty(str = this.ReadLine(objStream)))
            {                
                Thread.Sleep(200);
                try
                {
                    Console.WriteLine("Thread " + Thread.CurrentThread.ManagedThreadId + ": " + str);
                    Write(GetCommandBuffer("STORED").Array, GetCommandBuffer("STORED").Offset, GetCommandBuffer("STORED").Count,
                        objStream);
                    Console.WriteLine("Sending response....");
                    objStream.Flush();
                    Console.WriteLine();
                    Console.WriteLine("Waiting.....");
                }
                catch
                {
                    return null;
                }
            }

            return null;
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

        #region IDependancyServer Members

        public void Start()
        {
            this._objThreadPool.Start();
            this._objThreadPool.QueueWorkItem(new WorkItemCallback(StartListener),null);
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
            this._object = null;
        }

        #endregion       
    }
}
