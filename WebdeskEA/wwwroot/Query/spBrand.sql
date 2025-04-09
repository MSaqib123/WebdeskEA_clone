--CREATE TABLE dbo.Brands (
--    Id INT IDENTITY(1,1) PRIMARY KEY,
--    BrandName NVARCHAR(100) NOT NULL,
--    CompanyId INT NOT NULL,
--    TenantId INT NOT NULL,
--	Active bit default 0,
--    CreatedBy NVARCHAR(50) NOT NULL,
--    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),
--    ModifiedBy NVARCHAR(50) NULL,
--    ModifiedOn DATETIME NULL
--);
--GO


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBrand_BulkInsert')
    DROP PROCEDURE [dbo].[spBrand_BulkInsert];
GO

CREATE PROCEDURE [dbo].[spBrand_BulkInsert]
    @jsonInput NVARCHAR(MAX)
AS
BEGIN
   print '';
END;
GO





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBrand_Delete')
    DROP PROCEDURE [dbo].[spBrand_Delete];
GO

CREATE PROCEDURE [dbo].[spBrand_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Brands WHERE Id = @Id;
END;
GO






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBrand_Insert')
    DROP PROCEDURE [dbo].[spBrand_Insert];
GO

CREATE PROCEDURE [dbo].[spBrand_Insert]
    @BrandName NVARCHAR(100),
    @CompanyId INT,
    @TenantId INT,
    @Active bit,
	@CreatedOn datetime,
	@CreatedBy NVARCHAR(100),
	@ModifiedOn datetime,
	@ModifiedBy NVARCHAR(100),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Brands (BrandName, CompanyId, TenantId, CreatedBy, CreatedOn)
    VALUES (@BrandName, @CompanyId, @TenantId, @CreatedBy, GETDATE());

    SET @Id = SCOPE_IDENTITY();
END;
GO





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBrand_Update')
    DROP PROCEDURE [dbo].[spBrand_Update];
GO

CREATE PROCEDURE [dbo].[spBrand_Update]
    @Id INT,
    @BrandName NVARCHAR(100),
    @CompanyId INT,
    @TenantId INT,
    @Active bit,
	@CreatedOn datetime,
	@CreatedBy NVARCHAR(100),
	@ModifiedOn datetime,
	@ModifiedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Brands
    SET 
        BrandName = @BrandName,
        --CompanyId = @CompanyId,
        --TenantId = @TenantId,
        ModifiedBy = @ModifiedBy,
        ModifiedOn = GETDATE()
    WHERE 
        Id = @Id;
END;
GO





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBrand_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spBrand_GetAllByTenantAndCompanyId];
GO

CREATE PROCEDURE [dbo].[spBrand_GetAllByTenantAndCompanyId]
    @TenantId INT,
    @ParentCompanyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        BrandName,
        CompanyId,
        TenantId,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM 
        dbo.Brands
    WHERE 
        TenantId = @TenantId AND CompanyId = @ParentCompanyId;
END;
GO








IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBrand_GetAll')
    DROP PROCEDURE [dbo].[spBrand_GetAll];
GO

CREATE PROCEDURE [dbo].[spBrand_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        BrandName,
        CompanyId,
        TenantId,
        Active,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM 
        dbo.Brands;
END;
GO




IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBrand_GetById')
    DROP PROCEDURE [dbo].[spBrand_GetById];
GO

CREATE PROCEDURE [dbo].[spBrand_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        BrandName,
        CompanyId,
        TenantId,
        Active,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM 
        dbo.Brands
    WHERE 
        Id = @Id;
END;
GO
