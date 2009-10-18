create table if not exists `tblSessions`
    (
        `SessionId`          varchar(80)    not null,
        `ApplicationName`    varchar(255)   not null,
        `Created`            datetime       not null,
        `Expires`            datetime       not null,
        `LockDate`           datetime       not null,
        `LockId`             int            not null,
        `Timeout`            int            not null,
        `Locked`             bit            not null,
        `SessionItems`       varbinary(64000)   null,
        `Flags`              int            not null,
         primary key ( `SessionId`, `ApplicationName` )
    ) 
engine = memory
default charset = utf8;