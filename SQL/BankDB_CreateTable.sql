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
 CONSTRAINT [PK_AspNetUser] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[CustomerAccount](
	[IBANNo] [nvarchar](34) NOT NULL,
	[AccountNo] [nvarchar](20) NOT NULL,
	[IDCard] [nvarchar](20) NOT NULL,
	[FullName] [nvarchar](35) NOT NULL,
	[Balance] [decimal](18, 2) NOT NULL,	
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[IBANNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CustomerAccount]  WITH CHECK ADD  CONSTRAINT [FK_Accounts_User_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CustomerAccount] CHECK CONSTRAINT [FK_Accounts_User_CreatedBy]
GO


CREATE TABLE [dbo].[MasterFee](
	[EffectiveDate] [date] NOT NULL,
	[Fee_Per] [decimal](9, 2) NULL,	
	[CreatedBy] [varchar](20) NULL,
	[CreatedTime] [datetime] NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedTime] [datetime] NULL,
 CONSTRAINT [PK_MasterFee] PRIMARY KEY CLUSTERED 
(
	[EffectiveDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


