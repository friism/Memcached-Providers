using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security;

namespace DependancyServer.Server
{
    
    public interface IMemcachedDependancy : IDisposable
    {
        MemcachedDependancy Type { get;}
        string DependancyKey { get;}
    }
        
    internal class FileMemcachedDependancy : IMemcachedDependancy
    {
        private string _strDependancyKey;
        private string _strFilePath;
                
        public FileMemcachedDependancy(string strDependancyKey, string strFilePath)
        {
            this._strDependancyKey = strDependancyKey;            
            this._strFilePath = strFilePath;
        }       

        #region IMemcachedDependancy Members

        public MemcachedDependancy Type
        {
            get { return MemcachedDependancy.File; }
        }

        public string DependancyKey
        {
            get { return this._strDependancyKey; }
        }
               
        #endregion

        #region IDisposable Members

        public void Dispose()
        {            
        }

        #endregion
    }

    public enum MemcachedDependancy { File = 0, Sql2005 = 1, OtherKey = 2};
}
