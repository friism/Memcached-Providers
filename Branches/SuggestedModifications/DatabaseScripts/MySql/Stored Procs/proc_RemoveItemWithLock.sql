DROP PROCEDURE IF EXISTS proc_RemoveItemWithLock;

CREATE PROCEDURE proc_RemoveItemWithLock(_SessionId          varchar(80),
                                         _ApplicationName    varchar(255),
                                         _LockId             int)
   BEGIN
      DELETE FROM tblSessions
            WHERE     SessionId = _SessionId
                  AND ApplicationName = _ApplicationName
                  AND LockId = _LockId;
   END