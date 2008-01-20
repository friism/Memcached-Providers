using System;
using System.Collections.Generic;
using System.Text;
using System.Web.SessionState;
using System.Web;

namespace MemcachedProviders.Session.Db
{
    public interface IDbOperations : IDisposable
    {
        bool Add(string strSessionId, string strApplicationName,
                DateTime dCreated, DateTime dExpires, DateTime dLockDate,
                int iLockId, int iTimeout, bool bLocked, 
                byte[] objSessionItems, int bFlags);

        bool Update(string strSessionId, string strApplicationName,int iLockId,
                DateTime dExpires, byte[] objSessionItems, bool bLocked);

        SessionStateStoreData GetItem(string strSessionId, string strApplicationName, int iTime,
            HttpContext context,
            bool lockRecord,
            out bool locked,
            out TimeSpan lockAge,
            out object lockId,
            out SessionStateActions actionFlags);

        bool ReleaseItem(string strSessionId, string strApplicationName, int iLockId, int iTime);
        
        bool LockItem(string strSessionId, string strApplicationName, int iLockId);

        bool LockItemWithoutLockId(string strSessionId, string strApplicationName);

        bool RemoveItem(string strSessionId, string strApplicationName, int iLockId);

        bool RemoveItem(string strSessionId, string strApplicationName);

        bool ResetItemTimeout(string strSessionId, string strApplicationName,
            int iTime);
    }
}
