using System;
using System.Collections.Generic;
using System.Text;


namespace MemcachedProviders.Session.Memcached
{
    //public class MemcachedOperations
    //{
    //    private string _strPoolName;
    //    private SockIOPool _objPool;
    //    private long _lTimeoutInMilliSec;
    //    private string _strApplicationName;

    //    public MemcachedOperations(string strPoolName, SockIOPool objPool,
    //        long lTimeoutInMilliSec, string strApplicationName)
    //    {
    //        this._lTimeoutInMilliSec = lTimeoutInMilliSec;
    //        this._objPool = objPool;
    //        this._strPoolName = strPoolName;
    //        this._strApplicationName = strApplicationName;
    //    }

    //    private MemcachedClient GetClient()
    //    {
    //        MemcachedClient objClient = new MemcachedClient();
    //        objClient.PoolName = this._strPoolName;
    //        objClient.EnableCompression = false;

    //        return objClient;
    //    }

    //    private string GetKey(string id)
    //    {
    //        return this._strApplicationName + id;
    //    }

    //    public bool Add(string strKey, object objValue)
    //    {
    //        MemcachedClient objClient = GetClient();
            
    //        try
    //        {
    //            return objClient.Set(this.GetKey(strKey), objValue,
    //                DateTime.Now.AddMilliseconds(this._lTimeoutInMilliSec));
    //        }
    //        catch { return false; }
    //    }

    //    public bool Add(string strKey, object objValue, long lTimeout)
    //    {
    //        MemcachedClient objClient = GetClient();

    //        try
    //        {
    //            return objClient.Set(this.GetKey(strKey), objValue,
    //                DateTime.Now.AddMilliseconds(lTimeout));
    //        }
    //        catch { return false; }
    //    }

    //    public object Get(string strKey)
    //    {
    //        MemcachedClient objClient = GetClient();
    //        try
    //        {
    //            return objClient.Get(this.GetKey(strKey));
    //        }
    //        catch { return null; }
    //    }

    //    public object Remove(string strKey)
    //    {
    //        MemcachedClient objClient = GetClient();
    //        try
    //        {
    //            object obj = objClient.Get(this.GetKey(strKey));
    //            objClient.Delete(strKey);
    //            return obj;
    //        }
    //        catch { return null; }
    //    }

    //    public bool KeyExists(string strKey)
    //    {
    //        MemcachedClient objClient = GetClient();
    //        try
    //        {
    //            return objClient.KeyExists(this.GetKey(strKey));
    //        }
    //        catch { return false; }
    //    }

    //}
}
