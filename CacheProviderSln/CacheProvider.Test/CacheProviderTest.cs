using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Core;
using NUnit.Framework;
using CacheProvider;
using System.Diagnostics;
using System.Collections;

namespace CacheProvider.Test
{
    [TestFixture]
    public class DistCacheProviderTest
    {
        [NUnit.Framework.TestFixtureSetUp]
        public void SetUp()
        {
            Debug.WriteLine("------Memcached Client StartUp------");
        }

        [NUnit.Framework.TestFixtureTearDown]
        public void TearDown()
        {
            DistCache.Shutdown();
            Debug.WriteLine("------Memcached Client Shutdown------");
        }

        [NUnit.Framework.Test]
        public void Add_int_Positive_Test()
        {
            Debug.WriteLine("-------------Add_int_Positive_Test()------------");
            string strKey = "firstname";
            string strValue = "Fahad";
            string strReturned = string.Empty;
            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Setting"))
            {
                Assert.IsTrue(DistCache.Add(strKey, strValue),"Error adding value");
            }

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Getting"))
            {
                strReturned = DistCache.Get(strKey) as string;
            }

            Assert.IsTrue(strValue.Equals(strReturned),string.Format("Not the same: {0}",strReturned));
        }

        [NUnit.Framework.Test]
        public void CheckExpire_TimeSpan_Positive_Test()
        {
            Debug.WriteLine("-------------CheckExpire_TimeSpan_Positive_Test()------------");
            string strKey = "key11";
            string strValue = "Fahad";

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Setting"))
            {
                Assert.IsTrue(DistCache.Add(strKey, strValue,500));
            }

            System.Threading.Thread.Sleep(2500);

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Getting"))
            {
                Assert.IsNull(DistCache.Get(strKey));
            }
        }

        [NUnit.Framework.Test]
        public void CheckExpire_Default_Positive_Test()
        {
            Debug.WriteLine("-------------CheckExpire_Default_Positive_Test()------------");
            string strKey = "keyDefault";
            string strValue = "Fahad";

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Setting"))
            {
                Assert.IsTrue(DistCache.Add(strKey, strValue,true));
            }

            System.Threading.Thread.Sleep(3000);

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Getting"))
            {
                Assert.IsNull(DistCache.Get(strKey));
            }
        }

        [NUnit.Framework.Test]
        public void CheckExpire_Max_Positive_Test()
        {
            Debug.WriteLine("-------------CheckExpire_Max_Positive_Test()------------");
            string strKey = "keyDefault";
            string strValue = "Fahad";

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Setting"))
            {
                Assert.IsTrue(DistCache.Add(strKey, strValue));
            }

            System.Threading.Thread.Sleep(10000);

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Getting"))
            {
                Assert.IsNotNull(DistCache.Get(strKey));
            }
        }
        
        
        [NUnit.Framework.Test]
        public void DisplayStats_Test()
        {
            Debug.WriteLine("#-------------Start Display Stats----------------#");
            Hashtable table = DistCache.GetStats();

            Assert.IsNotNull(table);

            foreach (object obj in table.Keys)
            {
                Debug.WriteLine("Server Address and Port : "+obj);
                Debug.WriteLine("");
                foreach (object obj1 in ((Hashtable)table[obj]).Keys)
                {
                    Debug.WriteLine(string.Format("{0} -- {1}", obj1, ((Hashtable)table[obj])[obj1]));
                }
                Debug.WriteLine("-----------------------------");
            }
            Debug.WriteLine("#-------------End Display Stats----------------#");
        }

        [NUnit.Framework.Test]
        public void GetCount_Positive_Test()
        {
            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Setting"))
            {
                long lCount = DistCache.Count;
                Debug.WriteLine("Count: "+ lCount);
                Assert.IsTrue( lCount == -1);  
            }            
        }

        [NUnit.Framework.Test]
        public void GetDefaultExpireTime_Positive_Test()
        {
            using (CalcTimeSpan calc = new CalcTimeSpan("Time for setting"))
            {
                long lDefaulttime = DistCache.DefaultExpireTime;
                Debug.WriteLine("Default Expire Time: "+lDefaulttime);
                Assert.IsTrue(lDefaulttime==2000);
            }
        }


        [NUnit.Framework.Test]
        public void GetServers_Positive_Test()
        {
            using (CalcTimeSpan calc = new CalcTimeSpan("Time for setting"))
            {
                string[] strServers = DistCache.Servers;
                int i = 0;
                Debug.WriteLine("Server" + i + ": " + strServers[0]);
                Assert.AreEqual(strServers[0], "127.0.0.1:11211");
            }
        }

    }

    internal class CalcTimeSpan : IDisposable
    {
        DateTime _dStart;
        string _strMessage;
        public CalcTimeSpan(string strMessage)
        {
            _dStart = DateTime.Now;
            _strMessage = strMessage;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Debug.WriteLine(string.Format("{0}: {1}",_strMessage,DateTime.Now.Subtract(_dStart)));
        }

        #endregion
    }
}
