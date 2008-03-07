using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections;
using MemcachedProviders.Properties;
using MemcachedProviders.Common;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedProviders.Cache
{
    public class MemcachedCacheProvider : CacheProvider
    {
        #region Membership Variables
        private long _lDefaultExpireTime = 2000; // default Expire Time
        private string _strKeySuffix = string.Empty;
        private MemcachedClient _client = null;
        #endregion

        #region ProviderBase Methods
        public override string Name
        {
            get
            {
                return "MemcachedCacheProvider";
            }
        }

        public override string Description
        {
            get
            {
                return "MemcachedCacheProvider";
            }
        }

        public override void Initialize(string name, 
            System.Collections.Specialized.NameValueCollection config)
        {            
            // Initialize values from Web.config.
            if (null == config)
            {
                throw (new ArgumentNullException("config"));
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "MemcachedProviders.CacheProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Memcached Cache Provider");
            }
            // Call the base class implementation.
            base.Initialize(name, config);

            // Load configuration data.            
            _lDefaultExpireTime =
                Convert.ToInt64(ConfigurationUtil.GetConfigValue(config["defaultExpireTime"], "2000"));
            
            _strKeySuffix =
                ConfigurationUtil.GetConfigValue(config["keySuffix"], string.Empty);

            this._client = MemcachedProviders.Common.MemcachedClientService.Instance.Client;
            
        }
        #endregion

        #region Cache Provider
        
        public override long DefaultExpireTime
        {
            get
            {
                return _lDefaultExpireTime;
            }
            set
            {
                _lDefaultExpireTime = value;
            }
        }

        public override bool Add(string strKey, object objValue, bool bDefaultExpire)
        {
            if (bDefaultExpire == true)
            {
                return this._client.Store(StoreMode.Add, _strKeySuffix + strKey, objValue,
                    DateTime.Now.AddMilliseconds(this._lDefaultExpireTime));
            }
            else
            {
                return this._client.Store(StoreMode.Set, _strKeySuffix + strKey, objValue); 
            }
        }

        public override bool Add(string strKey, object objValue)
        {
            return this._client.Store(StoreMode.Set, _strKeySuffix + strKey, objValue); 
        }
        
        public override bool Add(string strKey, object objValue, long lNumOfMilliSeconds)
        {
            return this._client.Store(StoreMode.Set, _strKeySuffix + strKey, objValue,
                    DateTime.Now.AddMilliseconds(lNumOfMilliSeconds));
        }

        public override bool Add(string strKey, object objValue, TimeSpan timeSpan)
        {
            return this._client.Store(StoreMode.Set, _strKeySuffix + strKey,
                objValue, timeSpan);
        }

        public override object Get(string strKey)
        {            
            return this._client.Get(_strKeySuffix + strKey);
        }

        /// <summary>
        /// This method will work with memcached 1.2.4 and higher
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override IDictionary<string, object> Get(params string[] keys)
        {
            IList<string> keysList = new List<string>();

            foreach (string str in keys)
            {
                keysList.Add(_strKeySuffix + str); 
            }

            IDictionary<string, object> _ret = this._client.Get(keysList);
            IDictionary<string, object> _retVal = new Dictionary<string, object>();

            foreach (string str in _ret.Keys)
            {
                _retVal.Add(str.Remove(0, _strKeySuffix.Length), _ret[str]); 
            }

            return _retVal;
        }

        public override void RemoveAll()
        {
            this._client.FlushAll();
        }

        public override bool Remove(string strKey)
        {
            return this._client.Remove(_strKeySuffix + strKey);            
        }

        public override string KeySuffix
        {
            get
            {
                return this._strKeySuffix;
            }
            set
            {
                this._strKeySuffix = value;
            }
        }
        
        public override T Get<T>(string strKey)
        {
            return this._client.Get<T>(_strKeySuffix+strKey);
        }

        public override long Increment(string strKey, long lAmount)
        {
            return this._client.Increment(_strKeySuffix +strKey, (uint)lAmount);
        }

        public override long Decrement(string strKey, long lAmount)
        {
            return this._client.Decrement(_strKeySuffix + strKey, (uint)lAmount);
        }
        #endregion
    }
}
