using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Web.SessionState;
using System.Web;
using System.Data.SqlTypes;

namespace SessionState.Db
{
    internal class SQLDbOperations : IDbOperations
    {
        private string _strConn;
        private SqlConnection _objConn;

        public SQLDbOperations(string strConn)
        {
            this._strConn = strConn;
            this._objConn = new SqlConnection(strConn);
        }

        #region IDbOperations Members

        public bool Add(string strSessionId, string strApplicationName, DateTime dCreated, DateTime dExpires, 
            DateTime dLockDate, int iLockId, int iTimeout, bool bLocked, byte[] objSessionItems, int iFlags)
        {
            SqlConnection objConn = this.GetConnection();
            SqlCommand objComm = this.GetCommand("proc_Add", objConn);

            try
            {
                objComm.Parameters.Add(this.GetParameter("@SessionId", strSessionId));
                objComm.Parameters.Add(this.GetParameter("@ApplicationName", strApplicationName));
                objComm.Parameters.Add(this.GetParameter("@Created", dCreated));
                objComm.Parameters.Add(this.GetParameter("@Expires", dExpires));
                objComm.Parameters.Add(this.GetParameter("@LockDate", dLockDate));
                objComm.Parameters.Add(this.GetParameter("@LockId", iLockId));
                objComm.Parameters.Add(this.GetParameter("@Timeout", iTimeout));
                objComm.Parameters.Add(this.GetParameter("@Locked", bLocked,DbType.Boolean,false));
                objComm.Parameters.Add(this.GetParameter("@SessionItems", objSessionItems, DbType.Binary,true));
                objComm.Parameters.Add(this.GetParameter("@Flags", iFlags));
                SqlParameter output = this.GetParameter("@RETURN_VALUE", ParameterDirection.ReturnValue);

                objComm.Parameters.Add(output);

                objComm.ExecuteNonQuery();

                if (int.Parse(output.Value.ToString()) > 0)
                    return true;
                else
                    return false;
            }
            finally 
            {
                
            }
        }

        public bool Update(string strSessionId, string strApplicationName, int iLockId, DateTime dExpires, byte[] objSessionItems, bool bLocked)
        {
            SqlConnection objConn = this.GetConnection();
            SqlCommand objComm = this.GetCommand("proc_Update", objConn);

            try
            {
                objComm.Parameters.Add(this.GetParameter("@Expires", dExpires));
                objComm.Parameters.Add(this.GetParameter("@SessionItems", objSessionItems));
                objComm.Parameters.Add(this.GetParameter("@Locked", bLocked));
                objComm.Parameters.Add(this.GetParameter("@SessionId", strSessionId));
                objComm.Parameters.Add(this.GetParameter("@ApplicationName", strApplicationName));
                objComm.Parameters.Add(this.GetParameter("@LockId", iLockId));
                
                SqlParameter output = this.GetParameter("@RETURN_VALUE", ParameterDirection.ReturnValue);

                objComm.Parameters.Add(output);

                SqlDataReader objReader = objComm.ExecuteReader();

                if (int.Parse(output.Value.ToString()) > 0)
                    return true;
                else
                    return false; 
            }
            finally
            {
                
            }
        }

        public SessionStateStoreData GetItem(string strSessionId, string strApplicationName, int iTime,
            HttpContext context,
            bool lockRecord,
            out bool locked,
            out TimeSpan lockAge,
            out object lockId,
            out SessionStateActions actionFlags)
        {
            SqlConnection objConn = this.GetConnection();
            SqlCommand objComm = null;

            SessionStateStoreData objItem = null;
            lockAge = TimeSpan.Zero;
            lockId = null;
            locked = false;
            actionFlags = 0;

            DateTime expires;
            // String to hold serialized SessionStateItemCollection.
            byte[] objSerializedItem = null;
            // True if a record is found in the database.
            bool foundRecord = false;
            // True if the returned session item is expired and needs to be deleted.
            bool deleteData = false;
            // Timeout value from the data store.
            int timeout = 0;

            if (lockRecord == true)
            {
                locked = this.LockItemWithoutLockId(strSessionId, strApplicationName);
            }

            // Retrieving current session item
            objComm = this.GetCommand("proc_GetItem", objConn);

            objComm.Parameters.Add(this.GetParameter("@SessionId", strSessionId));
            objComm.Parameters.Add(this.GetParameter("@ApplicationName", strApplicationName));

            SqlDataReader reader = objComm.ExecuteReader(CommandBehavior.SingleRow);

            while (reader.Read())
            {
                expires = reader.GetDateTime(0);

                if (expires < DateTime.Now)
                {
                    // The record was expired. Mark it as not locked.
                    locked = false;
                    // The session was expired. Mark the data for deletion.
                    deleteData = true;
                }
                else
                {
                    foundRecord = true;
                }
                
                long lCount;

                try
                {
                    lCount = reader.GetBytes(1, 0, null, 0, 0);
                }
                catch (SqlNullValueException)
                {
                    lCount = 0;
                }

                if (lCount > 0)
                {
                    objSerializedItem = new byte[lCount];
                    reader.GetBytes(1, 0, objSerializedItem, 0, objSerializedItem.Length);                                      
                }
                else
                {
                    objSerializedItem = null;
                }
                

                lockId = (object)reader.GetInt32(2);
                lockAge = DateTime.Now.Subtract(reader.GetDateTime(3));
                actionFlags = (SessionStateActions)reader.GetInt32(4);
                timeout = reader.GetInt32(5);
            }
            reader.Close();

            if (deleteData)
            {
                this.RemoveItem(strSessionId, strApplicationName);
            }

            // The record was not found. Ensure that locked is false.
            if (!foundRecord)
                locked = false;

            // If the record was found and you obtained a lock, then set 
            // the lockId, clear the actionFlags,
            // and create the SessionStateStoreItem to return.
            if (foundRecord && !locked)
            {
                lockId = (int)lockId + 1;

                this.LockItem(strSessionId, strApplicationName, (int)lockId);

                // If the actionFlags parameter is not InitializeItem, 
                // deserialize the stored SessionStateItemCollection.
                if (actionFlags == SessionStateActions.InitializeItem)
                {
                    objItem = Common.CreateNewStoreData(context, iTime);
                }
                else
                {
                    objItem = Common.Deserialize(context, objSerializedItem, timeout);
                }
            }

            //returning item
            return objItem;
        }

        public bool ReleaseItem(string strSessionId, string strApplicationName, int iLockId, int iTime)
        {
            SqlConnection objConn = this.GetConnection();
            SqlCommand objComm = this.GetCommand("proc_ReleaseItem", objConn);

            try
            {
                objComm.Parameters.Add(this.GetParameter("@SessionId", strSessionId));
                objComm.Parameters.Add(this.GetParameter("@ApplicationName", strApplicationName));
                objComm.Parameters.Add(this.GetParameter("@AddMin", iTime));
                objComm.Parameters.Add(this.GetParameter("@LockId", iLockId));

                return (objComm.ExecuteNonQuery() > 0);
            }
            finally
            {

            }
        }

        public bool LockItem(string strSessionId, string strApplicationName, int iLockId)
        {
            SqlConnection objConn = this.GetConnection();
            SqlCommand objComm = this.GetCommand("proc_LockItem", objConn);

            try
            {
                objComm.Parameters.Add(this.GetParameter("@SessionId", strSessionId));
                objComm.Parameters.Add(this.GetParameter("@ApplicationName", strApplicationName));
                objComm.Parameters.Add(this.GetParameter("@LockId", iLockId));
                return (objComm.ExecuteNonQuery() > 0);
            }
            finally
            {

            }
        }

        public bool LockItemWithoutLockId(string strSessionId, string strApplicationName)
        {
            SqlConnection objConn = this.GetConnection();
            SqlCommand objComm = this.GetCommand("proc_LockItemWithoutLockId", objConn);

            try
            {
                objComm.Parameters.Add(this.GetParameter("@SessionId", strSessionId));
                objComm.Parameters.Add(this.GetParameter("@ApplicationName", strApplicationName));
                
                return (objComm.ExecuteNonQuery() > 0);
            }
            finally
            {

            }
        }

        public bool RemoveItem(string strSessionId, string strApplicationName, int iLockId)
        {
            SqlConnection objConn = this.GetConnection();
            SqlCommand objComm = this.GetCommand("proc_RemoveItemWithLock", objConn);

            try
            {
                objComm.Parameters.Add(this.GetParameter("@SessionId", strSessionId));
                objComm.Parameters.Add(this.GetParameter("@ApplicationName", strApplicationName));
                objComm.Parameters.Add(this.GetParameter("@LockId", iLockId));
                return (objComm.ExecuteNonQuery() > 0);
            }
            finally
            {
                
            }
        }

        public bool RemoveItem(string strSessionId, string strApplicationName)
        {
            SqlConnection objConn = this.GetConnection();
            SqlCommand objComm = this.GetCommand("proc_RemoveItem", objConn);

            try
            {
                objComm.Parameters.Add(this.GetParameter("@SessionId", strSessionId));
                objComm.Parameters.Add(this.GetParameter("@ApplicationName", strApplicationName));
                return (objComm.ExecuteNonQuery() > 0); 
            }
            finally
            {
                
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSessionId"></param>
        /// <param name="strApplicationName"></param>
        /// <param name="iTime">Time in Minutes</param>
        /// <returns></returns>
        public bool ResetItemTimeout(string strSessionId, string strApplicationName, int iTime)
        {
            SqlConnection objConn = this.GetConnection();
            SqlCommand objComm = this.GetCommand("proc_ResetItemTimeout", objConn);

            try
            {
                objComm.Parameters.Add(this.GetParameter("@SessionId", strSessionId));
                objComm.Parameters.Add(this.GetParameter("@ApplicationName", strApplicationName));
                objComm.Parameters.Add(this.GetParameter("@AddMin", iTime));

                return (objComm.ExecuteNonQuery() > 0); 
            }
            finally
            {
                
            }
            
        }
        #endregion

        #region Helper functions

        private SqlConnection GetConnection()
        {
            return this._objConn;
        }

        private SqlCommand GetCommand(string strComm, SqlConnection objConn)
        {
            if (objConn.State != ConnectionState.Open)
            {
                objConn.Open();
            }

            SqlCommand objComm = new SqlCommand(strComm,objConn);
            objComm.CommandType = CommandType.StoredProcedure;

            return objComm;
        }

        private SqlParameter GetParameter(string strName, object objValue)
        {
            SqlParameter obj = new SqlParameter(strName, objValue);
            obj.Direction = ParameterDirection.Input;
            return obj;
        }

        private SqlParameter GetParameter(string strName, object objValue, DbType type, bool IsNullable)
        {
            SqlParameter obj = new SqlParameter();
            obj.ParameterName = strName;
            obj.DbType = type;
            obj.Direction = ParameterDirection.Input;
            obj.IsNullable = IsNullable;
            obj.Value = objValue;
            
            return obj;
        }

        private SqlParameter GetParameter(string strName, ParameterDirection direction)
        {
            SqlParameter obj = new SqlParameter();
            obj.ParameterName = strName;
            obj.Direction = direction;
            return obj;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this._objConn.Close();
        }

        #endregion
    }
}
