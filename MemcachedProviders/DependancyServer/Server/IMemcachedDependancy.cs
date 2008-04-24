using System;
using System.Collections.Generic;
using System.Text;

namespace DependancyServer.Server
{
    
    public interface IMemcachedDependancy : IDisposable
    {
        MemcachedDependancy Type { get;}
        string DependancyKey { get;}	
    }

    public enum MemcachedDependancy { File = 0, Sql2005 = 1, OtherKey = 2};
}
