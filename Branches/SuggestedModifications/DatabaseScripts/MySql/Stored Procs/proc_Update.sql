DROP PROCEDURE IF EXISTS proc_Update;

CREATE PROCEDURE proc_Update
                    (
                        _Expires             DateTime,
                        _SessionItems        varbinary(64000),
                        _Locked              bit,
                        _SessionId           varchar(80),
                        _ApplicationName     varchar(255),
                        _LockId              int,
                    OUT RetVal              int
                    )
BEGIN

    DECLARE EXIT HANDLER FOR SQLEXCEPTION SET RetVal = -1;

    DELETE FROM tblSessions
          WHERE SessionId       = _SessionId
            AND ApplicationName = _ApplicationName
            AND Expires         < sysDate();

    UPDATE tblSessions
       SET Expires      = _Expires,
           SessionItems = _SessionItems,
           Locked       = _Locked
     WHERE SessionId       = _SessionId
       AND ApplicationName = _ApplicationName
       AND LockId          = _LockId;

    SET RetVal = 1;

END