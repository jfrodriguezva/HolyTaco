-- Base de datos del microservicio de Reservaciones
IF DB_ID('HolyTacReservationsDb') IS NULL
BEGIN
    CREATE DATABASE HolyTacReservationsDb;
END
GO

USE HolyTacReservationsDb;
GO

IF OBJECT_ID('dbo.Tables', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tables
    (
        Id       UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Number   INT              NOT NULL UNIQUE,
        Capacity INT              NOT NULL,
        Zone     INT              NOT NULL -- 1 Interior, 2 Terraza, 3 Barra
    );
END
GO

IF OBJECT_ID('dbo.Reservations', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Reservations
    (
        Id               UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        CustomerName     NVARCHAR(120)    NOT NULL,
        Phone            NVARCHAR(30)     NOT NULL,
        Email            NVARCHAR(200)    NULL,
        PartySize        INT              NOT NULL,
        ReservationAtUtc DATETIME2        NOT NULL,
        TableNumber      INT              NOT NULL,
        Status           INT              NOT NULL, -- 1 Pendiente, 2 Confirmada, 3 Cancelada, 4 Completada
        Notes            NVARCHAR(300)    NULL,
        CreatedAtUtc     DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_Reservations_ReservationAtUtc ON dbo.Reservations(ReservationAtUtc);
    CREATE INDEX IX_Reservations_TableNumber ON dbo.Reservations(TableNumber);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Tables)
BEGIN
    INSERT INTO dbo.Tables (Id, Number, Capacity, Zone) VALUES
    (NEWID(), 1, 2, 3),   -- Barra
    (NEWID(), 2, 2, 3),   -- Barra
    (NEWID(), 3, 4, 1),   -- Interior
    (NEWID(), 4, 4, 1),   -- Interior
    (NEWID(), 5, 4, 1),   -- Interior
    (NEWID(), 6, 6, 1),   -- Interior
    (NEWID(), 7, 6, 2),   -- Terraza
    (NEWID(), 8, 8, 2),   -- Terraza
    (NEWID(), 9, 2, 2),   -- Terraza
    (NEWID(), 10, 10, 1); -- Interior (mesa grande / eventos)
END
GO
