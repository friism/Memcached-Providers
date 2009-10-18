DROP PROCEDURE IF EXISTS proc_ResetItemTimeout;

CREATE PROCEDURE proc_ResetItemTimeout(_SessionId          varchar(80),
                                       _ApplicationName    varchar(255),
                                       _AddMin             int)
   BEGIN
      UPDATE tblSessions
         SET Expires = date_add(sysDate(), INTERVAL _AddMin MINUTE)
       WHERE SessionId = _SessionId AND ApplicationName = _ApplicationName;
   END