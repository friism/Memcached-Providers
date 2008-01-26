using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Collections;

namespace MemcachedProviders.Cache
{
    public static class DistCache
    {
        private static CacheProvider _objProvider = null;
        private static CacheProviderCollection _objProviders = null;
        private static object _lock = new object();

        /// <summary>
        /// This function loads providers from the App.Config or Web.config file
        /// </summary>
        private static void LoadProvider()
        {
            // Avoid claiming lock if provider already loaded
            if (_objProvider == null)
            {
                lock (_lock)
                {
                    //make sure _provider is still null
                    if (_objProvider == null)
                    {
                        //Get a reference to the <cacheProvider> section 
                        CacheProviderSection objSection = (CacheProviderSection)
                            WebConfigurationManager.GetSection("cacheProvider");

                        //Load registered providers and point _objProvider to the default provider
                        _objProviders = new CacheProviderCollection();
                        ProvidersHelper.InstantiateProviders
                            (objSection.Providers, _objProviders, typeof(CacheProvider));
                        _objProvider = _objProviders[objSection.DefaultProvider];

                        if (_objProvider == null)
                        {
                            throw new ProviderException("Unable to load default cache provider");
                        }

                    }
                }
            }
        }
        
        public static long DefaultExpireTime
        {
            get 
            {
                LoadProvider();
                return _objProvider.DefaultExpireTime;
            }

            set 
            {
                LoadProvider();
                _objProvider.DefaultExpireTime = value;                
            }
        }

        public static string KeySuffix
        {
            get
            {
                LoadProvider();
                return _objProvider.KeySuffix;
            }

            set 
            {
                LoadProvider();
                _objProvider.KeySuffix = value;
            }
        }

        public static bool Add(string strKey, object objValue)
        {
            LoadProvider();
            return _objProvider.Add(strKey, objValue);
        }

        public static bool Add(string strKey, object objValue, bool bDefaultTimeSpan)
        {
            LoadProvider();
            return _objProvider.Add(strKey, objValue, bDefaultTimeSpan);
        }

        public static bool Add(string strKey, object objValue, long lNumofMilliSeconds)
        {
            LoadProvider();
            return _objProvider.Add(strKey, objValue, lNumofMilliSeconds);
        }

        public static bool Add(string strKey, object objValue, TimeSpan tspan)
        {
            LoadProvider();
            return _objProvider.Add(strKey, objValue, tspan); 
        }
        
        public static object Get(string strKey)
        {
            LoadProvider();
            return _objProvider.Get(strKey);
        }

        public static T Get<T>(string strKey)
        {
            LoadProvider();
            return _objProvider.Get<T>(strKey);
        }

        public static object Remove(string strKey)
        {
            LoadProvider();
            return _objProvider.Remove(strKey);
        }

        public static void RemoveAll()
        {
            LoadProvider();
            _objProvider.RemoveAll();
        }

        public static long Increment(string strKey, long lAmount)
        {
            LoadProvider();
            return _objProvider.Increment(strKey, lAmount); 
        }

        public static long Decrement(string strKey, long lAmount)
        {
            LoadProvider();
            return _objProvider.Decrement(strKey, lAmount); 
        }

    }
}
