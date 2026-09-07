-- Base de datos del microservicio de Promociones
IF DB_ID('HolyTacPromotionsDb') IS NULL
BEGIN
    CREATE DATABASE HolyTacPromotionsDb;
END
GO

USE HolyTacPromotionsDb;
GO

IF OBJECT_ID('dbo.Promotions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Promotions
    (
        Id            UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Title         NVARCHAR(150)    NOT NULL,
        Description   NVARCHAR(500)    NOT NULL DEFAULT '',
        ImageUrl      NVARCHAR(500)    NULL,
        DiscountType  INT              NOT NULL, -- 1 Percentage, 2 FixedAmount, 3 ComboPrice
        DiscountValue DECIMAL(10,2)    NULL,
        ComboPrice    DECIMAL(10,2)    NULL,
        StartsAtUtc   DATETIME2        NOT NULL,
        EndsAtUtc     DATETIME2        NOT NULL,
        IsFeatured    BIT              NOT NULL DEFAULT 0,
        IsActive      BIT              NOT NULL DEFAULT 1
    );
END
GO

IF OBJECT_ID('dbo.PromotionMenuItems', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PromotionMenuItems
    (
        PromotionId UNIQUEIDENTIFIER NOT NULL,
        MenuItemId  UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT PK_PromotionMenuItems PRIMARY KEY (PromotionId, MenuItemId),
        CONSTRAINT FK_PromotionMenuItems_Promotions FOREIGN KEY (PromotionId) REFERENCES dbo.Promotions(Id) ON DELETE CASCADE
    );
END
GO
