DROP PROCEDURE IF EXISTS proc_LockItemWithoutLockId;

CREATE PROCEDURE proc_LockItemWithoutLockId(_SessionId          varchar(80),
                                            _ApplicationName    varchar(255))
   BEGIN
      UPDATE tblSessions
         SET Locked = 1, LockDate = sysDate()
       WHERE     SessionId = _SessionId
             AND ApplicationName = _ApplicationName
             AND Locked = 0
             AND Expires > sysDate();
   END