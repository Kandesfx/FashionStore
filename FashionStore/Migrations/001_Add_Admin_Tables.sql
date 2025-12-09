-- =============================================
-- Migration Script: Thêm các bảng Admin theo ADMIN_BLUEPRINT
-- Ngày tạo: 2025-01-12
-- Mô tả: Thêm các bảng cần thiết cho hệ thống admin
-- =============================================

USE [FashionStore]
GO

-- =============================================
-- 1. Cập nhật các bảng hiện có
-- =============================================

-- Cập nhật bảng Product: Thêm các trường SEO, SKU, Barcode, CostPrice, LowStockThreshold
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Product]') AND name = 'Slug')
BEGIN
    ALTER TABLE [dbo].[Product]
    ADD [Slug] NVARCHAR(200) NULL,
        [MetaTitle] NVARCHAR(200) NULL,
        [MetaDescription] NVARCHAR(500) NULL,
        [SKU] NVARCHAR(50) NULL,
        [Barcode] NVARCHAR(50) NULL,
        [CostPrice] DECIMAL(18,2) NULL,
        [LowStockThreshold] INT NOT NULL DEFAULT 10,
        [UpdatedDate] DATETIME NULL;
    
    CREATE UNIQUE INDEX [IX_Products_Slug] ON [dbo].[Product]([Slug]) WHERE [Slug] IS NOT NULL;
    CREATE UNIQUE INDEX [IX_Products_SKU] ON [dbo].[Product]([SKU]) WHERE [SKU] IS NOT NULL;
END
GO

-- Cập nhật bảng Order: Thêm các trường liên quan thanh toán, vận chuyển
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND name = 'RecipientPhone')
BEGIN
    ALTER TABLE [dbo].[Order]
    ADD [RecipientPhone] NVARCHAR(20) NULL,
        [RecipientName] NVARCHAR(100) NULL,
        [ApprovedDate] DATETIME NULL,
        [PackedDate] DATETIME NULL,
        [ShippedDate] DATETIME NULL,
        [CompletedDate] DATETIME NULL,
        [CancelledDate] DATETIME NULL,
        [CancellationReason] NVARCHAR(500) NULL,
        [ShippingFee] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [DiscountAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [CouponCode] NVARCHAR(50) NULL,
        [UpdatedDate] DATETIME NULL;
END
GO

-- Cập nhật bảng User: Thêm các trường CRM
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND name = 'Gender')
BEGIN
    ALTER TABLE [dbo].[User]
    ADD [Gender] NVARCHAR(50) NULL,
        [DateOfBirth] DATE NULL,
        [ConversionChannel] NVARCHAR(100) NULL,
        [TotalOrderValue] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [TotalOrders] INT NOT NULL DEFAULT 0,
        [InternalNotes] NVARCHAR(500) NULL,
        [UpdatedDate] DATETIME NULL,
        [LastLoginDate] DATETIME NULL;
END
GO

-- Cập nhật bảng Category: Thêm ParentCategoryId, Slug, SEO fields
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Category]') AND name = 'ParentCategoryId')
BEGIN
    ALTER TABLE [dbo].[Category]
    ADD [ParentCategoryId] INT NULL,
        [Slug] NVARCHAR(200) NULL,
        [MetaTitle] NVARCHAR(200) NULL,
        [MetaDescription] NVARCHAR(500) NULL,
        [UpdatedDate] DATETIME NULL;
    
    ALTER TABLE [dbo].[Category]
    ADD CONSTRAINT [FK_Category_ParentCategory] FOREIGN KEY ([ParentCategoryId]) REFERENCES [dbo].[Category]([Id]);
    
    -- Filtered unique index: chỉ unique khi Slug IS NOT NULL, cho phép nhiều NULL
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Categories_Slug] 
    ON [dbo].[Category]([Slug]) 
    WHERE [Slug] IS NOT NULL;
END
GO

-- =============================================
-- 2. Tạo các bảng mới
-- =============================================

-- ProductVariant: Biến thể sản phẩm
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductVariant]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductVariant](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ProductId] INT NOT NULL,
        [SKU] NVARCHAR(50) NULL,
        [Barcode] NVARCHAR(50) NULL,
        [Size] NVARCHAR(50) NULL,
        [Color] NVARCHAR(50) NULL,
        [VariantName] NVARCHAR(100) NULL,
        [Price] DECIMAL(18,2) NULL,
        [Stock] INT NOT NULL DEFAULT 0,
        [ImageUrl] NVARCHAR(255) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedDate] DATETIME NULL,
        CONSTRAINT [PK_ProductVariant] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ProductVariant_Product] FOREIGN KEY([ProductId]) REFERENCES [dbo].[Product]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_ProductVariant_ProductId] ON [dbo].[ProductVariant]([ProductId]);
END
GO

-- Payment: Thanh toán
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Payment](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [OrderId] INT NOT NULL,
        [PaymentMethod] NVARCHAR(50) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [Amount] DECIMAL(18,2) NOT NULL,
        [TransactionId] NVARCHAR(100) NULL,
        [ReferenceCode] NVARCHAR(100) NULL,
        [Notes] NVARCHAR(500) NULL,
        [PaidDate] DATETIME NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedDate] DATETIME NULL,
        CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Payment_Order] FOREIGN KEY([OrderId]) REFERENCES [dbo].[Order]([Id])
    );
    
    CREATE INDEX [IX_Payments_OrderId] ON [dbo].[Payment]([OrderId]);
    CREATE INDEX [IX_Payments_Status] ON [dbo].[Payment]([Status]);
    CREATE INDEX [IX_Payments_TransactionId] ON [dbo].[Payment]([TransactionId]);
END
GO

-- Refund: Hoàn tiền
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Refund]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Refund](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [OrderId] INT NOT NULL,
        [PaymentId] INT NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [RefundType] NVARCHAR(50) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [Reason] NVARCHAR(500) NULL,
        [RefundTransactionId] NVARCHAR(100) NULL,
        [Notes] NVARCHAR(500) NULL,
        [ProcessedByUserId] INT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [CompletedDate] DATETIME NULL,
        CONSTRAINT [PK_Refund] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Refund_Order] FOREIGN KEY([OrderId]) REFERENCES [dbo].[Order]([Id]),
        CONSTRAINT [FK_Refund_Payment] FOREIGN KEY([PaymentId]) REFERENCES [dbo].[Payment]([Id]),
        CONSTRAINT [FK_Refund_User] FOREIGN KEY([ProcessedByUserId]) REFERENCES [dbo].[User]([Id])
    );
    
    CREATE INDEX [IX_Refund_OrderId] ON [dbo].[Refund]([OrderId]);
END
GO

-- Shipment: Vận chuyển
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Shipment]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Shipment](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [OrderId] INT NOT NULL,
        [ShippingCompany] NVARCHAR(100) NULL,
        [TrackingNumber] NVARCHAR(100) NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [ShippingFee] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [ShippingAddress] NVARCHAR(500) NULL,
        [RecipientPhone] NVARCHAR(20) NULL,
        [RecipientName] NVARCHAR(100) NULL,
        [PackedDate] DATETIME NULL,
        [ShippedDate] DATETIME NULL,
        [DeliveredDate] DATETIME NULL,
        [Notes] NVARCHAR(500) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedDate] DATETIME NULL,
        CONSTRAINT [PK_Shipment] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Shipment_Order] FOREIGN KEY([OrderId]) REFERENCES [dbo].[Order]([Id])
    );
    
    CREATE INDEX [IX_Shipment_OrderId] ON [dbo].[Shipment]([OrderId]);
END
GO

-- Return: Đổi trả hàng
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Return]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Return](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [OrderId] INT NOT NULL,
        [ReturnType] NVARCHAR(50) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Requested',
        [Reason] NVARCHAR(500) NOT NULL,
        [Description] NVARCHAR(1000) NULL,
        [ReturnTrackingNumber] NVARCHAR(100) NULL,
        [Notes] NVARCHAR(500) NULL,
        [ProcessedByUserId] INT NULL,
        [RequestedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ApprovedDate] DATETIME NULL,
        [ReceivedDate] DATETIME NULL,
        [CompletedDate] DATETIME NULL,
        CONSTRAINT [PK_Return] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Return_Order] FOREIGN KEY([OrderId]) REFERENCES [dbo].[Order]([Id]),
        CONSTRAINT [FK_Return_User] FOREIGN KEY([ProcessedByUserId]) REFERENCES [dbo].[User]([Id])
    );
    
    CREATE INDEX [IX_Return_OrderId] ON [dbo].[Return]([OrderId]);
END
GO

-- InventoryTransaction: Giao dịch tồn kho
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventoryTransaction]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[InventoryTransaction](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ProductId] INT NOT NULL,
        [ProductVariantId] INT NULL,
        [TransactionType] NVARCHAR(50) NOT NULL,
        [Quantity] INT NOT NULL,
        [Reason] NVARCHAR(500) NULL,
        [ReferenceCode] NVARCHAR(100) NULL,
        [OrderId] INT NULL,
        [CreatedByUserId] INT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [Notes] NVARCHAR(500) NULL,
        CONSTRAINT [PK_InventoryTransaction] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_InventoryTransaction_Product] FOREIGN KEY([ProductId]) REFERENCES [dbo].[Product]([Id]),
        CONSTRAINT [FK_InventoryTransaction_ProductVariant] FOREIGN KEY([ProductVariantId]) REFERENCES [dbo].[ProductVariant]([Id]),
        CONSTRAINT [FK_InventoryTransaction_Order] FOREIGN KEY([OrderId]) REFERENCES [dbo].[Order]([Id]),
        CONSTRAINT [FK_InventoryTransaction_User] FOREIGN KEY([CreatedByUserId]) REFERENCES [dbo].[User]([Id])
    );
    
    CREATE INDEX [IX_InventoryTransactions_ProductId] ON [dbo].[InventoryTransaction]([ProductId]);
    CREATE INDEX [IX_InventoryTransactions_TransactionType] ON [dbo].[InventoryTransaction]([TransactionType]);
    CREATE INDEX [IX_InventoryTransactions_CreatedDate] ON [dbo].[InventoryTransaction]([CreatedDate]);
END
GO

-- Promotion: Khuyến mãi
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Promotion]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Promotion](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [PromotionType] NVARCHAR(50) NOT NULL,
        [DiscountPercentage] DECIMAL(18,2) NULL,
        [DiscountAmount] DECIMAL(18,2) NULL,
        [MinimumOrderAmount] DECIMAL(18,2) NULL,
        [MaximumDiscountAmount] DECIMAL(18,2) NULL,
        [StartDate] DATETIME NOT NULL,
        [EndDate] DATETIME NOT NULL,
        [UsageLimit] INT NULL,
        [UsedCount] INT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedDate] DATETIME NULL,
        CONSTRAINT [PK_Promotion] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE INDEX [IX_Promotion_IsActive] ON [dbo].[Promotion]([IsActive]);
    CREATE INDEX [IX_Promotion_StartDate_EndDate] ON [dbo].[Promotion]([StartDate], [EndDate]);
END
GO

-- Coupon: Mã giảm giá
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Coupon]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Coupon](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Code] NVARCHAR(50) NOT NULL,
        [Name] NVARCHAR(200) NULL,
        [Description] NVARCHAR(500) NULL,
        [DiscountType] NVARCHAR(50) NOT NULL,
        [DiscountPercentage] DECIMAL(18,2) NULL,
        [DiscountAmount] DECIMAL(18,2) NULL,
        [MinimumOrderAmount] DECIMAL(18,2) NULL,
        [MaximumDiscountAmount] DECIMAL(18,2) NULL,
        [StartDate] DATETIME NOT NULL,
        [EndDate] DATETIME NOT NULL,
        [UsageLimit] INT NULL,
        [UsedCount] INT NOT NULL DEFAULT 0,
        [UsageLimitPerUser] INT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedDate] DATETIME NULL,
        CONSTRAINT [PK_Coupon] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE INDEX [IX_Coupons_Code] ON [dbo].[Coupon]([Code]);
    CREATE INDEX [IX_Coupons_IsActive] ON [dbo].[Coupon]([IsActive]);
    CREATE INDEX [IX_Coupons_StartDate_EndDate] ON [dbo].[Coupon]([StartDate], [EndDate]);
END
GO

-- CouponUsage: Lịch sử sử dụng mã giảm giá
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CouponUsage]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CouponUsage](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [CouponId] INT NOT NULL,
        [OrderId] INT NOT NULL,
        [UserId] INT NOT NULL,
        [DiscountAmount] DECIMAL(18,2) NOT NULL,
        [UsedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_CouponUsage] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_CouponUsage_Coupon] FOREIGN KEY([CouponId]) REFERENCES [dbo].[Coupon]([Id]),
        CONSTRAINT [FK_CouponUsage_Order] FOREIGN KEY([OrderId]) REFERENCES [dbo].[Order]([Id]),
        CONSTRAINT [FK_CouponUsage_User] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id])
    );
    
    CREATE INDEX [IX_CouponUsage_CouponId] ON [dbo].[CouponUsage]([CouponId]);
    CREATE INDEX [IX_CouponUsage_OrderId] ON [dbo].[CouponUsage]([OrderId]);
    CREATE INDEX [IX_CouponUsage_UserId] ON [dbo].[CouponUsage]([UserId]);
END
GO

-- Permission: Quyền truy cập
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permission]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Permission](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(200) NULL,
        [Module] NVARCHAR(100) NOT NULL,
        [Action] NVARCHAR(50) NOT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE INDEX [IX_Permissions_Name] ON [dbo].[Permission]([Name]);
    CREATE INDEX [IX_Permission_Module] ON [dbo].[Permission]([Module]);
END
GO

-- RolePermission: Quan hệ Role-Permission
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RolePermission]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RolePermission](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [RoleId] INT NOT NULL,
        [PermissionId] INT NOT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_RolePermission] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_RolePermission_Role] FOREIGN KEY([RoleId]) REFERENCES [dbo].[Role]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermission_Permission] FOREIGN KEY([PermissionId]) REFERENCES [dbo].[Permission]([Id]) ON DELETE CASCADE
    );
    
    CREATE UNIQUE INDEX [IX_RolePermission_RoleId_PermissionId] ON [dbo].[RolePermission]([RoleId], [PermissionId]);
END
GO

-- AuditLog: Nhật ký audit
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AuditLog](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [TableName] NVARCHAR(100) NOT NULL,
        [RecordId] INT NOT NULL,
        [Action] NVARCHAR(50) NOT NULL,
        [FieldName] NVARCHAR(100) NULL,
        [OldValue] NTEXT NULL,
        [NewValue] NTEXT NULL,
        [UserId] INT NULL,
        [IpAddress] NVARCHAR(50) NULL,
        [Notes] NVARCHAR(500) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_AuditLog_User] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id])
    );
    
    CREATE INDEX [IX_AuditLogs_TableName] ON [dbo].[AuditLog]([TableName]);
    CREATE INDEX [IX_AuditLogs_TableName_RecordId] ON [dbo].[AuditLog]([TableName], [RecordId]);
    CREATE INDEX [IX_AuditLogs_CreatedDate] ON [dbo].[AuditLog]([CreatedDate]);
END
GO

-- Address: Địa chỉ khách hàng
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Address]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Address](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [RecipientName] NVARCHAR(100) NOT NULL,
        [Phone] NVARCHAR(20) NOT NULL,
        [StreetAddress] NVARCHAR(200) NOT NULL,
        [Ward] NVARCHAR(100) NULL,
        [District] NVARCHAR(100) NULL,
        [Province] NVARCHAR(100) NULL,
        [PostalCode] NVARCHAR(20) NULL,
        [IsDefault] BIT NOT NULL DEFAULT 0,
        [AddressType] NVARCHAR(50) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedDate] DATETIME NULL,
        CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Address_User] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_Addresses_UserId] ON [dbo].[Address]([UserId]);
END
GO

-- Media: Media của sản phẩm
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Media]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Media](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ProductId] INT NULL,
        [ProductVariantId] INT NULL,
        [Url] NVARCHAR(500) NOT NULL,
        [FileName] NVARCHAR(200) NULL,
        [MediaType] NVARCHAR(50) NULL,
        [Size] NVARCHAR(50) NULL,
        [DisplayOrder] INT NOT NULL DEFAULT 0,
        [AltText] NVARCHAR(200) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Media] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Media_Product] FOREIGN KEY([ProductId]) REFERENCES [dbo].[Product]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Media_ProductVariant] FOREIGN KEY([ProductVariantId]) REFERENCES [dbo].[ProductVariant]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_Media_ProductId] ON [dbo].[Media]([ProductId]);
    CREATE INDEX [IX_Media_ProductVariantId] ON [dbo].[Media]([ProductVariantId]);
END
GO

-- Collection: Bộ sưu tập
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Collection]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Collection](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [ImageUrl] NVARCHAR(255) NULL,
        [Slug] NVARCHAR(200) NULL,
        [DisplayOrder] INT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedDate] DATETIME NULL,
        CONSTRAINT [PK_Collection] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE INDEX [IX_Collections_Slug] ON [dbo].[Collection]([Slug]) WHERE [Slug] IS NOT NULL;
END
GO

-- ProductCollection: Quan hệ Product-Collection
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductCollection]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductCollection](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ProductId] INT NOT NULL,
        [CollectionId] INT NOT NULL,
        [DisplayOrder] INT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_ProductCollection] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ProductCollection_Product] FOREIGN KEY([ProductId]) REFERENCES [dbo].[Product]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProductCollection_Collection] FOREIGN KEY([CollectionId]) REFERENCES [dbo].[Collection]([Id]) ON DELETE CASCADE
    );
    
    CREATE UNIQUE INDEX [IX_ProductCollections_ProductId_CollectionId] ON [dbo].[ProductCollection]([ProductId], [CollectionId]);
END
GO

PRINT 'Migration hoàn tất: Đã thêm tất cả các bảng Admin theo ADMIN_BLUEPRINT'
GO

