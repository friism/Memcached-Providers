IF Object_Id('tblSessions') IS NOT NULL
	Drop Table [tblSessions]
GO

CREATE TABLE [tblSessions](
	[SessionId] [nvarchar](80) NOT NULL,
	[ApplicationName] [nvarchar](255) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Expires] [datetime] NOT NULL,
	[LockDate] [datetime] NOT NULL,
	[LockId] [int] NOT NULL,
	[Timeout] [int] NOT NULL,
	[Locked] [bit] NOT NULL,
	[SessionItems] [varbinary](max) NULL,
	[Flags] [int] NOT NULL,
 CONSTRAINT [PKSessions] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC,
	[ApplicationName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO 