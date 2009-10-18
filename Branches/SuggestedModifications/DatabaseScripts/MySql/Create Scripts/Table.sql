CREATE TABLE IF NOT EXISTS `tblSessions`
    (
        `SessionId`          varchar(80)    NOT NULL,
        `ApplicationName`    varchar(255)   NOT NULL,
        `Created`            datetime       NOT NULL,
        `Expires`            datetime       NOT NULL,
        `LockDate`           datetime       NOT NULL,
        `LockId`             int            NOT NULL,
        `Timeout`            int            NOT NULL,
        `Locked`             bit            NOT NULL,
        `SessionItems`       varbinary(64000)   NULL,
        `Flags`              int            NOT NULL,
         PRIMARY KEY ( `SessionId`, `ApplicationName` )
    )
ENGINE = memory
DEFAULT CHARSET = utf8;