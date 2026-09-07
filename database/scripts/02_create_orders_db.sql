-- Base de datos del microservicio de Pedidos
IF DB_ID('HolyTacOrdersDb') IS NULL
BEGIN
    CREATE DATABASE HolyTacOrdersDb;
END
GO

USE HolyTacOrdersDb;
GO

IF OBJECT_ID('dbo.Orders', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders
    (
        Id            UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        TableNumber   INT              NOT NULL,
        CustomerName  NVARCHAR(120)    NULL,
        Status        INT              NOT NULL, -- 1 Pendiente, 2 EnPreparacion, 3 Listo, 4 Entregado, 5 Cancelado
        CreatedAtUtc  DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_Orders_Status ON dbo.Orders(Status);
END
GO

IF OBJECT_ID('dbo.OrderItems', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderItems
    (
        Id            UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        OrderId       UNIQUEIDENTIFIER NOT NULL,
        MenuItemId    UNIQUEIDENTIFIER NOT NULL,
        MenuItemName  NVARCHAR(120)    NOT NULL,
        UnitPrice     DECIMAL(10,2)    NOT NULL,
        Quantity      INT              NOT NULL,
        Notes         NVARCHAR(300)    NULL,
        CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_OrderItems_OrderId ON dbo.OrderItems(OrderId);
END
GO
