--============================================================
--=========================== City ===========================
--============================================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCity_GetAll')
    DROP PROCEDURE [dbo].[spCity_GetAll];
GO
CREATE PROCEDURE spCity_GetAll
AS
BEGIN
    SELECT 
	c.Id, 
	c.CityName,
	c.CityLatitude,
	c.CityLongitude,

	--__________ Joint Columns _________
    s.stateProvinceName AS StateName,
    cn.countryName AS CountryName
    FROM Cities c
    INNER JOIN StateProvinces s ON c.StateProvinceId = s.Id
    INNER JOIN Countries cn ON s.countryId = cn.Id;
END; 
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCity_GetById')
    DROP PROCEDURE [dbo].[spCity_GetById];
GO
CREATE PROCEDURE spCity_GetById
    @CityId INT
AS
BEGIN
    SELECT 
	c.Id, 
	c.CityName,
	c.CityLatitude,
	c.CityLongitude,

	--__________ Joint Columns _________
    s.stateProvinceName AS StateName,
    cn.countryName AS CountryName
    FROM Cities c
    INNER JOIN StateProvinces s ON c.StateProvinceId = s.Id
    INNER JOIN Countries cn ON s.countryId = cn.Id
    WHERE c.Id = @CityId;
END;
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCityByStateId_GetAll')
    DROP PROCEDURE [dbo].[spCityByStateId_GetAll];
GO
CREATE PROCEDURE spCityByStateId_GetAll
    @StateId INT
AS
BEGIN
    SELECT 
	c.Id, 
	c.CityName,
	c.CityLatitude,
	c.CityLongitude,

	--__________ Joint Columns _________
    s.stateProvinceName AS StateName,
    cn.countryName AS CountryName
    FROM Cities c
    INNER JOIN StateProvinces s ON c.StateProvinceId = s.Id
    INNER JOIN Countries cn ON s.countryId = cn.Id
    WHERE c.StateProvinceId = @StateId;
END;
GO

--============================================================
--======================== Country ===========================
--============================================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCountry_GetAll')
    DROP PROCEDURE [dbo].[spCountry_GetAll];
GO
CREATE PROCEDURE spCountry_GetAll
AS
BEGIN
    SELECT Id, countryName, countryIso3, countryIso2, capital, currency, currencyName, currencySymbol, region, subRegion, countryLatitude, countryLongitude
    FROM Countries;
END;
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCountry_GetById')
    DROP PROCEDURE [dbo].[spCountry_GetById];
GO
CREATE PROCEDURE spCountry_GetById
    @CountryId INT
AS
BEGIN
    SELECT Id, countryName, countryIso3, countryIso2, capital, currency, currencyName, currencySymbol, region, subRegion, countryLatitude, countryLongitude
    FROM Countries
    WHERE Id = @CountryId;
END;
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spStateProvince_GetAll')
    DROP PROCEDURE [dbo].[spStateProvince_GetAll];
GO

--============================================================
--========================== State ===========================
--============================================================
CREATE PROCEDURE spStateProvince_GetAll
AS
BEGIN
    SELECT 
	sp.Id, sp.stateProvinceName, 
	sp.stateProvinceCode, 
	sp.stateProvinceLatitude,
	sp.stateProvinceLongitude,
	--__________ Joint Columns _________
    c.countryName AS CountryName
    FROM StateProvinces sp
    INNER JOIN Countries c ON sp.countryId = c.Id;
END;
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spStateProvinceGet_ById')
    DROP PROCEDURE [dbo].[spStateProvinceGet_ById];
GO
CREATE PROCEDURE spStateProvinceGet_ById
    @StateProvinceId INT
AS
BEGIN
    SELECT 
	sp.Id,
	sp.stateProvinceName,
	sp.stateProvinceCode, 
	sp.stateProvinceLatitude, 
	sp.stateProvinceLongitude,
	--__________ Joint Columns _________
	c.countryName AS CountryName
    FROM StateProvinces sp
    INNER JOIN Countries c ON sp.countryId = c.Id
    WHERE sp.Id = @StateProvinceId;
END;
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spStateProvinceByCountryId_GetById')
    DROP PROCEDURE [dbo].[spStateProvinceByCountryId_GetById];
GO
CREATE PROCEDURE spStateProvinceByCountryId_GetById
    @countryId INT
AS
BEGIN
    SELECT 
	sp.Id,
	sp.stateProvinceName,
	sp.stateProvinceCode, 
	sp.stateProvinceLatitude, 
	sp.stateProvinceLongitude,
	--__________ Joint Columns _________
	c.countryName AS CountryName
    FROM StateProvinces sp
    INNER JOIN Countries c ON sp.countryId = c.Id
    WHERE sp.CountryId = @countryId;
END;
GO