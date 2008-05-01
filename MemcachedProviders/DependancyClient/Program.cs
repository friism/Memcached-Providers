using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using Amib.Threading;

namespace DependancyClient
{
    public class Program
    {
        private static readonly ArraySegment<byte> DataTerminator = new ArraySegment<byte>(new byte[2] { (byte)'\r', (byte)'\n' });
        private static Queue<TcpClient> objSocketQueue = new Queue<TcpClient>(5);
        static int iSocketPool = 1;
        private static SmartThreadPool objThread;
        private static object objLock = new object();

        public static void Main(string[] args)
        {
            for (int i = 0; i < iSocketPool; i++)
            {
                objSocketQueue.Enqueue(new TcpClient("localhost", 5050));
                Console.WriteLine("TCP Client Created: " + i);
            }

            objThread = new SmartThreadPool(100, 20, 5);
            objThread.Start();

            objThread.QueueWorkItem(new WorkItemCallback(Start));
                       
            Console.ReadLine();

            objThread.Shutdown();
        }

        public static object Start(object obj)
        {
            for (int i = 1; i <= 2; i++)
            {
                objThread.QueueWorkItem(new WorkItemCallback(SendItem));                
            }
            
            return null;
        }

        private static string ReadLine(NetworkStream inputStream)
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

        public static ArraySegment<byte> GetCommandBuffer(string value)
        {
            int valueLength = value.Length;
            byte[] data = new byte[valueLength + 2];

            Encoding.ASCII.GetBytes(value, 0, valueLength, data, 0);

            data[valueLength] = 13;
            data[valueLength + 1] = 10;

            return new ArraySegment<byte>(data);
        }

        public static object SendItem(object obj)
        {
            TcpClient objClient = null;

            while (true)
            {
                if (objSocketQueue.Count > 0)
                {
                    Console.WriteLine("Dequeuing....");

                    lock (objLock)
                    {
                        if (objSocketQueue.Count > 0)
                        {
                            objClient = objSocketQueue.Dequeue();
                        }
                        else
                        {
                            continue;
                        }
                    }

                    Console.WriteLine("Got TCP Client.....");
                    break;                    
                }
                else
                {
                    System.Threading.Thread.Sleep(DateTime.Now.Millisecond);                    
                }
            }
            
            string strFile = "test.txt";
            string strMsg = "FILE MyKey " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(strFile));
            // Translate the passed message into ASCII and store it as a Byte array.

            byte[] data = System.Text.Encoding.ASCII.GetBytes(strMsg);

            NetworkStream stream = objClient.GetStream();

            // Send the message to the connected TcpServer. 
            ArraySegment<byte> objData = GetCommandBuffer(strMsg);
            stream.Write(objData.Array, objData.Offset, objData.Count);
            stream.Flush();

            Console.WriteLine("Trying to read...");
            Console.WriteLine(ReadLine(stream)+ " "+ DateTime.Now);


            lock (objLock)
            {
                objSocketQueue.Enqueue(objClient);
            }

            return null;
        }
    }
}
