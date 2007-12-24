using System;
using System.Collections.Generic;
using System.Text;
using Memcached.ClientLibrary;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Collections;

namespace CacheProvider
{
    public abstract class CacheProvider : ProviderBase
    {
        public abstract long Count { get;}
        public abstract string[] Servers { get;}
        public abstract long DefaultExpireTime { get;set;}
        public abstract bool Add(string strKey, object objValue);
        public abstract bool Add(string strKey, object objValue,bool bDefaultExpire);        
        public abstract bool Add(string strKey, object objValue, long lNumofMilliSeconds);
        public abstract object Get(string strKey);
        public abstract bool RemoveAll();
        public abstract object Remove(string strKey);
        public abstract void Shutdown();
        public abstract bool KeyExists(string strKey);
        public abstract Hashtable GetStats();
    }
}
