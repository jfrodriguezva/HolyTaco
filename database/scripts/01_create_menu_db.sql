-- Base de datos del microservicio de Menú
IF DB_ID('HolyTacMenuDb') IS NULL
BEGIN
    CREATE DATABASE HolyTacMenuDb;
END
GO

USE HolyTacMenuDb;
GO

IF OBJECT_ID('dbo.MenuItems', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MenuItems
    (
        Id            UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name          NVARCHAR(120)    NOT NULL,
        Description   NVARCHAR(500)    NOT NULL DEFAULT '',
        Price         DECIMAL(10,2)    NOT NULL,
        Currency      NVARCHAR(3)      NOT NULL DEFAULT 'MXN',
        Category      INT              NOT NULL, -- 1 Tacos, 2 Antojitos, 3 Platillos, 4 Coctelería, 5 Cervezas, 6 Bebidas, 7 Postres
        IsSpicy       BIT              NOT NULL DEFAULT 0,
        IsAvailable   BIT              NOT NULL DEFAULT 1,
        ImageUrl      NVARCHAR(500)    NULL
    );

    CREATE INDEX IX_MenuItems_Category ON dbo.MenuItems(Category);
END
GO
