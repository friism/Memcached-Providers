DROP PROCEDURE IF EXISTS proc_ReleaseItem;

CREATE PROCEDURE proc_ReleaseItem(_SessionId          varchar(80),
                                  _ApplicationName    varchar(255),
                                  _AddMin             int,
                                  _LockId             int)
   BEGIN
      UPDATE tblSessions
         SET Locked = 0,
             Expires = date_add(sysDate(), INTERVAL _AddMin MINUTE)
       WHERE     SessionId = _SessionId
             AND ApplicationName = _ApplicationName
             AND LockId = _LockId;
   END;