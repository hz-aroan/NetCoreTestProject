--| after: 01.001.001.001
--| description: create initial tables


IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.TABLES	WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Event' )
BEGIN
	CREATE TABLE [dbo].[Event] (
		[EventId] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](100) NOT NULL,
		[FeeCurrency] [nvarchar](10)  NULL,
		[FeeAmount] [decimal](18,2) NOT NULL,
		[IsAvailable] [bit] not null,
		[Description] [nvarchar](max) NULL,
		CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED ([EventId] ASC)
	)
END
GO

IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IDX_Event_Name' AND object_id = OBJECT_ID('dbo.Event'))
BEGIN
   CREATE INDEX [IDX_Event_Name] 
             ON [dbo].[Event] ([Name] ASC)
END
GO			 







IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.TABLES	WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Product' )
BEGIN
	CREATE TABLE [dbo].[Product] (
		[ProductId] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](100) NOT NULL,
		[IsAvailable] [bit] not null,
		[FeeCurrency] [nvarchar](10)  NULL,
		[FeeAmount] [decimal](18,2)  NULL,
		CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ProductId] ASC)
	)
END
GO

IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IDX_Product_Name' AND object_id = OBJECT_ID('dbo.Product'))
BEGIN
   CREATE UNIQUE INDEX [IDX_Product_Name] 
             ON [dbo].[Product] ([Name] ASC)
END
GO			 






IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.TABLES	WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'BasketHead' )
BEGIN
	CREATE TABLE [dbo].[BasketHead] (
		[BasketHeadId] [int] IDENTITY(1,1) NOT NULL,
		[UID] [uniqueidentifier]  NOT NULL,
		[EventId] [int]  NULL,
		CONSTRAINT [PK_BasketHead] PRIMARY KEY CLUSTERED ([BasketHeadId] ASC)
	)
END
GO

IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IDX_BasketHead_UID' AND object_id = OBJECT_ID('dbo.BasketHead'))
BEGIN
   CREATE UNIQUE INDEX [IDX_BasketHead_UID] 
             ON [dbo].[BasketHead] ([UID] ASC)
END
GO			 

IF NOT EXISTS (SELECT * FROM sys.objects o WHERE o.object_id = object_id(N'[dbo].[FK_BasketHead_Event]') AND OBJECTPROPERTY(o.object_id, N'IsForeignKey') = 1)
BEGIN
  ALTER TABLE [dbo].[BasketHead]
      ADD CONSTRAINT [FK_BasketHead_Event] 
	     FOREIGN KEY ([EventId]) 
             REFERENCES [dbo].[Event]([EventId])
             ON DELETE SET NULL
END
GO











IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.TABLES	WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'BasketItem' )
BEGIN
	CREATE TABLE [dbo].[BasketItem] (
		[BasketItemId] [int] IDENTITY(1,1) NOT NULL,
		[BasketHeadId] [int] NULL,
		[ProductId] [int] NULL,
		[Quantity] [int] NOT NULL,
		CONSTRAINT [PK_BasketItem] PRIMARY KEY CLUSTERED ([BasketItemId] ASC)
	)
END
GO

IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IDX_BasketItem_HEAD' AND object_id = OBJECT_ID('dbo.BasketItem'))
BEGIN
   CREATE INDEX [IDX_BasketItem_HEAD] 
             ON [dbo].[BasketItem] ([BasketHeadId] ASC)
END
GO			 


IF NOT EXISTS (SELECT * FROM sys.objects o WHERE o.object_id = object_id(N'[dbo].[FK_BasketItem_BasketHead]') AND OBJECTPROPERTY(o.object_id, N'IsForeignKey') = 1)
BEGIN
  ALTER TABLE [dbo].[BasketItem]
      ADD CONSTRAINT [FK_BasketItem_BasketHead] 
	FOREIGN KEY ([BasketHeadId]) 
	REFERENCES [dbo].[BasketHead]([BasketHeadId])
	ON DELETE SET NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects o WHERE o.object_id = object_id(N'[dbo].[FK_BasketItem_Product]') AND OBJECTPROPERTY(o.object_id, N'IsForeignKey') = 1)
BEGIN
  ALTER TABLE [dbo].[BasketItem]
      ADD CONSTRAINT [FK_BasketItem_Product] 
	FOREIGN KEY ([ProductId]) 
	REFERENCES [dbo].[Product]([ProductId])
	ON DELETE SET NULL
END
GO
