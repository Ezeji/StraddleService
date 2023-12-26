CREATE TABLE [dbo].[WalletAccounts]
(
	[AccountId] UNIQUEIDENTIFIER NOT NULL DEFAULT (newid()),
	[CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [BankName] NVARCHAR(MAX) NOT NULL, 
    [BankCode] NVARCHAR(MAX) NOT NULL,
    [BankAccountNumber] NVARCHAR(MAX) NOT NULL, 
    [BankAccountName] NVARCHAR(MAX) NOT NULL,
    [BankAccountType] [int] NOT NULL DEFAULT ((1)), --1-Savings, 2-Current, 3-Checking
    [BankAccountCurrency] [int] NOT NULL DEFAULT ((1)), --1-USD, 2-EUR
    [WalletBalance] DECIMAL(19, 2) NOT NULL, 
    [LedgerBalance] DECIMAL(19, 2) NOT NULL, 
    [TotalCredits] DECIMAL(19, 2) NOT NULL, 
    [TotalLedgerCredits] DECIMAL(19, 2) NOT NULL, 
    [TotalDebits] DECIMAL(19, 2) NOT NULL, 
    [TotalLedgerDebits] DECIMAL(19, 2) NOT NULL, 
    [DateCreated] DATETIME NULL DEFAULT (getutcdate()), 
    [DateUpdated] DATETIME NULL DEFAULT (getutcdate()),
    [BankAccountTierLevel] [int] NOT NULL DEFAULT ((1)), --1-Primary, 2-Secondary, 3-Tertiary  
    [Discount] DECIMAL(19, 2) NULL DEFAULT ((0.00)),
    [LienAmount] DECIMAL(19, 2) NOT NULL, 
    [AccountStatus] INT NOT NULL DEFAULT ((1)), -- 1: Active, 2 : InActive, 3: Blocked
    CONSTRAINT [PK_WalletAccounts] PRIMARY KEY ([AccountId]),
    FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[WalletCustomers]([CustomerId])
)
