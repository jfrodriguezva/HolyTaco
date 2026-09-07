-- Siembra de promociones de ejemplo. Usa un cruce a HolyTacMenuDb solo aquí (herramienta de siembra),
-- el código de la aplicación nunca hace joins entre bases de datos de distintos microservicios.
USE HolyTacPromotionsDb;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Promotions)
BEGIN
    DECLARE @Promo1 UNIQUEIDENTIFIER = NEWID();
    DECLARE @Promo2 UNIQUEIDENTIFIER = NEWID();
    DECLARE @Promo3 UNIQUEIDENTIFIER = NEWID();

    DECLARE @Michelada UNIQUEIDENTIFIER = (SELECT Id FROM HolyTacMenuDb.dbo.MenuItems WHERE Name = N'Michelada HolyTac');
    DECLARE @Pastor UNIQUEIDENTIFIER = (SELECT Id FROM HolyTacMenuDb.dbo.MenuItems WHERE Name = N'Taco de Pastor');
    DECLARE @CervezaAmbar UNIQUEIDENTIFIER = (SELECT Id FROM HolyTacMenuDb.dbo.MenuItems WHERE Name = N'Cerveza Artesanal Ámbar');
    DECLARE @MezcalPaloma UNIQUEIDENTIFIER = (SELECT Id FROM HolyTacMenuDb.dbo.MenuItems WHERE Name = N'Mezcal Paloma');
    DECLARE @MargaritaTamarindo UNIQUEIDENTIFIER = (SELECT Id FROM HolyTacMenuDb.dbo.MenuItems WHERE Name = N'Margarita de Tamarindo');

    INSERT INTO dbo.Promotions (Id, Title, Description, ImageUrl, DiscountType, DiscountValue, ComboPrice, StartsAtUtc, EndsAtUtc, IsFeatured, IsActive)
    VALUES
    (@Promo1, N'2x1 en Micheladas', N'Todos los martes, lleva dos Micheladas HolyTac por el precio de una.', NULL, 1, 50.00, NULL, SYSUTCDATETIME(), DATEADD(DAY, 60, SYSUTCDATETIME()), 1, 1),
    (@Promo2, N'Combo Antro', N'Taco de Pastor + Michelada HolyTac + Cerveza Artesanal Ámbar a precio especial.', NULL, 3, NULL, 149.00, SYSUTCDATETIME(), DATEADD(DAY, 60, SYSUTCDATETIME()), 1, 1),
    (@Promo3, N'Happy Hour Coctelería', N'20% de descuento en Mezcal Paloma y Margarita de Tamarindo, de 18:00 a 20:00.', NULL, 1, 20.00, NULL, SYSUTCDATETIME(), DATEADD(DAY, 60, SYSUTCDATETIME()), 0, 1);

    IF @Michelada IS NOT NULL INSERT INTO dbo.PromotionMenuItems (PromotionId, MenuItemId) VALUES (@Promo1, @Michelada);

    IF @Pastor IS NOT NULL INSERT INTO dbo.PromotionMenuItems (PromotionId, MenuItemId) VALUES (@Promo2, @Pastor);
    IF @Michelada IS NOT NULL INSERT INTO dbo.PromotionMenuItems (PromotionId, MenuItemId) VALUES (@Promo2, @Michelada);
    IF @CervezaAmbar IS NOT NULL INSERT INTO dbo.PromotionMenuItems (PromotionId, MenuItemId) VALUES (@Promo2, @CervezaAmbar);

    IF @MezcalPaloma IS NOT NULL INSERT INTO dbo.PromotionMenuItems (PromotionId, MenuItemId) VALUES (@Promo3, @MezcalPaloma);
    IF @MargaritaTamarindo IS NOT NULL INSERT INTO dbo.PromotionMenuItems (PromotionId, MenuItemId) VALUES (@Promo3, @MargaritaTamarindo);
END
GO
