using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Collections;

namespace MemcachedProviders.Cache
{
    public abstract class CacheProvider : ProviderBase
    {               
        public abstract long DefaultExpireTime { get;set;}
        public abstract string KeySuffix { get; set; }
        public abstract bool Add(string strKey, object objValue);
        public abstract bool Add(string strKey, object objValue,bool bDefaultExpire);        
        public abstract bool Add(string strKey, object objValue, long lNumofMilliSeconds);
        public abstract bool Add(string strKey, object objValue, TimeSpan timeSpan);
        public abstract object Get(string strKey);
        public abstract T Get<T>(string strKey);
        public abstract void RemoveAll();
        public abstract bool Remove(string strKey);        
        public abstract long Increment(string key, long amount);
        public abstract long Decrement(string key, long amount);        
    }
}
