DROP PROCEDURE IF EXISTS proc_CleanExpiredData;

CREATE PROCEDURE proc_CleanExpiredData ()
BEGIN

    DELETE FROM tblSessions WHERE Expires < sysDate();

END