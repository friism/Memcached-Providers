using System;
using System.Collections.Generic;
using System.Text;
using System.Web.SessionState;
using System.Web;
using System.IO;
using MemcachedProviders.Session;
using System.Diagnostics;
using MemcachedProviders.Session.Db;

namespace MemcachedProviders.Session
{
    internal class Common
    {
        private const string  eventSource = "MemcachedSessionStateStore";
        private const string eventLog = "Application";
        private const string exceptionMessage =
          "An exception occurred. Please contact your administrator.";

        public static SessionStateStoreData CreateNewStoreData
            (HttpContext context, int timeout)
        {
            return new SessionStateStoreData(new SessionStateItemCollection(),
                SessionStateUtility.GetSessionStaticObjects(context), timeout);
        }

        public static byte[] Serialize(SessionStateItemCollection items)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);

            if (items != null)
                items.Serialize(writer);

            writer.Close();
            
            return ms.ToArray();            
        }

        public static SessionStateStoreData Deserialize(HttpContext context,
          byte[] serializedItems, int timeout)
        {            
            MemoryStream ms = new MemoryStream(serializedItems);

            SessionStateItemCollection sessionItems =
              new SessionStateItemCollection();

            if (ms.Length > 0)
            {  
                BinaryReader reader = new BinaryReader(ms);
                sessionItems = SessionStateItemCollection.Deserialize(reader);
            }

            return new SessionStateStoreData(sessionItems,
              SessionStateUtility.GetSessionStaticObjects(context),
              timeout);
        }

        public static void WriteToEventLog(Exception e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;

            string message =
              "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e.ToString();

            log.WriteEntry(message);
        }

        public static string[] GetServerArray(string strServerList)
        {
            return strServerList.Split(',');
        }

        public static class DbType
        {
            public static string SQLServer
            { get { return "SQLServer"; } }

            public static string None
            { get { return "None"; } }

            public static string MySQL
            { get { return "MySQL"; } }

            public static string Oracle
            { get { return "Oracle"; } }
        }        
    }

    public abstract class DbFactory
    {
        public static IDbOperations CreateDbOperations(string strDbType, string strConn)
        {
            if (strDbType == Common.DbType.SQLServer)
            {
                return new SQLDbOperations(strConn);
            }

            return null;
        }
    }

    //public static class ConfigurationUtil
    //{
    //    /// <summary>
    //    /// Helper method to validate the given configuration value and assign the given default
    //    /// if the configuration value is not valid.
    //    /// </summary>
    //    /// <param name="configValue">value to test.</param>
    //    /// <param name="defaultValue">value to assign if <c>configValue</c> is not valid.</param>
    //    /// <returns>A valid configuration value.</returns>
    //    public static string GetConfigValue(string configValue, string defaultValue)
    //    {
    //        return (string.IsNullOrEmpty(configValue) ? defaultValue : configValue);
    //    }
    //}
}
