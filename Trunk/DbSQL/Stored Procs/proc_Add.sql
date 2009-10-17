IF Object_ID('proc_Add') IS NOT NULL	
	Drop procedure [proc_Add]

GO

CREATE procedure [proc_Add]
( 
	@SessionId nvarchar(80),
	@ApplicationName nvarchar(255),
	@Created DateTime,
	@Expires DateTime,
	@LockDate Datetime,
	@LockId int,
	@Timeout int,
	@Locked bit,
	@SessionItems varbinary(max) = NULL,
	@Flags int
) 
AS 
	Begin Try
		-- Deleting any already expired data
		Delete from tblSessions where 
			SessionId = @SessionId and
			ApplicationName = @ApplicationName and
			Expires < GetDate();

		-- Inserting new session data
		Insert Into tblSessions(
			SessionId, ApplicationName, Created, Expires, LockDate, LockId, [Timeout],
			Locked, SessionItems, Flags)
		Values(@SessionId, @ApplicationName, @Created, @Expires, @LockDate, @LockId, @Timeout, 
			@Locked, @SessionItems, @Flags);
		return 1;
	End Try
	Begin Catch
		return -1;
	End Catch
			
GO