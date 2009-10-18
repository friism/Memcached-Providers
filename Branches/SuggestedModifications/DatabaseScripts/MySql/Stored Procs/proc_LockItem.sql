DROP PROCEDURE IF EXISTS proc_LockItem;

CREATE PROCEDURE proc_LockItem(_SessionId          varchar(80),
                               _ApplicationName    varchar(255),
                               _LockId             int)
   BEGIN
      UPDATE tblSessions
         SET LockId = _LockId, Flags = 0
       WHERE SessionId = _SessionId AND ApplicationName = _ApplicationName;
   END