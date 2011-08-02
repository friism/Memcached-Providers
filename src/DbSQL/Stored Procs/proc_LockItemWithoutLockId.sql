IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'proc_LockItemWithoutLockId')
	BEGIN
		DROP  Procedure  proc_LockItemWithoutLockId
	END

GO

CREATE Procedure proc_LockItemWithoutLockId
(
	@SessionId nvarchar(80),
	@ApplicationName nvarchar(255)	
)


AS
	
	UPDATE tblSessions SET
            Locked = 1, LockDate = GetDate()
            WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName
            AND Locked = 0 AND Expires > GetDate();

GO


GRANT EXEC ON proc_LockItemWithoutLockId TO PUBLIC

GO


