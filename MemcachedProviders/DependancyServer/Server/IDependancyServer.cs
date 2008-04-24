using System;
using System.Collections.Generic;
using System.Text;


namespace DependancyServer.Server
{
    public interface IDependancyServer : IDisposable
    {
        void Start();        
        void Stop();
    }
}
