using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using System.Web.SessionState;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Configuration;
using System.Diagnostics;
using MemcachedProviders.Session.Memcached;
using MemcachedProviders.Session.Db;
using MemcachedProviders.Session;
using MemcachedProviders.Common;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedProviders.Session
{
    public class SessionStateProvider : SessionStateStoreProviderBase
    {
        #region Membership Variables
        private string _strApplicationName;
        private string _strConn;
        private string _strDbType;
        private ConnectionStringSettings _objConnectionStringSettings;
        private SessionStateSection _objConfig = null;
        private bool _objWriteExceptionsToEventLog = false;
        private bool _bIsDbNone = false;

        // Memcached variables
        private MemcachedClient _client = MemcachedClientService.Instance.Client;
        private long _lTimeoutInMilliSec = 20000; // default Expire Time
        #endregion

        public bool WriteExceptionsToEventLog
        {
            get { return _objWriteExceptionsToEventLog; }
            set { _objWriteExceptionsToEventLog = value; }
        }

        public string ApplicationName
        {
            get { return this._strApplicationName; }
            set { this._strApplicationName = value; }
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "MemcachedSessionStateStore";

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

            ApplicationName =
              System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;

            //
            // Get <sessionState> configuration element.
            //

            Configuration cfg =
              WebConfigurationManager.OpenWebConfiguration(ApplicationName);
            _objConfig =
              (SessionStateSection)cfg.GetSection("system.web/sessionState");

            if (!string.IsNullOrEmpty(config["dbType"]))
            {
                if (config["dbType"].ToLower() == "none")
                {
                    this._strDbType = Common.DbType.None;
                    this._bIsDbNone = true;
                }
                else
                {
                    this._strDbType = Common.DbType.SQLServer;
                    this._bIsDbNone = false;
                    //
                    // Initialize connection string.
                    //
                    _objConnectionStringSettings =
                      ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

                    if (_objConnectionStringSettings == null ||
                      _objConnectionStringSettings.ConnectionString.Trim() == "")
                    {
                        throw new ProviderException("Connection string cannot be blank.");
                    }

                    _strConn = _objConnectionStringSettings.ConnectionString;
                }
            }
            //
            // Initialize Memcached Values
            //
            _lTimeoutInMilliSec = _objConfig.Timeout.Milliseconds;

            //
            // Initialize WriteExceptionsToEventLog
            //
            _objWriteExceptionsToEventLog = false;

            if (config["writeExceptionsToEventLog"] != null)
            {
                if (config["writeExceptionsToEventLog"].ToUpper() == "TRUE")
                    _objWriteExceptionsToEventLog = true;
            }
        }

        public override SessionStateStoreData CreateNewStoreData(System.Web.HttpContext context, int timeout)
        {
            return Common.CreateNewStoreData(context, timeout);
        }

        public override void CreateUninitializedItem(System.Web.HttpContext context, string id,
            int timeout)
        {
            DateTime dSetTime = DateTime.Now;

            if (this._bIsDbNone == false)
            {
                #region Setting it in Db
                using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                {
                    objDb.Add(id, ApplicationName, dSetTime,
                        dSetTime.AddMinutes((double)timeout),
                        dSetTime, 0, timeout, false, null, 1);
                }
                #endregion
            }

            #region Updating item in memcached

            MemcachedHolder objHolder = new MemcachedHolder(
                null, false, dSetTime, 0, 1);

            this._client.Store(StoreMode.Set, id, objHolder, new TimeSpan(0, timeout, 0));

            #endregion
        }

        public override void Dispose()
        {

        }

        public override void EndRequest(System.Web.HttpContext context)
        {
            // leave empty
        }

        public override SessionStateStoreData GetItem(System.Web.HttpContext context, string id,
            out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            SessionStateStoreData objItem = null;

            MemcachedHolder objHolder = this._client.Get<MemcachedHolder>(id);
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
                    this._client.Store(StoreMode.Set,
                        id, objHolder, new TimeSpan(0, 0, 0, 0, _objConfig.Timeout.Milliseconds));

                    actions = (SessionStateActions)objHolder.ActionFlag;
                    lockId = objHolder.LockId;
                    lockAge = objHolder.LockAge;

                    if (this._bIsDbNone == false) // Saving to Db
                    {
                        using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                        {
                            objDb.LockItem(id, ApplicationName, objHolder.LockId);
                        }
                    }

                    if (actions == SessionStateActions.InitializeItem)
                    {
                        objItem = Common.CreateNewStoreData(context, _objConfig.Timeout.Minutes);
                    }
                    else
                    {
                        objItem = Common.Deserialize(context, objHolder.Content, _objConfig.Timeout.Minutes);
                    }

                    return objItem;
                    #endregion
                }
                else
                {
                    lockAge = objHolder.LockAge;
                    locked = true;
                    lockId = objHolder.LockId;
                    actions = (SessionStateActions)objHolder.ActionFlag;

                    return objItem;
                }
            }
            else
            {
                if (this._bIsDbNone == false)
                {
                    using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                    {
                        return objDb.GetItem(id, ApplicationName, _objConfig.Timeout.Minutes,
                            context, false, out locked, out lockAge, out lockId, out actions);
                    }
                }
                else
                {
                    return objItem;
                }
            }
        }

        public override SessionStateStoreData GetItemExclusive(System.Web.HttpContext context, string id,
            out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            SessionStateStoreData objItem = null;
            MemcachedHolder objHolder = this._client.Get<MemcachedHolder>(id);
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
                    this._client.Store(StoreMode.Set, id, objHolder,
                        new TimeSpan(0, 0, 0, 0, _objConfig.Timeout.Milliseconds));

                    actions = (SessionStateActions)objHolder.ActionFlag;
                    lockId = objHolder.LockId;
                    lockAge = objHolder.LockAge;

                    if (this._bIsDbNone == false) // Saving to Db
                    {
                        using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                        {
                            locked = objDb.LockItemWithoutLockId(id, ApplicationName);
                            objDb.LockItem(id, ApplicationName, objHolder.LockId);
                        }
                    }

                    if (actions == SessionStateActions.InitializeItem)
                    {
                        objItem = Common.CreateNewStoreData(context, _objConfig.Timeout.Minutes);
                    }
                    else
                    {
                        objItem = Common.Deserialize(context, objHolder.Content, _objConfig.Timeout.Minutes);
                    }

                    return objItem;
                    #endregion
                }
                else
                {
                    lockAge = objHolder.LockAge;
                    locked = true;
                    lockId = objHolder.LockId;
                    actions = (SessionStateActions)objHolder.ActionFlag;

                    return objItem;
                }

            }
            else
            {
                if (this._bIsDbNone == false) // Saving to Db
                {
                    using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                    {
                        return objDb.GetItem(id, ApplicationName, _objConfig.Timeout.Minutes,
                            context, true, out locked, out lockAge, out lockId, out actions);
                    }
                }
                else
                {
                    return objItem;
                }
            }
        }

        public override void InitializeRequest(System.Web.HttpContext context)
        {
            //leave empty
        }

        public override void ReleaseItemExclusive(System.Web.HttpContext context, string id, object lockId)
        {
            #region Updating item in memcached
            MemcachedHolder objHolder = this._client.Get<MemcachedHolder>(id);

            if (objHolder != null)
            {
                objHolder.Locked = false;
                objHolder.LockId = (int)lockId;
                this._client.Store(StoreMode.Set, id, objHolder);
            }
            #endregion

            if (this._bIsDbNone == false)
            {
                #region Updating Database
                using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                {
                    objDb.ReleaseItem(id, ApplicationName, (int)lockId, _objConfig.Timeout.Minutes);
                }
                #endregion
            }
        }

        public override void RemoveItem(System.Web.HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            #region Removing Item from memcached
            this._client.Remove(id);
            #endregion

            if (this._bIsDbNone == false) // Saving to Db
            {
                #region Removing item from db
                using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                {
                    objDb.RemoveItem(id, ApplicationName);
                }
                #endregion
            }
        }

        public override void ResetItemTimeout(System.Web.HttpContext context, string id)
        {
            #region Reset Item Timeout in Memcached
            object obj = this._client.Get(id);

            if (obj != null)
            {
                this._client.Store(StoreMode.Set, id, obj);
            }
            #endregion

            if (this._bIsDbNone == false) // Saving to Db
            {
                #region Reset Item Timeout in db
                using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                {
                    objDb.ResetItemTimeout(id, ApplicationName, _objConfig.Timeout.Minutes);
                }
                #endregion
            }
        }

        public override void SetAndReleaseItemExclusive(System.Web.HttpContext context,
            string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            if (this._bIsDbNone == false)
            {
                #region Db option
                using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                {
                    byte[] objContent = null;
                    DateTime dSetTime = DateTime.Now;

                    objContent = Common.Serialize((SessionStateItemCollection)item.Items);

                    if (newItem == true)
                    {
                        objDb.Add(id, ApplicationName, dSetTime,
                            dSetTime.AddMinutes((Double)item.Timeout), dSetTime,
                            0, item.Timeout, false,
                            objContent, 0);

                        // Setting it up in memcached                    
                        MemcachedHolder objHolder = new MemcachedHolder(
                             objContent, false, dSetTime, 0, 0);
                        this._client.Store(StoreMode.Set,
                            id, objHolder, new TimeSpan(0, item.Timeout, 0));
                    }
                    else
                    {
                        objDb.Update(id, ApplicationName, (int)lockId,
                            dSetTime.AddMinutes((Double)item.Timeout),
                            objContent, false);

                        // Setting it up in memcached                    
                        MemcachedHolder objHolder = new MemcachedHolder(
                            objContent, false, dSetTime, 0, 0);

                        this._client.Store(StoreMode.Set,
                            id, objHolder, new TimeSpan(0, item.Timeout, 0));
                    }
                }
                #endregion
            }
            else // Just memcached version
            {
                byte[] objContent = null;
                DateTime dSetTime = DateTime.Now;

                objContent = Common.Serialize((SessionStateItemCollection)item.Items);

                if (newItem == true)
                {
                    /*objDb.Add(id, ApplicationName, dSetTime,
                        dSetTime.AddMinutes((Double)item.Timeout), dSetTime,
                        0, item.Timeout, false,
                        objContent, 0);*/

                    // Setting it up in memcached                    
                    MemcachedHolder objHolder = new MemcachedHolder(
                         objContent, false, dSetTime, 0, 0);
                    this._client.Store(StoreMode.Set,
                        id, objHolder, new TimeSpan(0, item.Timeout, 0));
                }
                else
                {
                    /*
                    objDb.Update(id, ApplicationName, (int)lockId,
                        dSetTime.AddMinutes((Double)item.Timeout),
                        objContent, false);*/

                    // Setting it up in memcached                    
                    MemcachedHolder objHolder = new MemcachedHolder(
                        objContent, false, dSetTime, 0, 0);

                    this._client.Store(StoreMode.Set,
                        id, objHolder, new TimeSpan(0, item.Timeout, 0));
                } 
            }
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            return false;
        }        
    }
}
