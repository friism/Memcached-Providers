IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'proc_ReleaseItem')
	BEGIN
		DROP  Procedure  proc_ReleaseItem
	END

GO

CREATE Procedure proc_ReleaseItem
(
	@SessionId nvarchar(80),
	@ApplicationName nvarchar(255),
	@AddMin int,
	@LockId int
)

AS

	UPDATE tblSessions SET Locked = 0, Expires = DateAdd(minute,@AddMin, GetDate())
        WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName AND LockId = @LockId;

GO


GRANT EXEC ON proc_ReleaseItem TO PUBLIC

GO


