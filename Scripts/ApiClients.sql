CREATE TABLE [dbo].[ApiClients]
(
	[ApiClientId] [UNIQUEIDENTIFIER] NOT NULL DEFAULT (newid()),
	[ClientId] [NVARCHAR](MAX) NOT NULL,
	[ClientSecret] [NVARCHAR](MAX) NOT NULL,
	[ClientCode] [NVARCHAR](MAX) NOT NULL,
	[ClientName] [NVARCHAR](MAX) NOT NULL,
	[IsEnabled] BIT NOT NULL DEFAULT ((1)),
	[DateCreated] DATETIME NULL DEFAULT (getutcdate()), 
    [DateUpdated] DATETIME NULL DEFAULT (getutcdate()),
    CONSTRAINT [PK_ApiClients] PRIMARY KEY ([ApiClientId])
)
