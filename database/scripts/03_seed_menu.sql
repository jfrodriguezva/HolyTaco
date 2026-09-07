USE HolyTacMenuDb;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.MenuItems)
BEGIN
    INSERT INTO dbo.MenuItems (Id, Name, Description, Price, Currency, Category, IsSpicy, IsAvailable, ImageUrl) VALUES
    -- Tacos
    (NEWID(), N'Taco de Pastor',        N'Trompo marinado con achiote, piña asada, cebolla y cilantro', 22.00, 'MXN', 1, 1, 1, NULL),
    (NEWID(), N'Taco de Suadero',       N'Suadero dorado en su propia grasa, con salsa verde', 22.00, 'MXN', 1, 0, 1, NULL),
    (NEWID(), N'Taco de Birria',        N'Birria de res estilo Jalisco con consomé para remojar', 28.00, 'MXN', 1, 1, 1, NULL),
    (NEWID(), N'Taco Gobernador',       N'Camarón, queso Oaxaca, pimiento y cebolla a la plancha', 45.00, 'MXN', 1, 0, 1, NULL),
    -- Antojitos
    (NEWID(), N'Gringa de Pastor',      N'Tortilla de harina, pastor, queso derretido y piña', 55.00, 'MXN', 2, 1, 1, NULL),
    (NEWID(), N'Quesadilla de Flor de Calabaza', N'Tortilla hecha a mano, flor de calabaza y queso Oaxaca', 40.00, 'MXN', 2, 0, 1, NULL),
    (NEWID(), N'Vampiro de Suadero',    N'Tostada de tortilla frita con suadero y queso gratinado', 48.00, 'MXN', 2, 0, 1, NULL),
    -- Platillos
    (NEWID(), N'Molcajete HolyTac',     N'Arrachera, pollo, chorizo, nopales y queso panela en molcajete caliente', 245.00, 'MXN', 3, 1, 1, NULL),
    (NEWID(), N'Costillas en Salsa Guajillo', N'Costilla de cerdo braseada en salsa de chile guajillo', 195.00, 'MXN', 3, 1, 1, NULL),
    -- Coctelería
    (NEWID(), N'Mezcal Paloma',         N'Mezcal artesanal, toronja y sal de gusano', 130.00, 'MXN', 4, 0, 1, NULL),
    (NEWID(), N'Margarita de Tamarindo',N'Tequila blanco, tamarindo, chile piquín en el escarchado', 125.00, 'MXN', 4, 1, 1, NULL),
    (NEWID(), N'Michelada HolyTac',     N'Cerveza clara, salsa maggi, clamato y limón', 95.00, 'MXN', 4, 1, 1, NULL),
    -- Cervezas
    (NEWID(), N'Cerveza Artesanal Ámbar', N'Cerveza de la casa estilo amber ale, 500ml', 85.00, 'MXN', 5, 0, 1, NULL),
    (NEWID(), N'Cerveza Clara Nacional', N'Cerveza clara 355ml', 55.00, 'MXN', 5, 0, 1, NULL),
    -- Bebidas
    (NEWID(), N'Agua de Horchata',      N'Horchata de arroz con canela, preparación de la casa', 35.00, 'MXN', 6, 0, 1, NULL),
    (NEWID(), N'Refresco',              N'Refresco de línea 355ml', 30.00, 'MXN', 6, 0, 1, NULL),
    -- Postres
    (NEWID(), N'Churros con Cajeta',    N'Churros artesanales rellenos de cajeta', 65.00, 'MXN', 7, 0, 1, NULL);
END
GO
