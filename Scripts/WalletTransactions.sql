CREATE TABLE [dbo].[WalletTransactions]
(
    [TransactionId] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[SourceAccountId] [uniqueidentifier] NULL, --sender's account identifier
	[DestinationAccountId] [uniqueidentifier] NULL, --receiver's account identifier
	[TransactionRequest] [nvarchar](MAX) NULL, --transaction request to external provider in json string
	[TransactionResponse] [nvarchar](MAX) NULL, --transaction response from external provider in json string
	[TransactionStatus] [int] NOT NULL DEFAULT ((1)), --1-Pending, 2-Cancelled, 3-Refunded, 4-Processed
	[TransactionType] [int] NOT NULL DEFAULT ((1)), --1-Intra, 2-Inter, 3-Own
	[TransactionAmount] [decimal](19, 2) NOT NULL DEFAULT ((0.00)),
	[TransactionFee] [decimal](19, 2) NULL DEFAULT ((0.00)),
	[GrossTransactionAmount] [decimal](19, 2) NULL DEFAULT ((0.00)), --addition of transaction amount and transaction fee
	[ExchangeRate] [decimal](19, 2) NULL DEFAULT ((0.00)),
	[ConvertedTransactionAmount] [decimal](19, 2) NULL DEFAULT ((0.00)),
	[TransactionReference] [nvarchar](MAX) NOT NULL, --unique transaction identifier
	[TransactionDetails] [nvarchar](MAX) NOT NULL, --transaction details from create transaction dto in json string
	[PaymentReason] [nvarchar](MAX) NULL,
	[IsAmountLiened] BIT NOT NULL DEFAULT((0)), --0-False, 1-True
	[DateCreated] datetime NULL DEFAULT (getutcdate()), 
    [DateUpdated] datetime NULL DEFAULT (getutcdate()), 
    CONSTRAINT [PK_WalletTransactions] PRIMARY KEY ([TransactionId])
)
