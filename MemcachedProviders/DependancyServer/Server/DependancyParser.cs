using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DependancyServer.Server
{
    internal static class DependancyParser
    {
        
        public static bool ProcessCommand(string strCommnd, IDictionary<string, IMemcachedDependancy> objMemcached)        
        {
            string[] str = strCommnd.Split(' ');

            if (str[0].ToUpper() == "FILE")
            {
                // Convert the filename from Base64 string
                string strFileName = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(str[2]));

                // Check if the key does not exists create it otherwise just replace
                if (objMemcached.ContainsKey(strFileName) == false)
                {
                    objMemcached.Add(strFileName, new FileMemcachedDependancy(str[1], strFileName));
                }
                else
                {
                    objMemcached[strFileName] = new FileMemcachedDependancy(str[1], strFileName);
                }

                return true;                
            }

            return false;
        }
        
    }
}
