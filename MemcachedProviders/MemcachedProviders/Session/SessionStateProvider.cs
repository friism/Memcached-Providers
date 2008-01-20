using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using System.Web.SessionState;
using System.IO;
using System.Web;
using SessionState;
using System.Web.Configuration;
using System.Configuration;
using Memcached.ClientLibrary;
using System.Diagnostics;
using MemcachedProviders.Session.Memcached;
using MemcachedProviders.Session.Db;
using MemcachedProviders.Session;
using MemcachedProviders.Common;

namespace SessionState
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
        
        // Memcached variables
        private SockIOPool _objPool;
        private const string _privatePoolName = "__SessionStatePool__"; // Pool Name
        private long _lTimeoutInMilliSec = 20000; // default Expire Time
        private string _strServerList; // Server list
        private int _iSocketConnectTimeout = 1000;
        private int _iSocketTimeout = 1000;
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

            this._strDbType = Common.DbType.SQLServer;

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "MemcachedSessionStateStore";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Memcached Session State Store provider 1.0");
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

            //
            // Initialize Memcached Values
            //
            _lTimeoutInMilliSec = _objConfig.Timeout.Milliseconds;
                
            _iSocketConnectTimeout =
                Convert.ToInt32(ConfigurationUtil.GetConfigValue(config["socketConnectTimeout"], "1000"));
            _iSocketTimeout =
                Convert.ToInt32(ConfigurationUtil.GetConfigValue(config["socketTimeout"], "1000"));

            string strServers = config["servers"];

            if (string.IsNullOrEmpty(strServers))
            {
                throw new ProviderException("No Cache Server");
            }

            _strServerList = strServers;

            InitializePool(Common.GetServerArray(_strServerList));            

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
            
            #region Setting it in Db
            using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
            {
                objDb.Add(id, ApplicationName, dSetTime,
                    dSetTime.AddMinutes((double)timeout),
                    dSetTime, 0, timeout, false, null, 1);
            }
            #endregion

            #region Updating item in memcached
            MemcachedOperations objMemOper = this.GetMemClient();
            MemcachedHolder objHolder = new MemcachedHolder(
                null,false,dSetTime, 0, 1);
                        
            objMemOper.Add(id, objHolder,this.GetMilliSecOfMinutes(timeout));
            
            #endregion
        }

        public override void Dispose()
        {
            ShutdownPool();
        }

        public override void EndRequest(System.Web.HttpContext context)
        {
            // leave empty
        }

        public override SessionStateStoreData GetItem(System.Web.HttpContext context, string id, 
            out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            MemcachedOperations objMemOper = this.GetMemClient();
            SessionStateStoreData objItem = null;

            MemcachedHolder objHolder = objMemOper.Get(id) as MemcachedHolder;
            DateTime dSetTime = DateTime.Now;

            if (objHolder != null)
            {
                #region Initialized

                lockAge = TimeSpan.Zero;
                lockId = null;
                locked = false;
                actions = 0;
                #endregion

                if (objHolder.Locked == false)
                {
                    #region
                    objHolder.LockId++;
                    objHolder.SetTime = dSetTime;
                    objMemOper.Add(id, objHolder, _objConfig.Timeout.Milliseconds);

                    actions = (SessionStateActions)objHolder.ActionFlag;
                    lockId = objHolder.LockId;
                    lockAge = objHolder.LockAge;

                    using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                    {                        
                        objDb.LockItem(id, ApplicationName, objHolder.LockId);
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
                using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                {
                    return objDb.GetItem(id, ApplicationName, _objConfig.Timeout.Minutes,
                        context, false, out locked, out lockAge, out lockId, out actions);
                }
            }
        }

        public override SessionStateStoreData GetItemExclusive(System.Web.HttpContext context, string id, 
            out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            MemcachedOperations objMemOper = this.GetMemClient();
            SessionStateStoreData objItem = null;

            MemcachedHolder objHolder = objMemOper.Get(id) as MemcachedHolder;
            DateTime dSetTime = DateTime.Now;

            if (objHolder != null)
            {
                #region Initialized
                
                lockAge = TimeSpan.Zero;
                lockId = null;
                locked = false;
                actions = 0;
                #endregion

                if (objHolder.Locked == false)
                {
                    #region
                    objHolder.LockId++;
                    objHolder.SetTime = dSetTime;
                    objMemOper.Add(id, objHolder, _objConfig.Timeout.Milliseconds);

                    actions = (SessionStateActions)objHolder.ActionFlag;
                    lockId = objHolder.LockId;
                    lockAge = objHolder.LockAge;

                    using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                    {
                        locked = objDb.LockItemWithoutLockId(id, ApplicationName);
                        objDb.LockItem(id, ApplicationName, objHolder.LockId);
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
                using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
                {
                    return objDb.GetItem(id, ApplicationName, _objConfig.Timeout.Minutes,
                        context, true, out locked, out lockAge, out lockId, out actions);
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
            MemcachedOperations objMemOper = this.GetMemClient();
            MemcachedHolder objHolder = objMemOper.Get(id) as MemcachedHolder;

            if (objHolder != null)
            {
                objHolder.Locked = false;
                objHolder.LockId = (int)lockId;
                objMemOper.Add(id, objHolder);
            }
            #endregion

            #region Updating Database
            using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
            {
                objDb.ReleaseItem(id, ApplicationName, (int)lockId, _objConfig.Timeout.Minutes);
            }
            #endregion
        }

        public override void RemoveItem(System.Web.HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            #region Removing Item from memcached
            MemcachedOperations objMemOper = this.GetMemClient();
            objMemOper.Remove(id);
            #endregion

            #region Removing item from db
            using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
            {
                objDb.RemoveItem(id, ApplicationName);
            }
            #endregion
        }

        public override void ResetItemTimeout(System.Web.HttpContext context, string id)
        {
            #region Reset Item Timeout in Memcached
            MemcachedOperations objMemOper = this.GetMemClient();
            // Reseting the time in memcached
            object obj = objMemOper.Get(id);

            if (obj != null)
                objMemOper.Add(id, obj);
            #endregion

            #region Reset Item Timeout in db
            using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
            {   
                objDb.ResetItemTimeout(id, ApplicationName, _objConfig.Timeout.Minutes);
            }
            #endregion
        }

        public override void SetAndReleaseItemExclusive(System.Web.HttpContext context, 
            string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            using (IDbOperations objDb = DbFactory.CreateDbOperations(_strDbType, _strConn))
            {
                MemcachedOperations objMemOper = this.GetMemClient();
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
                    objMemOper.Add(id, objHolder,
                        this.GetMilliSecOfMinutes(item.Timeout));
                    
                }
                else
                {
                    objDb.Update(id, ApplicationName, (int)lockId,
                        dSetTime.AddMinutes((Double)item.Timeout),
                        objContent, false);

                    // Setting it up in memcached                    
                    MemcachedHolder objHolder = new MemcachedHolder(
                        objContent, false, dSetTime, 0, 0);
                    objMemOper.Add(id, objHolder,
                        this.GetMilliSecOfMinutes(item.Timeout));
                    
                }
            }
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            return false;
        }

        #region Memcached Helper Method
        private void InitializePool(string[] strServerList)
        {
            Debug.WriteLine("#-----InitializePool called----#");
            _objPool = SockIOPool.GetInstance(_privatePoolName);
            _objPool.SetServers(strServerList);
            _objPool.SocketConnectTimeout = _iSocketConnectTimeout;
            _objPool.SocketTimeout = _iSocketTimeout;
            _objPool.Initialize();
            Debug.WriteLine(string.Format("--------Is Pool Initialized: {0} ---------", _objPool.Initialized));
        }

        private void ShutdownPool()
        {
            _objPool.Shutdown();
            Debug.WriteLine("#-----Pool Shutdown----#");
        }

        private MemcachedOperations GetMemClient()
        {
            return new MemcachedOperations(_privatePoolName, _objPool,
                _lTimeoutInMilliSec, ApplicationName);
        }

        private long GetMilliSecOfMinutes(int iMin)
        {
            return (long)(iMin*60*1000);
        }
        #endregion
    }
}
