--CREATE TABLE dbo.ProductCOAs (
--    Id INT IDENTITY(1,1) PRIMARY KEY,
--    ProductId INT NOT NULL,
--    ProductSaleCoaId INT,
--    ProductBuyCoaId INT,
--    TenantId INT NOT NULL,
--    CompanyId INT NOT NULL,
--	Active bit default 0,
--    CreatedBy NVARCHAR(50),
--    CreatedOn DATETIME DEFAULT GETDATE(),
--    ModifiedBy NVARCHAR(50),
--    ModifiedOn DATETIME
--);





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProductCOA_BulkInsert')
    DROP PROCEDURE [dbo].[spProductCOA_BulkInsert];
GO

CREATE PROCEDURE [dbo].[spProductCOA_BulkInsert]
    @jsonInput NVARCHAR(MAX)
AS
BEGIN
	print '';
END;
GO


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProductCOA_GetById')
    DROP PROCEDURE [dbo].[spProductCOA_GetById];
GO

CREATE PROCEDURE [dbo].[spProductCOA_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        ProductId,
        ProductSaleCoaId,
        ProductBuyCoaId,
        TenantId,
        CompanyId,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM 
        dbo.ProductCOAs
    WHERE 
        Id = @Id;
END;
GO








IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProductCOA_GetAll')
    DROP PROCEDURE [dbo].[spProductCOA_GetAll];
GO
CREATE PROCEDURE [dbo].[spProductCOA_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        ProductId,
        ProductSaleCoaId,
        ProductBuyCoaId,
        TenantId,
        CompanyId,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM 
        dbo.ProductCOAs
END;
GO






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProductCOA_GetSelectedSaleCOAsByProductId')
    DROP PROCEDURE [dbo].[spProductCOA_GetSelectedSaleCOAsByProductId];
GO

CREATE PROCEDURE [dbo].[spProductCOA_GetSelectedSaleCOAsByProductId]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        ProductId,
        ProductSaleCoaId,
        ProductBuyCoaId,
        TenantId,
        CompanyId,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM 
        dbo.ProductCOAs
    WHERE 
        ProductId = @Id;
END;
GO




IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProductCOA_GetSelectedBuyCOAsByProductId')
    DROP PROCEDURE [dbo].[spProductCOA_GetSelectedBuyCOAsByProductId];
GO

CREATE PROCEDURE [dbo].[spProductCOA_GetSelectedBuyCOAsByProductId]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        ProductId,
        ProductSaleCoaId,
        ProductBuyCoaId,
        TenantId,
        CompanyId,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM 
        dbo.ProductCOAs
    WHERE 
        ProductId = @Id;
END;
GO








IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProductCOA_Insert')
    DROP PROCEDURE [dbo].[spProductCOA_Insert];
GO

CREATE PROCEDURE [dbo].[spProductCOA_Insert]
    @ProductId INT,
    @ProductSaleCoaId INT,
    @ProductBuyCoaId INT,
    @TenantId INT,
    @CompanyId INT,
    @Active bit,
	@CreatedOn datetime,
	@CreatedBy NVARCHAR(100),
	@ModifiedOn datetime,
	@ModifiedBy NVARCHAR(100),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ProductCOAs (ProductId, ProductSaleCoaId, ProductBuyCoaId, TenantId, CompanyId, CreatedBy, CreatedOn)
    VALUES (@ProductId, @ProductSaleCoaId, @ProductBuyCoaId, @TenantId, @CompanyId, @CreatedBy, GETDATE());

    SET @Id = SCOPE_IDENTITY();
END;
GO






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProductCOA_Update')
    DROP PROCEDURE [dbo].[spProductCOA_Update];
GO

CREATE PROCEDURE [dbo].[spProductCOA_Update]
    @Id INT,
    @ProductId INT,
    @ProductSaleCoaId INT,
    @ProductBuyCoaId INT,
    @TenantId INT,
    @CompanyId INT,
    @Active bit,
	@CreatedOn datetime,
	@CreatedBy NVARCHAR(100),
	@ModifiedOn datetime,
	@ModifiedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ProductCOAs
    SET 
        ProductId = @ProductId,
        ProductSaleCoaId = @ProductSaleCoaId,
        ProductBuyCoaId = @ProductBuyCoaId,
        TenantId = @TenantId,
        CompanyId = @CompanyId,
        ModifiedBy = @ModifiedBy,
        ModifiedOn = GETDATE()
    WHERE 
        Id = @Id;
END;
GO







IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProductCOA_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spProductCOA_GetAllByTenantAndCompanyId];
GO

CREATE PROCEDURE [dbo].[spProductCOA_GetAllByTenantAndCompanyId]
    @TenantId INT,
    @CompanyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        ProductId,
        ProductSaleCoaId,
        ProductBuyCoaId,
        TenantId,
        CompanyId,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM 
        dbo.ProductCOAs
    WHERE 
        TenantId = @TenantId AND CompanyId = @CompanyId;
END;
GO






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProductCOA_Delete')
    DROP PROCEDURE [dbo].[spProductCOA_Delete];
GO

CREATE PROCEDURE [dbo].[spProductCOA_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.ProductCOAs WHERE Id = @Id;
END;
GO




IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProductCOA_DeleteBuyAccountByProductId')
    DROP PROCEDURE [dbo].[spProductCOA_DeleteBuyAccountByProductId];
GO

CREATE PROCEDURE [dbo].[spProductCOA_DeleteBuyAccountByProductId]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.ProductCOAs WHERE ProductId = @Id and ProductSaleCoaId = 0
END;
GO



IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProductCOA_DeleteSaleAccountByProductId')
    DROP PROCEDURE [dbo].[spProductCOA_DeleteSaleAccountByProductId];
GO

CREATE PROCEDURE [dbo].[spProductCOA_DeleteSaleAccountByProductId]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.ProductCOAs WHERE ProductId = @Id and ProductBuyCoaId = 0
END;
GO


