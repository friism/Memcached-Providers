#region [License]
/**
 Apache License
Version 2.0, January 2004
http://www.apache.org/licenses/

TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

1. Definitions.

"License" shall mean the terms and conditions for use, reproduction, and distribution as defined by Sections 1 through 9 of this document.

"Licensor" shall mean the copyright owner or entity authorized by the copyright owner that is granting the License.

"Legal Entity" shall mean the union of the acting entity and all other entities that control, are controlled by, or are under common control with that entity. For the purposes of this definition, "control" means (i) the power, direct or indirect, to cause the direction or management of such entity, whether by contract or otherwise, or (ii) ownership of fifty percent (50%) or more of the outstanding shares, or (iii) beneficial ownership of such entity.

"You" (or "Your") shall mean an individual or Legal Entity exercising permissions granted by this License.

"Source" form shall mean the preferred form for making modifications, including but not limited to software source code, documentation source, and configuration files.

"Object" form shall mean any form resulting from mechanical transformation or translation of a Source form, including but not limited to compiled object code, generated documentation, and conversions to other media types.

"Work" shall mean the work of authorship, whether in Source or Object form, made available under the License, as indicated by a copyright notice that is included in or attached to the work (an example is provided in the Appendix below).

"Derivative Works" shall mean any work, whether in Source or Object form, that is based on (or derived from) the Work and for which the editorial revisions, annotations, elaborations, or other modifications represent, as a whole, an original work of authorship. For the purposes of this License, Derivative Works shall not include works that remain separable from, or merely link (or bind by name) to the interfaces of, the Work and Derivative Works thereof.

"Contribution" shall mean any work of authorship, including the original version of the Work and any modifications or additions to that Work or Derivative Works thereof, that is intentionally submitted to Licensor for inclusion in the Work by the copyright owner or by an individual or Legal Entity authorized to submit on behalf of the copyright owner. For the purposes of this definition, "submitted" means any form of electronic, verbal, or written communication sent to the Licensor or its representatives, including but not limited to communication on electronic mailing lists, source code control systems, and issue tracking systems that are managed by, or on behalf of, the Licensor for the purpose of discussing and improving the Work, but excluding communication that is conspicuously marked or otherwise designated in writing by the copyright owner as "Not a Contribution."

"Contributor" shall mean Licensor and any individual or Legal Entity on behalf of whom a Contribution has been received by Licensor and subsequently incorporated within the Work.

2. Grant of Copyright License.

Subject to the terms and conditions of this License, each Contributor hereby grants to You a perpetual, worldwide, non-exclusive, no-charge, royalty-free, irrevocable copyright license to reproduce, prepare Derivative Works of, publicly display, publicly perform, sublicense, and distribute the Work and such Derivative Works in Source or Object form.

3. Grant of Patent License.

Subject to the terms and conditions of this License, each Contributor hereby grants to You a perpetual, worldwide, non-exclusive, no-charge, royalty-free, irrevocable (except as stated in this section) patent license to make, have made, use, offer to sell, sell, import, and otherwise transfer the Work, where such license applies only to those patent claims licensable by such Contributor that are necessarily infringed by their Contribution(s) alone or by combination of their Contribution(s) with the Work to which such Contribution(s) was submitted. If You institute patent litigation against any entity (including a cross-claim or counterclaim in a lawsuit) alleging that the Work or a Contribution incorporated within the Work constitutes direct or contributory patent infringement, then any patent licenses granted to You under this License for that Work shall terminate as of the date such litigation is filed.

4. Redistribution.

You may reproduce and distribute copies of the Work or Derivative Works thereof in any medium, with or without modifications, and in Source or Object form, provided that You meet the following conditions:

1. You must give any other recipients of the Work or Derivative Works a copy of this License; and

2. You must cause any modified files to carry prominent notices stating that You changed the files; and

3. You must retain, in the Source form of any Derivative Works that You distribute, all copyright, patent, trademark, and attribution notices from the Source form of the Work, excluding those notices that do not pertain to any part of the Derivative Works; and

4. If the Work includes a "NOTICE" text file as part of its distribution, then any Derivative Works that You distribute must include a readable copy of the attribution notices contained within such NOTICE file, excluding those notices that do not pertain to any part of the Derivative Works, in at least one of the following places: within a NOTICE text file distributed as part of the Derivative Works; within the Source form or documentation, if provided along with the Derivative Works; or, within a display generated by the Derivative Works, if and wherever such third-party notices normally appear. The contents of the NOTICE file are for informational purposes only and do not modify the License. You may add Your own attribution notices within Derivative Works that You distribute, alongside or as an addendum to the NOTICE text from the Work, provided that such additional attribution notices cannot be construed as modifying the License.

You may add Your own copyright statement to Your modifications and may provide additional or different license terms and conditions for use, reproduction, or distribution of Your modifications, or for any such Derivative Works as a whole, provided Your use, reproduction, and distribution of the Work otherwise complies with the conditions stated in this License.

5. Submission of Contributions.

Unless You explicitly state otherwise, any Contribution intentionally submitted for inclusion in the Work by You to the Licensor shall be under the terms and conditions of this License, without any additional terms or conditions. Notwithstanding the above, nothing herein shall supersede or modify the terms of any separate license agreement you may have executed with Licensor regarding such Contributions.

6. Trademarks.

This License does not grant permission to use the trade names, trademarks, service marks, or product names of the Licensor, except as required for reasonable and customary use in describing the origin of the Work and reproducing the content of the NOTICE file.

7. Disclaimer of Warranty.

Unless required by applicable law or agreed to in writing, Licensor provides the Work (and each Contributor provides its Contributions) on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied, including, without limitation, any warranties or conditions of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A PARTICULAR PURPOSE. You are solely responsible for determining the appropriateness of using or redistributing the Work and assume any risks associated with Your exercise of permissions under this License.

8. Limitation of Liability.

In no event and under no legal theory, whether in tort (including negligence), contract, or otherwise, unless required by applicable law (such as deliberate and grossly negligent acts) or agreed to in writing, shall any Contributor be liable to You for damages, including any direct, indirect, special, incidental, or consequential damages of any character arising as a result of this License or out of the use or inability to use the Work (including but not limited to damages for loss of goodwill, work stoppage, computer failure or malfunction, or any and all other commercial damages or losses), even if such Contributor has been advised of the possibility of such damages.

9. Accepting Warranty or Additional Liability.

While redistributing the Work or Derivative Works thereof, You may choose to offer, and charge a fee for, acceptance of support, warranty, indemnity, or other liability obligations and/or rights consistent with this License. However, in accepting such obligations, You may act only on Your own behalf and on Your sole responsibility, not on behalf of any other Contributor, and only if You agree to indemnify, defend, and hold each Contributor harmless for any liability incurred by, or claims asserted against, such Contributor by reason of your accepting any such warranty or additional liability. 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections;
using MemcachedProviders.Properties;
using MemcachedProviders.Common;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedProviders.Cache
{
    public class MemcachedCacheProvider : CacheProvider
    {
        #region Membership Variables
        private long _lDefaultExpireTime = 2000; // default Expire Time
        private string _strKeySuffix = string.Empty;
        private MemcachedClient _client = null;
        private string _strCacheCatName = "Memcached Cache Provider";
        private string _strTotalOpName = "# operations executed";
        private string _strOpPerSecName = "# operations / sec";
        private string _strAddOpName = "# of add operations executed";
        private string _strGetOpName = "# of get operations executed";
        private string _strAddOpPerSecName = "# of add operations / sec";
        private string _strGetOpPerSecName = "# of get operations / sec";
        private PerformanceCounter _objTotalOperations;
        private PerformanceCounter _objOperationsPerSecond;
        private PerformanceCounter _objAddOperations;
        private PerformanceCounter _objGetOperations;
        private PerformanceCounter _objAddPerSecondOperations;
        private PerformanceCounter _objGetPerSecondOperations;
        #endregion
               

        #region ProviderBase Methods
        public override string Name
        {
            get
            {
                return "MemcachedCacheProvider";
            }
        }

        public override string Description
        {
            get
            {
                return "MemcachedCacheProvider";
            }
        }

        public override void Initialize(string name, 
            System.Collections.Specialized.NameValueCollection config)
        {            
            // Initialize values from Web.config.
            if (null == config)
            {
                throw (new ArgumentNullException("config"));
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "MemcachedProviders.CacheProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Memcached Cache Provider");
            }
            // Call the base class implementation.
            base.Initialize(name, config);

            // Load configuration data.            
            _lDefaultExpireTime =
                Convert.ToInt64(ConfigurationUtil.GetConfigValue(config["defaultExpireTime"], "2000"));
            
            _strKeySuffix =
                ConfigurationUtil.GetConfigValue(config["keySuffix"], string.Empty);

            this._client = MemcachedProviders.Common.MemcachedClientService.Instance.Client;
            
        }
        #endregion

        #region Cache Provider
        
        public override long DefaultExpireTime
        {
            get
            {
                return _lDefaultExpireTime;
            }
            set
            {
                _lDefaultExpireTime = value;
            }
        }

        public override bool Add(string strKey, object objValue, bool bDefaultExpire)
        {
            this.IncrementTotalOperPC();
            this.IncrementAddOperPC();
            if (bDefaultExpire == true)
            {
                return this._client.Store(StoreMode.Add, _strKeySuffix + strKey, objValue,
                    DateTime.Now.AddMilliseconds(this._lDefaultExpireTime));
            }
            else
            {
                return this._client.Store(StoreMode.Set, _strKeySuffix + strKey, objValue); 
            }
        }

        public override bool Add(string strKey, object objValue)
        {
            this.IncrementTotalOperPC();
            this.IncrementAddOperPC();
            return this._client.Store(StoreMode.Set, _strKeySuffix + strKey, objValue); 
        }
        
        public override bool Add(string strKey, object objValue, long lNumOfMilliSeconds)
        {
            this.IncrementTotalOperPC();
            this.IncrementAddOperPC();
            return this._client.Store(StoreMode.Set, _strKeySuffix + strKey, objValue,
                    DateTime.Now.AddMilliseconds(lNumOfMilliSeconds));
        }

        public override bool Add(string strKey, object objValue, TimeSpan timeSpan)
        {
            this.IncrementTotalOperPC();
            this.IncrementAddOperPC();
            return this._client.Store(StoreMode.Set, _strKeySuffix + strKey,
                objValue, timeSpan);
        }

        public override object Get(string strKey)
        {
            this.IncrementTotalOperPC();
            this.IncrementGetOperPC();
            return this._client.Get(_strKeySuffix + strKey);
        }

        /// <summary>
        /// This method will work with memcached 1.2.4 and higher
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override IDictionary<string, object> Get(params string[] keys)
        {
            this.IncrementTotalOperPC();
            this.IncrementGetOperPC();

            IList<string> keysList = new List<string>();

            foreach (string str in keys)
            {
                keysList.Add(_strKeySuffix + str); 
            }

            IDictionary<string, object> _ret = this._client.Get(keysList);
            IDictionary<string, object> _retVal = new Dictionary<string, object>();

            foreach (string str in _ret.Keys)
            {
                _retVal.Add(str.Remove(0, _strKeySuffix.Length), _ret[str]); 
            }

            return _retVal;
        }

        public override void RemoveAll()
        {
            this.IncrementTotalOperPC();
            this._client.FlushAll();
        }

        public override bool Remove(string strKey)
        {
            this.IncrementTotalOperPC();
            return this._client.Remove(_strKeySuffix + strKey);            
        }

        public override string KeySuffix
        {
            get
            {
                return this._strKeySuffix;
            }
            set
            {
                this._strKeySuffix = value;
            }
        }
        
        public override T Get<T>(string strKey)
        {
            this.IncrementTotalOperPC();
            this.IncrementGetOperPC();
            return this._client.Get<T>(_strKeySuffix+strKey);
        }

        public override long Increment(string strKey, long lAmount)
        {
            this.IncrementTotalOperPC();
            return this._client.Increment(_strKeySuffix +strKey, (uint)lAmount);
        }

        public override long Decrement(string strKey, long lAmount)
        {
            this.IncrementTotalOperPC();
            return this._client.Decrement(_strKeySuffix + strKey, (uint)lAmount);
        }
        #endregion

        #region Performance Counter Methods
        internal override void CheckPerformanceCounterCategories()
        {
            if (!PerformanceCounterCategory.Exists(_strCacheCatName))
            {
                CounterCreationDataCollection counters = new CounterCreationDataCollection();
                                
                CounterCreationData totalOps = new CounterCreationData();
                totalOps.CounterName = _strTotalOpName;
                totalOps.CounterHelp = "Total number of operations executed";
                totalOps.CounterType = PerformanceCounterType.NumberOfItems32;
                counters.Add(totalOps);
                                
                CounterCreationData opsPerSecond = new CounterCreationData();
                opsPerSecond.CounterName = _strOpPerSecName;
                opsPerSecond.CounterHelp = "Number of operations executed per second";
                opsPerSecond.CounterType = PerformanceCounterType.RateOfCountsPerSecond32;
                counters.Add(opsPerSecond);
                                
                CounterCreationData addOps = new CounterCreationData();
                addOps.CounterName = _strAddOpName;
                addOps.CounterHelp = "Number of add operations execution";
                addOps.CounterType = PerformanceCounterType.NumberOfItems32;
                counters.Add(addOps);

                CounterCreationData addOpsPerSec = new CounterCreationData();
                addOpsPerSec.CounterName = _strAddOpPerSecName;
                addOpsPerSec.CounterHelp = "Number of add operations per second";
                addOpsPerSec.CounterType = PerformanceCounterType.RateOfCountsPerSecond32;
                counters.Add(addOpsPerSec);
                                
                CounterCreationData getOps = new CounterCreationData();
                getOps.CounterName = _strGetOpName;
                getOps.CounterHelp = "Number of get operations execution";
                getOps.CounterType = PerformanceCounterType.NumberOfItems32;
                counters.Add(getOps);

                CounterCreationData getOpsPerSec = new CounterCreationData();
                getOpsPerSec.CounterName = _strGetOpPerSecName;
                getOpsPerSec.CounterHelp = "Number of get operations per second";
                getOpsPerSec.CounterType = PerformanceCounterType.RateOfCountsPerSecond32;
                counters.Add(getOpsPerSec);
                
                // create new category with the counters above
                PerformanceCounterCategory.Create(_strCacheCatName,
                        "Memcached Cache Provider Performance Counter", 
                        PerformanceCounterCategoryType.SingleInstance, counters);
            }

            // create counters to work with
            _objTotalOperations = new PerformanceCounter();
            _objTotalOperations.CategoryName = _strCacheCatName;
            _objTotalOperations.CounterName = _strTotalOpName;
            _objTotalOperations.MachineName = ".";
            _objTotalOperations.ReadOnly = false;
            _objTotalOperations.RawValue = 0;

            _objOperationsPerSecond = new PerformanceCounter();
            _objOperationsPerSecond.CategoryName = _strCacheCatName;
            _objOperationsPerSecond.CounterName = _strOpPerSecName;
            _objOperationsPerSecond.MachineName = ".";
            _objOperationsPerSecond.ReadOnly = false;
            _objOperationsPerSecond.RawValue = 0;

            _objAddOperations = new PerformanceCounter();
            _objAddOperations.CategoryName = _strCacheCatName;
            _objAddOperations.CounterName = _strAddOpName;
            _objAddOperations.MachineName = ".";
            _objAddOperations.ReadOnly = false;
            _objAddOperations.RawValue = 0;

            _objAddPerSecondOperations = new PerformanceCounter();
            _objAddPerSecondOperations.CategoryName = _strCacheCatName;
            _objAddPerSecondOperations.CounterName = _strAddOpPerSecName;
            _objAddPerSecondOperations.MachineName = ".";
            _objAddPerSecondOperations.ReadOnly = false;
            _objAddPerSecondOperations.RawValue = 0;

            _objGetOperations = new PerformanceCounter();
            _objGetOperations.CategoryName = _strCacheCatName;
            _objGetOperations.CounterName = _strGetOpName;
            _objGetOperations.MachineName = ".";
            _objGetOperations.ReadOnly = false;
            _objGetOperations.RawValue = 0;

            _objGetPerSecondOperations = new PerformanceCounter();
            _objGetPerSecondOperations.CategoryName = _strCacheCatName;
            _objGetPerSecondOperations.CounterName = _strGetOpPerSecName;
            _objGetPerSecondOperations.MachineName = ".";
            _objGetPerSecondOperations.ReadOnly = false;
            _objGetPerSecondOperations.RawValue = 0;


        }

        internal void IncrementTotalOperPC()
        {
            this._objTotalOperations.Increment();
            this._objOperationsPerSecond.Increment();
        }

        internal void IncrementAddOperPC()
        {
            this._objAddOperations.Increment();
            this._objAddPerSecondOperations.Increment();
        }

        internal void IncrementGetOperPC()
        {
            this._objGetOperations.Increment();
            this._objGetPerSecondOperations.Increment();
        }
        #endregion
    }
}
