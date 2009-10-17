IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'proc_CleanExpiredData')
	BEGIN
		DROP  Procedure  proc_CleanExpiredData
	END

GO

CREATE Procedure proc_CleanExpiredData

AS

DELETE FROM tblSessions WHERE Expires < GetDate();

GO


GRANT EXEC ON proc_CleanExpiredData TO PUBLIC

GO


