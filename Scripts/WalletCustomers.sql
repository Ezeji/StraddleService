CREATE TABLE [dbo].[WalletCustomers]
(
	[CustomerId] [UNIQUEIDENTIFIER] NOT NULL DEFAULT (newid()),
	[FirstName] [NVARCHAR](MAX) NOT NULL,
	[LastName] [NVARCHAR](MAX) NOT NULL,
	[MiddleName] [NVARCHAR](MAX) NULL,
	[PhoneNumber] [NVARCHAR](MAX) NOT NULL,
	[Email] [NVARCHAR](MAX) NULL,
	[CustomerType] [INT] NOT NULL DEFAULT ((1)), --1-Individual, 2-Corporate
	[Status] [INT] NOT NULL DEFAULT ((1)), --1-Pending, 2-Active
	[AddressLine] [NVARCHAR](MAX) NOT NULL,
	[City] [NVARCHAR](MAX) NOT NULL,
	[State] [NVARCHAR](MAX) NOT NULL,
	[Country] [NVARCHAR](MAX) NOT NULL,
	[IdentificationType] [INT] NOT NULL, -- 1:Passport, 2:WorkPermit, 3:ResidencePermit, 4:NationalIdentity
	[IdentificationNumber] [NVARCHAR](MAX) NOT NULL,
	[DateCreated] DATETIME NULL DEFAULT (getutcdate()), 
    [DateUpdated] DATETIME NULL DEFAULT (getutcdate()),
    CONSTRAINT [PK_WalletCustomers] PRIMARY KEY ([CustomerId])
)
