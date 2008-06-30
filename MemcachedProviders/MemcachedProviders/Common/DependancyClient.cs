using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;

namespace MemcachedProviders.Common
{
    public interface IDependancyClient
    {
        bool SetFileDependancy(string strFilename,params string []str);        
    }

    class DependancyClientImpl : IDependancyClient
    {
        private readonly ArraySegment<byte> _objDataTerminator = new ArraySegment<byte>(new byte[2] { (byte)'\r', (byte)'\n' });
        private Queue<TcpClient> _objSocketQueue = null;                
        private object _objLock = new object();        

        public DependancyClientImpl(string strServerAddr, int iPort, int iSocketPoolSize)
        {
            this._objSocketQueue = new Queue<TcpClient>(iSocketPoolSize);
            
            for (int i = 0; i < iSocketPoolSize; i++)
            {
                this._objSocketQueue.Enqueue(new TcpClient(strServerAddr, iSocketPoolSize));                
            }
        }

        #region IDependancyClient Members

        public bool SetFileDependancy(string strFilename, params string[] str)
        {
            try
            {
                 
            }
            catch(Exception err)
            {
                Debug.WriteLine(err.ToString());
                return false;                
            }
        }

        private string CreateSetFileDependancyCommand(string strFilename, params string[] strKeys)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("FILE ");
            sb.Append(Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(strFilename)));
            sb.Append(" ");

            foreach (string str in strKeys)
            {
                sb.Append(strKeys);
                sb.Append(" ");
            }

            return sb.ToString().Trim();
        }

        #endregion
    }

    public static class DependancyClientFactory
    {
        public static IDependancyClient CreateDependancyClient(string strServerAddr, int iPort, int iSocketPoolSize)
        {
            return new DependancyClientImpl(strServerAddr, iPort, iSocketPoolSize);
        }
   
    }


}
