USE [BankDB]
GO

/****** Object:  Table [dbo].[Accounts]    Script Date: 20/06/2021 08:17:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[User](
	[UserId] [nvarchar](20) NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[FullName] [nvarchar](200) NOT NULL,
	[CreatedDate] [datetime]  NULL,
	[CreatedBy] [nvarchar](20)  NULL,
	[ModifiedDate] [datetime]  NULL,
	[ModifiedBy] [nvarchar](20)  NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[CustomerAccount](
	
	[AccountNo] [nvarchar](34) NOT NULL,
	[IBANNo] [nvarchar](34) NOT NULL,
	[FullName] [nvarchar](200) NOT NULL,
	[Balance] [decimal](18, 2) NOT NULL,	
	[IsActived] bit  NOT NULL DEFAULT 1,	
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[ModifiedDate] [datetime]  NULL,
	[ModifiedBy] [nvarchar](20)  NULL,

 CONSTRAINT [PK_CustomerAccount] PRIMARY KEY CLUSTERED 
(
	[AccountNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CustomerAccount]  WITH CHECK ADD  CONSTRAINT [FK_CustomerAccount_User_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[User] ([UserId])
--ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CustomerAccount] CHECK CONSTRAINT [FK_CustomerAccount_User_CreatedBy]
GO

CREATE NONCLUSTERED INDEX [IX_CustomerAccount_AccountNo] ON [dbo].[CustomerAccount]
(
	[AccountNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO



CREATE TABLE [dbo].[MasterFee](
	[EffectiveDate] [date] NOT NULL,
	[FeeType] [nvarchar](1) NOT NULL, -- T = Transfer, D = Deposit, W = Withdraw
	[FeePercent] [decimal](9, 2) NOT NULL,	
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedTime] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedTime] [datetime] NULL,
 CONSTRAINT [PK_MasterFee] PRIMARY KEY CLUSTERED 
(
	[EffectiveDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MasterFee]  WITH CHECK ADD  CONSTRAINT [FK_MasterFee_User_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[User] ([UserId])
--ON DELETE CASCADE
GO

ALTER TABLE [dbo].[MasterFee] CHECK CONSTRAINT [FK_MasterFee_User_CreatedBy]
GO



CREATE TABLE [dbo].[Transaction](
	[Id] bigint identity(1,1) NOT NULL,	
	[AccountNo] [nvarchar](34) NOT NULL,	
	[ActionType] [nvarchar](1) NOT NULL, -- T = Transfer, D = Deposit, W = Withdraw
	[Amount] [decimal](18, 2) NOT NULL,
	[FeePercent] [decimal](9, 2) NOT NULL,
	[FeeAmount] [decimal](18, 2) NOT NULL,
	[NetAmount] [decimal](18, 2) NOT NULL,
	[ActionBy] [nvarchar](20) NOT NULL,
	[ActionDate] [datetime] NOT NULL,
	[ReferenceNo] [nvarchar](100) NOT NULL,
	[DestinationNo] [nvarchar](34) NULL,	
	[Description] [nvarchar](200) NULL,
 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_CustomerAccount_AccountNo] FOREIGN KEY([AccountNo])
REFERENCES [dbo].[CustomerAccount] ([AccountNo])
--ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_CustomerAccount_AccountNo]
GO

ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_User_ActionBy] FOREIGN KEY([ActionBy])
REFERENCES [dbo].[User] ([UserId])
--ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_User_ActionBy]
GO


CREATE NONCLUSTERED INDEX [IX_Transaction_ActionType] ON [dbo].[Transaction]
(
	[ActionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Transaction_ReferenceNo] ON [dbo].[Transaction]
(
	[ReferenceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO










