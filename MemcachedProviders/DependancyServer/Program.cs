using System;
using System.Collections.Generic;
using System.Text;
using DependancyServer.Server;


namespace DependancyServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IDependancyServer objServer = new DependancyServer.Server.MemcachedDependancyImpl();
            objServer.Start();
            Console.ReadLine();
        }
    }
}
