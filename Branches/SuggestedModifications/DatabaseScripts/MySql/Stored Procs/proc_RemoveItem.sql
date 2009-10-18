DROP PROCEDURE IF EXISTS proc_RemoveItem;

CREATE PROCEDURE proc_RemoveItem(_SessionId          varchar(80),
                                 _ApplicationName    varchar(255))
   BEGIN
      DELETE FROM tblSessions
            WHERE SessionId = _SessionId
                  AND ApplicationName = _ApplicationName;
   END