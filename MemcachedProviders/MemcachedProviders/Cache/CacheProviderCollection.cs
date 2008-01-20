using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;

namespace MemcachedProviders.Cache
{    

    public class CacheProviderCollection : ProviderCollection
    {
        public new CacheProvider this[string name]
        {
            get
            {
                return (CacheProvider)base[name];
            }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (!(provider is CacheProvider))
                throw new ArgumentException("Invalid provider type", "CacheProvider");

            base.Add(provider);
        }
    }    
}
