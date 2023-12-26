CREATE TABLE [dbo].[Partners]
(
	[PartnerId] INT NOT NULL IDENTITY(1, 1)  PRIMARY KEY,
	[PartnerCode] NVARCHAR(20) NOT NULL UNIQUE,
	[PartnerName] NVARCHAR(MAX) NOT NULL,
    [PhoneNumber] NVARCHAR(MAX) NOT NULL,
    [ContactEmail] NVARCHAR(MAX) NOT NULL,
    [SupportEmail] NVARCHAR(MAX) NULL,
    [ContactName] NVARCHAR(MAX) NOT NULL,
    [Location] NVARCHAR(MAX) NOT NULL,
    [StateOfResidence] NVARCHAR(MAX) NULL,
    [Lga] NVARCHAR(MAX) NULL,
    [Area] NVARCHAR(MAX) NULL,
    [StreetName] NVARCHAR(MAX) NULL,
    [IsActive] BIT NOT NULL DEFAULT ((1)),
    [PartnerType] INT NOT NULL DEFAULT ((1)), -- 1-AgentPartner, 2-Company
    [LogoUrl] NVARCHAR(MAX) NULL,
    [DateCreated] DATETIME NULL DEFAULT (getutcdate()),
    [DateUpdated] DATETIME NULL DEFAULT (getutcdate()),
)
