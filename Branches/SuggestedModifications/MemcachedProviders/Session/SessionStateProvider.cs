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
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.SessionState;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using MemcachedProviders.Common;
using MemcachedProviders.Session.Db;
using MemcachedProviders.Session.Memcached;

namespace MemcachedProviders.Session
{
    public class SessionStateProvider : SessionStateStoreProviderBase
    {
        #region Membership Variables

        private const string StrDbOp = "# of database operations executed";
        private const string StrDbOpPerSec = "# of database operations / sec";
        private const string StrMemcachedOp = "# of memcached operations executed";
        private const string StrMemcachedOpPerSec = "# of memcached operations / sec";
        private const string StrOperPerSecName = "# operations / sec";
        private const string StrSessionProviderCat = "Memcached Session Provider";
        private const string StrTotalOperName = "# operations executed";
        private readonly MemcachedClient _client = MemcachedClientService.Instance.Client;
        private bool _bIsDbNone;
        private SessionStateSection _objConfig;
        private ConnectionStringSettings _objConnectionStringSettings;
        private PerformanceCounter _objDbOperations;
        private PerformanceCounter _objDbOperationsPerSec;
        private PerformanceCounter _objMemcachedOperations;
        private PerformanceCounter _objMemcachedOperPerSec;
        private PerformanceCounter _objOperationsPerSecond;
        private PerformanceCounter _objTotalOperations;
        private bool _objWriteExceptionsToEventLog;
        private string _strApplicationName;
        private string _strConn;
        private string _strDbType;

        private int TimeoutInMinutes
        {
            get { return (int) Math.Ceiling(_objConfig.Timeout.TotalMinutes); }
        }

        #endregion

        public bool WriteExceptionsToEventLog
        {
            get { return _objWriteExceptionsToEventLog; }
            set { _objWriteExceptionsToEventLog = value; }
        }

        public string ApplicationName
        {
            get { return _strApplicationName; }
            set { _strApplicationName = value; }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //

            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (String.IsNullOrEmpty(name))
            {
                name = "MemcachedSessionStateStore";
            }

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Memcached Session Provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            //
            // Initialize the ApplicationName property.
            //

            ApplicationName = HostingEnvironment.ApplicationVirtualPath;

            //
            // Get <sessionState> configuration element.
            //

            Configuration cfg = WebConfigurationManager.OpenWebConfiguration(ApplicationName);
            _objConfig = (SessionStateSection) cfg.GetSection("system.web/sessionState");

            if (!string.IsNullOrEmpty(config["dbType"]))
            {
                if (config["dbType"].ToLower() == "none")
                {
                    _strDbType = Common.DbType.None;
                    _bIsDbNone = true;
                }
                else
                {
                    _strDbType = Common.DbType.SQLServer;
                    _bIsDbNone = false;
                    //
                    // Initialize connection string.
                    //
                    _objConnectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

                    if (_objConnectionStringSettings == null || _objConnectionStringSettings.ConnectionString.Trim() == string.Empty)
                    {
                        throw new ProviderException("Connection string cannot be blank.");
                    }

                    _strConn = _objConnectionStringSettings.ConnectionString;
                }
            }

            //Initialize Performance Counter
            CheckPerformanceCounterCategories();

            //
            // Initialize WriteExceptionsToEventLog
            //
            _objWriteExceptionsToEventLog = false;

            if (config["writeExceptionsToEventLog"] != null)
            {
                if (config["writeExceptionsToEventLog"].ToUpper() == "TRUE")
                {
                    _objWriteExceptionsToEventLog = true;
                }
            }
        }

        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            IncrementTotalOperPC();
            return Common.CreateNewStoreData(context, timeout);
        }

        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            IncrementTotalOperPC();
            DateTime dSetTime = DateTime.Now;

            if (_bIsDbNone == false)
            {
                #region Setting it in Db

                IncrementDbPC();
                using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                {
                    objDb.Add(id, ApplicationName, dSetTime, dSetTime.AddMinutes(timeout), dSetTime, 0, timeout, false, null, 1);
                }

                #endregion
            }

            #region Updating item in memcached

            IncrementMemcachedPC();
            var objHolder = new MemcachedHolder(null, false, dSetTime, 0, 1);

            _client.Store(StoreMode.Set, id, objHolder, TimeSpan.FromMinutes(timeout));

            #endregion
        }

        public override void Dispose()
        {
        }

        public override void EndRequest(HttpContext context)
        {
            // leave empty
        }

        public override SessionStateStoreData GetItem(
            HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            IncrementTotalOperPC();

            SessionStateStoreData objItem = null;

            IncrementMemcachedPC();
            var objHolder = _client.Get<MemcachedHolder>(id);
            DateTime dSetTime = DateTime.Now;

            #region Initialized

            lockAge = TimeSpan.Zero;
            lockId = null;
            locked = false;
            actions = 0;

            #endregion

            if (objHolder != null)
            {
                if (objHolder.Locked == false)
                {
                    #region

                    objHolder.LockId++;
                    objHolder.SetTime = dSetTime;
                    _client.Store(StoreMode.Set, id, objHolder, _objConfig.Timeout);

                    actions = (SessionStateActions) objHolder.ActionFlag;
                    lockId = objHolder.LockId;
                    lockAge = objHolder.LockAge;

                    if (_bIsDbNone == false) // Saving to Db
                    {
                        IncrementDbPC();
                        using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                        {
                            objDb.LockItem(id, ApplicationName, objHolder.LockId);
                        }
                    }

                    if (actions == SessionStateActions.InitializeItem)
                    {
                        objItem = Common.CreateNewStoreData(context, TimeoutInMinutes);
                    }
                    else
                    {
                        objItem = Common.Deserialize(context, objHolder.Content, TimeoutInMinutes);
                    }

                    #endregion
                }
                else
                {
                    lockAge = objHolder.LockAge;
                    locked = true;
                    lockId = objHolder.LockId;
                    actions = (SessionStateActions) objHolder.ActionFlag;
                }
            }
            else
            {
                if (_bIsDbNone == false)
                {
                    IncrementDbPC();
                    using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                    {
                        objItem = objDb.GetItem(
                            id, ApplicationName, TimeoutInMinutes, context, false, out locked, out lockAge, out lockId, out actions);
                    }
                }
            }

            return objItem;
        }

        public override SessionStateStoreData GetItemExclusive(
            HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            IncrementTotalOperPC();

            IncrementMemcachedPC();
            SessionStateStoreData objItem = null;
            var objHolder = _client.Get<MemcachedHolder>(id);
            DateTime dSetTime = DateTime.Now;

            #region Initialized

            lockAge = TimeSpan.Zero;
            lockId = null;
            locked = false;
            actions = 0;

            #endregion

            if (objHolder != null)
            {
                if (objHolder.Locked == false)
                {
                    #region

                    objHolder.LockId++;
                    objHolder.SetTime = dSetTime;

                    // Locking Item for memcached
                    objHolder.Locked = true;

                    IncrementMemcachedPC();
                    _client.Store(StoreMode.Set, id, objHolder, _objConfig.Timeout);

                    actions = (SessionStateActions) objHolder.ActionFlag;
                    lockId = objHolder.LockId;
                    lockAge = objHolder.LockAge;

                    if (_bIsDbNone == false) // Saving to Db
                    {
                        IncrementDbPC();
                        using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                        {
                            locked = objDb.LockItemWithoutLockId(id, ApplicationName);
                            objDb.LockItem(id, ApplicationName, objHolder.LockId);
                        }
                    }

                    if (actions == SessionStateActions.InitializeItem)
                    {
                        objItem = Common.CreateNewStoreData(context, TimeoutInMinutes);
                    }
                    else
                    {
                        objItem = Common.Deserialize(context, objHolder.Content, TimeoutInMinutes);
                    }

                    #endregion
                }
                else
                {
                    lockAge = objHolder.LockAge;
                    locked = true;
                    lockId = objHolder.LockId;
                    actions = (SessionStateActions) objHolder.ActionFlag;
                }
            }
            else
            {
                if (_bIsDbNone == false) // Saving to Db
                {
                    IncrementDbPC();
                    using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                    {
                        objItem = objDb.GetItem(
                            id, ApplicationName, TimeoutInMinutes, context, true, out locked, out lockAge, out lockId, out actions);
                    }
                }
            }

            return objItem;
        }

        public override void InitializeRequest(HttpContext context)
        {
            //leave empty
        }

        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            IncrementTotalOperPC();

            #region Updating item in memcached

            IncrementMemcachedPC();
            var objHolder = _client.Get<MemcachedHolder>(id);

            if (objHolder != null)
            {
                objHolder.Locked = false;
                objHolder.LockId = (int) lockId;
                IncrementMemcachedPC();
                _client.Store(StoreMode.Set, id, objHolder);
            }

            #endregion

            if (_bIsDbNone == false)
            {
                #region Updating Database

                IncrementDbPC();
                using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                {
                    objDb.ReleaseItem(id, ApplicationName, (int) lockId, TimeoutInMinutes);
                }

                #endregion
            }
        }

        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            IncrementTotalOperPC();

            #region Removing Item from memcached

            IncrementMemcachedPC();
            _client.Remove(id);

            #endregion

            if (_bIsDbNone == false) // Saving to Db
            {
                #region Removing item from db

                IncrementDbPC();
                using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                {
                    objDb.RemoveItem(id, ApplicationName);
                }

                #endregion
            }
        }

        public override void ResetItemTimeout(HttpContext context, string id)
        {
            IncrementTotalOperPC();

            #region Reset Item Timeout in Memcached

            IncrementMemcachedPC();
            object obj = _client.Get(id);

            if (obj != null)
            {
                IncrementMemcachedPC();
                _client.Store(StoreMode.Set, id, obj, new TimeSpan(0, TimeoutInMinutes, 0));
            }

            #endregion

            if (_bIsDbNone == false) // Saving to Db
            {
                #region Reset Item Timeout in db

                IncrementDbPC();
                using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                {
                    objDb.ResetItemTimeout(id, ApplicationName, TimeoutInMinutes);
                }

                #endregion
            }
        }

        public override void SetAndReleaseItemExclusive(
            HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            IncrementTotalOperPC();

            if (_bIsDbNone == false)
            {
                #region Db option

                using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                {
                    DateTime dSetTime = DateTime.Now;

                    byte[] objContent = Common.Serialize((SessionStateItemCollection) item.Items);

                    if (newItem)
                    {
                        IncrementDbPC();
                        objDb.Add(
                            id,
                            ApplicationName,
                            dSetTime,
                            dSetTime.AddMinutes(item.Timeout),
                            dSetTime,
                            0,
                            item.Timeout,
                            false,
                            objContent,
                            0);

                        // Setting it up in memcached                    
                        var objHolder = new MemcachedHolder(objContent, false, dSetTime, 0, 0);
                        IncrementMemcachedPC();
                        _client.Store(StoreMode.Set, id, objHolder, TimeSpan.FromMinutes(item.Timeout));
                    }
                    else
                    {
                        IncrementDbPC();
                        objDb.Update(id, ApplicationName, (int) lockId, dSetTime.AddMinutes(item.Timeout), objContent, false);

                        // Setting it up in memcached                    
                        var objHolder = new MemcachedHolder(objContent, false, dSetTime, 0, 0);

                        IncrementMemcachedPC();
                        _client.Store(StoreMode.Set, id, objHolder, TimeSpan.FromMinutes(item.Timeout));
                    }
                }

                #endregion
            }
            else // Just memcached version
            {
                DateTime dSetTime = DateTime.Now;

                byte[] objContent = Common.Serialize((SessionStateItemCollection) item.Items);

                if (newItem)
                {
                    // Setting it up in memcached                    
                    var objHolder = new MemcachedHolder(objContent, false, dSetTime, 0, 0);
                    IncrementMemcachedPC();
                    _client.Store(StoreMode.Set, id, objHolder, TimeSpan.FromMinutes(item.Timeout));
                }
                else
                {
                    // Setting it up in memcached                    
                    var objHolder = new MemcachedHolder(objContent, false, dSetTime, 0, 0);

                    IncrementMemcachedPC();
                    _client.Store(StoreMode.Set, id, objHolder, TimeSpan.FromMinutes(item.Timeout));
                }
            }
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            IncrementTotalOperPC();
            return false;
        }

        #region Performance Counter Methods

        private void CheckPerformanceCounterCategories()
        {
            if (!PerformanceCounterCategory.Exists(StrSessionProviderCat))
            {
                var counters = new CounterCreationDataCollection();

                var totalOps = new CounterCreationData
                                   {
                                       CounterName = StrTotalOperName,
                                       CounterHelp = "Total number of operations executed",
                                       CounterType = PerformanceCounterType.NumberOfItems32
                                   };
                counters.Add(totalOps);

                var opsPerSecond = new CounterCreationData
                                       {
                                           CounterName = StrOperPerSecName,
                                           CounterHelp = "Number of operations executed per second",
                                           CounterType = PerformanceCounterType.RateOfCountsPerSecond32
                                       };
                counters.Add(opsPerSecond);

                var memcachedOps = new CounterCreationData
                                       {
                                           CounterName = StrMemcachedOp,
                                           CounterHelp = "Number of Memcached operations execution",
                                           CounterType = PerformanceCounterType.NumberOfItems32
                                       };
                counters.Add(memcachedOps);

                var memcachedOpsPerSec = new CounterCreationData
                                             {
                                                 CounterName = StrMemcachedOpPerSec,
                                                 CounterHelp = "Number of Memcached operations per second",
                                                 CounterType = PerformanceCounterType.RateOfCountsPerSecond32
                                             };
                counters.Add(memcachedOpsPerSec);

                var dbOps = new CounterCreationData
                                {
                                    CounterName = StrDbOp,
                                    CounterHelp = "Number of database operations execution",
                                    CounterType = PerformanceCounterType.NumberOfItems32
                                };
                counters.Add(dbOps);

                var dbOpsPerSec = new CounterCreationData
                                      {
                                          CounterName = StrDbOpPerSec,
                                          CounterHelp = "Number of database operations execution",
                                          CounterType = PerformanceCounterType.RateOfCountsPerSecond32
                                      };
                counters.Add(dbOpsPerSec);

                // create new category with the counters above
                PerformanceCounterCategory.Create(
                    StrSessionProviderCat,
                    "Memcached Session Provider Performance Counter",
                    PerformanceCounterCategoryType.SingleInstance,
                    counters);
            }

            #region create counters to work with

            _objTotalOperations = new PerformanceCounter
                                      {
                                          CategoryName = StrSessionProviderCat,
                                          CounterName = StrTotalOperName,
                                          MachineName = ".",
                                          ReadOnly = false,
                                          RawValue = 0
                                      };

            _objOperationsPerSecond = new PerformanceCounter
                                          {
                                              CategoryName = StrSessionProviderCat,
                                              CounterName = StrOperPerSecName,
                                              MachineName = ".",
                                              ReadOnly = false,
                                              RawValue = 0
                                          };

            _objMemcachedOperations = new PerformanceCounter
                                          {
                                              CategoryName = StrSessionProviderCat,
                                              CounterName = StrMemcachedOp,
                                              MachineName = ".",
                                              ReadOnly = false,
                                              RawValue = 0
                                          };

            _objMemcachedOperPerSec = new PerformanceCounter
                                          {
                                              CategoryName = StrSessionProviderCat,
                                              CounterName = StrMemcachedOpPerSec,
                                              MachineName = ".",
                                              ReadOnly = false,
                                              RawValue = 0
                                          };

            _objDbOperations = new PerformanceCounter
                                   {
                                       CategoryName = StrSessionProviderCat,
                                       CounterName = StrDbOp,
                                       MachineName = ".",
                                       ReadOnly = false,
                                       RawValue = 0
                                   };

            _objDbOperationsPerSec = new PerformanceCounter
                                         {
                                             CategoryName = StrSessionProviderCat,
                                             CounterName = StrDbOpPerSec,
                                             MachineName = ".",
                                             ReadOnly = false,
                                             RawValue = 0
                                         };

            #endregion
        }

        private void IncrementTotalOperPC()
        {
            _objTotalOperations.Increment();
            _objOperationsPerSecond.Increment();
        }

        private void IncrementMemcachedPC()
        {
            _objMemcachedOperations.Increment();
            _objMemcachedOperPerSec.Increment();
        }

        private void IncrementDbPC()
        {
            _objDbOperations.Increment();
            _objDbOperationsPerSec.Increment();
        }

        #endregion
    }
}