using System;
using System.Collections.Generic;
using System.Text;
using Enyim.Caching;

namespace MemcachedProviders.Common
{
    internal class MemcachedClientService
    {
        private static MemcachedClientService _instance = new MemcachedClientService();
        private MemcachedClient _client;

        private MemcachedClientService()
        {
            this._client = new MemcachedClient();
        }

        public MemcachedClient Client
        {
            get { return this._client; }
        }

        public static MemcachedClientService Instance 
        {
            get { return _instance; }            
        }
    }
}
