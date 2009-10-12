IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'proc_ResetItemTimeout')
	BEGIN
		DROP  Procedure  proc_ResetItemTimeout
	END

GO

CREATE Procedure proc_ResetItemTimeout
(
	@SessionId nvarchar(80),
	@ApplicationName nvarchar(255),
	@AddMin int
)

AS

	UPDATE tblSessions SET Expires = DateAdd(minute,@AddMin, GetDate())
        WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName;
GO


GRANT EXEC ON proc_ResetItemTimeout TO PUBLIC

GO


