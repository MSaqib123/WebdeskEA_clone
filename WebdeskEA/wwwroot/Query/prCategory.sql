--CREATE TABLE Category (
--    Id INT IDENTITY(1,1) PRIMARY KEY,
--    CategoryName NVARCHAR(255) NOT NULL,
--    CompanyId INT  null,
--    TenantId INT null,
--    Active BIT NULL,
--    Image nvarchar(500) NULL,  -- To store image data; alternatively, use an NVARCHAR field for URLs
--    CreatedBy NVARCHAR(50) NULL,
--    CreatedOn DATETIME2 NOT NULL DEFAULT GETDATE(),
--    ModifiedBy NVARCHAR(50) NULL,
--    ModifiedOn DATETIME2 NULL
--);





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCategory_Insert')
    DROP PROCEDURE [dbo].[spCategory_Insert];
GO
CREATE PROCEDURE spCategory_Insert
    @CategoryName NVARCHAR(255),
    @CompanyId INT,
    @TenantId INT,
    @Active BIT,
    @Image NVARCHAR(500),
    @CreatedBy NVARCHAR(50),
    @CreatedOn DATETIME2,
    @ModifiedBy NVARCHAR(50),
    @ModifiedOn DATETIME2,
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO Category (CategoryName, CompanyId, TenantId, Active, Image, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn)
    VALUES (@CategoryName, @CompanyId, @TenantId, @Active, @Image, @CreatedBy, @CreatedOn, @ModifiedBy, @ModifiedOn);

    SET @Id = SCOPE_IDENTITY();
END;
GO






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCategory_Update')
    DROP PROCEDURE [dbo].[spCategory_Update];
GO
CREATE PROCEDURE spCategory_Update
    @Id INT,
    @CategoryName NVARCHAR(255),
    @CompanyId INT,
    @TenantId INT,
    @Active BIT,
    @Image NVARCHAR(500),
    @CreatedBy NVARCHAR(50),
    @CreatedOn DATETIME2,
    @ModifiedBy NVARCHAR(50),
    @ModifiedOn DATETIME2
AS
BEGIN
    UPDATE Category
    SET CategoryName = @CategoryName,
        CompanyId = @CompanyId,
        TenantId = @TenantId,
        Active = @Active,
        Image = @Image,
        ModifiedBy = @ModifiedBy,
        ModifiedOn = @ModifiedOn
    WHERE Id = @Id;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCategory_Delete')
    DROP PROCEDURE [dbo].[spCategory_Delete];
GO
CREATE PROCEDURE spCategory_Delete
    @Id INT
AS
BEGIN
    DELETE FROM Category
    WHERE Id = @Id;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCategory_GetAll')
    DROP PROCEDURE [dbo].[spCategory_GetAll];
GO
CREATE PROCEDURE spCategory_GetAll
AS
BEGIN
    SELECT 
        Id,
        CategoryName,
        CompanyId,
        TenantId,
        Active,
        Image,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM Category;
END;
GO





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCategory_GetById')
    DROP PROCEDURE [dbo].[spCategory_GetById];
GO
CREATE PROCEDURE spCategory_GetById
    @Id INT
AS
BEGIN
    SELECT 
        Id,
        CategoryName,
        CompanyId,
        TenantId,
        Active,
        Image,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM Category
    WHERE Id = @Id;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCategory_GetByTenant')
    DROP PROCEDURE [dbo].[spCategory_GetByTenant];
GO
CREATE PROCEDURE spCategory_GetByTenant
    @TenantId INT
AS
BEGIN
    SELECT 
        Id,
        CategoryName,
        CompanyId,
        TenantId,
        Active,
        Image,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM Category
    WHERE TenantId = @TenantId;
END;
GO








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCategory_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spCategory_GetAllByTenantAndCompanyId];
GO
CREATE PROCEDURE spCategory_GetAllByTenantAndCompanyId
    @TenantId INT,
    @CompanyId INT
AS
BEGIN
    SELECT 
        Id,
        CategoryName,
        CompanyId,
        TenantId,
        Active,
        Image,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM Category
    WHERE TenantId = @TenantId AND CompanyId = @CompanyId;
END;
GO
