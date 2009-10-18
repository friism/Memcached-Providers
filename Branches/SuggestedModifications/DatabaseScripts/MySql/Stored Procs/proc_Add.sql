DROP PROCEDURE IF EXISTS proc_Add;

CREATE PROCEDURE proc_Add
                    (
                        _SessionId           varchar(80),
                        _ApplicationName     varchar(255),
                        _Created             dateTime,
                        _Expires             dateTime,
                        _LockDate            datetime,
                        _LockId              int,
                        _Timeout             int,
                        _Locked              bit,
                        _SessionItems        varbinary(64000),
                        _Flags               int,
                    OUT RetVal              int
                    )
BEGIN

    DECLARE EXIT HANDLER FOR SQLEXCEPTION SET RetVal = -1;

    DELETE FROM tblSessions
          WHERE SessionId       = _SessionId
            AND ApplicationName = _ApplicationName
            AND Expires         < sysDate();

    INSERT INTO tblSessions
            (   SessionId,
                ApplicationName,
                Created,
                Expires,
                LockDate,
                LockId,
                Timeout,
                Locked,
                SessionItems,
                Flags
            )
    VALUES (    _SessionId,
                _ApplicationName,
                _Created,
                _Expires,
                _LockDate,
                _LockId,
                _Timeout,
                _Locked,
                _SessionItems,
                _Flags
            );

    SET RetVal = 1;

END