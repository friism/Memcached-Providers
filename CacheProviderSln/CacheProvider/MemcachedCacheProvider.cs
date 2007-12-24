using System;
using System.Collections.Generic;
using System.Text;
using Memcached.ClientLibrary;
using CacheProvider.Properties;
using System.Diagnostics;
using System.Collections;

namespace CacheProvider
{
    public class MemcachedCacheProvider : CacheProvider
    {
        #region Membership Variables
        private SockIOPool _objPool;
        private const string _privatePoolName = "MyCache"; // Pool Name
        private long _lDefaultExpireTime = 2000; // default Expire Time
        private string _strServerList; // Server list
        private int _iSocketConnectTimeout = 500;
        private int _iSocketTimeout = 500;
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

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {            
            // Initialize values from Web.config.
            if (null == config)
            {
                throw (new ArgumentNullException("config"));
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "MemcachedCacheProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Memcache Cache Provider");
            }
            // Call the base class implementation.
            base.Initialize(name, config);

            // Load configuration data.            
            _lDefaultExpireTime =
                Convert.ToInt64(ConfigurationUtil.GetConfigValue(config["defaultExpireTime"], "2000"));
            _iSocketConnectTimeout =
                Convert.ToInt32(ConfigurationUtil.GetConfigValue(config["socketConnectTimeout"], "500"));
            _iSocketTimeout =
                Convert.ToInt32(ConfigurationUtil.GetConfigValue(config["socketTimeout"], "500"));

            string strServers = config["servers"];

            if (string.IsNullOrEmpty(strServers))
            {
                throw new Exception(Resource.No_Cache_Server);
            }

            _strServerList = strServers;

            InitializePool(GetServerArray(_strServerList));            
        }
        #endregion

        #region CacheProvider
        public override long Count
        {
            get 
            {
                return -1;
            }
        }

        public override string[] Servers
        {
            get 
            {
                return GetServerArray(_strServerList); 
            }
        }

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
            MemcachedClient objClient = GetClient();
            try
            {
                if (bDefaultExpire)
                {
                    return objClient.Set(strKey, objValue, DateTime.Now.AddMilliseconds(_lDefaultExpireTime));
                }
                else
                {
                    return objClient.Set(strKey, objValue); 
                }
            }catch{return false;}
        }

        public override bool Add(string strKey, object objValue)
        {
            MemcachedClient objClient = GetClient();
            try
            {
                return objClient.Set(strKey, objValue);
            }catch { return false; }            
        }
        
        public override bool Add(string strKey, object objValue, long lNumOfMilliSeconds)
        {
            MemcachedClient objClient = GetClient();
            try
            {
                return objClient.Set(strKey, objValue, DateTime.Now.AddMilliseconds(lNumOfMilliSeconds));
            }
            catch { return false; }
        }

        public override object Get(string strKey)
        {
            MemcachedClient objClient = GetClient();
            try
            {
                return objClient.Get(strKey);
            }
            catch { return null; }
        }

        public override bool RemoveAll()
        {
            MemcachedClient objClient = GetClient();
            try
            {
                return objClient.FlushAll();
            }
            catch { return false; }

        }

        public override object Remove(string strKey)
        {
            MemcachedClient objClient = GetClient();
            try
            {
                object obj = objClient.Get(strKey);
                objClient.Delete(strKey);
                return obj;
            }
            catch { return null; }
        }

        public override void Shutdown()
        {            
            ShutdownPool();
        }

        public override bool KeyExists(string strKey)
        {
            MemcachedClient objClient = GetClient();
            try
            {
                return objClient.KeyExists(strKey);
            }
            catch { return false; }
        }

        public override Hashtable GetStats()
        {
            MemcachedClient objClient = GetClient();
            try
            {
                return objClient.Stats();
            }
            catch { return null; }
        }
        #endregion

        #region Private Methods
        private void InitializePool(string[] strServerList)
        {
            Debug.WriteLine("#-----InitializePool called----#");
            _objPool = SockIOPool.GetInstance(_privatePoolName);            
            _objPool.SetServers(strServerList);
            _objPool.SocketConnectTimeout = _iSocketConnectTimeout;
            _objPool.SocketTimeout = _iSocketTimeout;
            _objPool.Initialize();
           Debug.WriteLine(string.Format( "--------Is Pool Initialized: {0} ---------",_objPool.Initialized));      
        }

        private void ShutdownPool()
        {
            _objPool.Shutdown();
            Debug.WriteLine("#-----Pool Shutdown----#");
        }

        private MemcachedClient GetClient()
        {
            MemcachedClient objClient = new MemcachedClient();
            objClient.PoolName = _privatePoolName;
            objClient.EnableCompression = false;
            
            return objClient;
        }
        
        private string[] GetServerArray(string strServerList)
        {
            return strServerList.Split(',');
        }

        private long GetCount()
        {
            return -1;
        }
        #endregion
    }
}
