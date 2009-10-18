DROP PROCEDURE IF EXISTS proc_GetItem;

CREATE PROCEDURE proc_GetItem(_SessionId          varchar(80),
                              _ApplicationName    varchar(255))
   BEGIN
      SELECT Expires,
             SessionItems,
             LockId,
             LockDate,
             Flags,
             Timeout
        FROM tblSessions
       WHERE SessionId = _SessionId AND ApplicationName = _ApplicationName;
   END