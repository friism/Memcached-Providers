using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Core;
using NUnit.Framework;
using CacheProvider;
using System.Diagnostics;
using System.Collections;
using MemcachedProviders.Cache;
using System.Threading;

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
            Debug.WriteLine("------Memcached Client Shutdown------");
        }

        [NUnit.Framework.Test]
        public void Add_String_Positive_Test()
        {
            Debug.WriteLine("-------------Add_String_Positive_Test()------------");
            string strKey = "firstname";
            string strValue = "Fahad";
            string strReturned = string.Empty;
            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Setting"))
            {
                Assert.IsTrue(DistCache.Add(strKey, strValue),"Error adding value");
            }

            Debug.WriteLine("-------------------");

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Getting"))
            {
                strReturned = DistCache.Get(strKey) as string;
            }

            Debug.WriteLine("Returned: "+ (strReturned == null));

            Assert.IsTrue(strValue.Equals(strReturned),string.Format("Not the same: {0}",strReturned));
        }

        [NUnit.Framework.Test]
        public void Get_Multiple_Positive_Test()
        {
            Debug.WriteLine("-------------Get_Multiple_Positive_Test()------------");
            
            string[] keys = new string[10];

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Setting"))
            {
                for (int i = 0; i < 10; i++)
                {
                    DistCache.Add(i.ToString(), i * 2);
                    keys[i] = i.ToString();
                }
            }

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Getting"))
            {
                IDictionary<string, object> _retVal = DistCache.Get(keys);

                for (int i = 0; i < 10; i++)
                {
                    Assert.IsTrue( ((int)_retVal[keys[i]]) == (i * 2));
                }                
            }
 
        }

        [NUnit.Framework.Test]
        public void Get_Template_Positive_Test()
        {
            Debug.WriteLine("-------------Get_Template_Positive_Test()------------");
            string strKey = "firstname";
            string strValue = "Fahad";
            string strReturned = string.Empty;
            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Setting"))
            {
                Assert.IsTrue(DistCache.Add(strKey, strValue), "Error adding value");
            }

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Getting"))
            {
                strReturned = DistCache.Get<string>(strKey);
            }
            
            Assert.IsTrue(strValue.Equals(strReturned), string.Format("Not the same: {0}", strReturned)); 
        }

        [NUnit.Framework.Test]
        public void CheckExpire_Max_Positive_Test()
        {
            Debug.WriteLine("-------------CheckExpire_Max_Positive_Test()------------");
            string strKey = "name";
            string strValue = "Fahad";

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Setting"))
            {
                Assert.IsTrue(DistCache.Add(strKey, strValue));
            }

            System.Threading.Thread.Sleep(new TimeSpan(0,0,5));

            using (CalcTimeSpan calc = new CalcTimeSpan("Time for Getting"))
            {
                Assert.IsNotNull(DistCache.Get(strKey));
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
