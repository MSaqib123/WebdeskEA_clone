IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProduct_BulkInsert')
    DROP PROCEDURE [dbo].[spProduct_BulkInsert];
GO
CREATE PROCEDURE [dbo].[spProduct_BulkInsert]
    @jsonInput NVARCHAR(MAX)
AS
BEGIN
    -- Logic for bulk insert using JSON input
    -- Use OPENJSON or other logic as needed to parse and insert data from JSON input
	print '';
END;
GO


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProduct_Delete')
    DROP PROCEDURE [dbo].[spProduct_Delete];
GO
CREATE PROCEDURE [dbo].[spProduct_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM Products WHERE Id = @Id;
END;
GO





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProduct_GetAll')
    DROP PROCEDURE [dbo].[spProduct_GetAll];
GO
CREATE PROCEDURE [dbo].[spProduct_GetAll]
AS
BEGIN
    SELECT 
	p.Id, 
	p.ProductCode, 
	p.BrandId,
	p.ProductName,
	p.ProductSKU,
	p.ProductDescription,
	p.ProductPrice,
    p.isProductSale,
	p.isProductBuy,
	p.Image,
	p.CategoryId,
	p.TaxId,
	p.TenantId,
	p.CompanyId,
    p.CreatedBy, 
	p.CreatedOn,
	p.ModifiedBy,
	p.ModifiedOn,
	--____ JOined Column ____
	isNull(b.BrandName,'--') as BrandName,
	isNull(tm.TaxName,'--') as TaxName,
	isNull(cat.CategoryName,'--') as CategoryName
    FROM Products p
	left join Brands b on b.Id = p.BrandId
	left join TaxMasters tm on tm.Id = p.TaxId
	left join Category cat on cat.Id = p.CategoryId
END;
GO






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProduct_GetById')
    DROP PROCEDURE [dbo].[spProduct_GetById];
GO
CREATE PROCEDURE [dbo].[spProduct_GetById]
    @Id INT
AS
BEGIN
    SELECT 
	p.Id, 
	p.ProductCode, 
	p.BrandId,
	p.ProductName,
	p.ProductSKU,
	p.ProductDescription,
	p.ProductPrice,
    p.isProductSale,
	p.isProductBuy,
	p.Image,
	p.CategoryId,
	p.TaxId,
	p.TenantId,
	p.CompanyId,
    p.CreatedBy, 
	p.CreatedOn,
	p.ModifiedBy,
	p.ModifiedOn,
	--____ JOined Column ____
	isNull(b.BrandName,'--') as BrandName,
	isNull(tm.TaxName,'--') as TaxName,
	isNull(cat.CategoryName,'--') as CategoryName
    FROM Products p
	left join Brands b on b.Id = p.BrandId
	left join TaxMasters tm on tm.Id = p.TaxId
	left join Category cat on cat.Id = p.CategoryId
	where p.Id = @Id
END;
GO





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProduct_Insert')
    DROP PROCEDURE [dbo].[spProduct_Insert];
GO
CREATE PROCEDURE [dbo].[spProduct_Insert]
    @ProductCode NVARCHAR(50),
    @BrandId INT,
    @ProductName NVARCHAR(100),
    @ProductSKU NVARCHAR(100),
    @ProductDescription NVARCHAR(500),
    @ProductPrice DECIMAL(18, 2),
    @isProductSale BIT,
    @isProductBuy BIT,
	@Image nvarchar(500),
	@CategoryId int,
    @TaxId INT,
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
	--declare @NewCode varchar(40);
	--BEGIN
	--	EXEC dbo.GenerateCode  @TableName = 'Products', @ColumnName = 'ProductCode', @Prefix = 'PRO',  @NewCode  = @ProductCode OUTPUT;
	--END

    INSERT INTO Products (
	ProductCode, 
	BrandId, 
	ProductName,
	ProductSKU,
	ProductDescription,
	ProductPrice,
	isProductSale, 
	isProductBuy, 
	TaxId, 
	Image,
	CategoryId,
	TenantId, 
	CompanyId,
	CreatedBy, 
	CreatedOn
	)
    VALUES (@ProductCode, @BrandId, @ProductName, @ProductSKU, @ProductDescription, @ProductPrice,@isProductSale, @isProductBuy, @TaxId,@Image,@CategoryId, @TenantId, @CompanyId, @CreatedBy, GETDATE());
    SET @Id = SCOPE_IDENTITY();
END;
GO




IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProduct_Update')
    DROP PROCEDURE [dbo].[spProduct_Update];
GO
CREATE PROCEDURE [dbo].[spProduct_Update]
    @Id INT,
    @ProductCode NVARCHAR(50),
    @BrandId INT,
    @ProductName NVARCHAR(100),
    @ProductSKU NVARCHAR(100),
    @ProductDescription NVARCHAR(500),
    @ProductPrice DECIMAL(18, 2),
    @isProductSale BIT,
    @isProductBuy BIT,
    @TaxId INT,
	@Image nvarchar(500),
	@CategoryId int,
    @TenantId INT,
    @CompanyId INT,
    @Active bit,
	@CreatedOn datetime,
	@CreatedBy NVARCHAR(100),
	@ModifiedOn datetime,
	@ModifiedBy NVARCHAR(100)
AS
BEGIN
    UPDATE Products
    SET 
        BrandId = @BrandId,
        ProductName = @ProductName,
        ProductSKU = @ProductSKU,
        ProductDescription = @ProductDescription,
        ProductPrice = @ProductPrice,
        isProductSale = @isProductSale,
        isProductBuy = @isProductBuy,
        TaxId = @TaxId,
		Image = @Image,
		CategoryId = @CategoryId,
        TenantId = @TenantId,
        CompanyId = @CompanyId,
        ModifiedBy = @ModifiedBy,
        ModifiedOn = GETDATE()
    WHERE Id = @Id;
END;
GO





-- Drop existing procedure if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProduct_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spProduct_GetAllByTenantAndCompanyId];
GO

CREATE PROCEDURE [dbo].[spProduct_GetAllByTenantAndCompanyId]
    @TenantId INT,
    @ParentCompanyId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Select products based on TenantId and CompanyId
    SELECT 
	p.Id, 
	p.ProductCode, 
	p.BrandId,
	p.ProductName,
	p.ProductSKU,
	p.ProductDescription,
	p.ProductPrice,
    p.isProductSale,
	p.isProductBuy,
	p.TaxId,
	p.Image,
	p.CategoryId,
	p.TenantId,
	p.CompanyId,
    p.CreatedBy, 
	p.CreatedOn,
	p.ModifiedBy,
	p.ModifiedOn,
	--____ JOined Column ____
	isNull(b.BrandName,'--') as BrandName,
	isNull(tm.TaxName,'--') as TaxName,
	isNull(cat.CategoryName,'--') as CategoryName
    FROM Products p
	left join Brands b on b.Id = p.BrandId
	left join TaxMasters tm on tm.Id = p.TaxId
	left join Category cat on cat.Id = p.CategoryId
    WHERE 
        p.TenantId = @TenantId AND p.CompanyId = @ParentCompanyId;
END;
GO






--===============================================================================
-------------------------- Product + CurrentStock + Finacial Year Stockledge -------------------------------
--===============================================================================
--- Find  >>>>>>>   fnGetProductCurrentStock  ===> go to spStockLedger.sql

-- Drop existing procedure if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProduct_GetAllByTenantCompanyFinancialYearsId')
    DROP PROCEDURE [dbo].[spProduct_GetAllByTenantCompanyFinancialYearsId];
GO

CREATE PROCEDURE [dbo].[spProduct_GetAllByTenantCompanyFinancialYearsId]
    @TenantId INT,
    @ParentCompanyId INT,
    @FinancialYears INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Select products based on TenantId and CompanyId
    SELECT 
        p.Id, 
        p.ProductCode, 
        p.BrandId,
        p.ProductName,
        p.ProductSKU,
        p.ProductDescription,
        p.ProductPrice,
        p.isProductSale,
        p.isProductBuy,
        p.TaxId,
		p.Image,
		p.CategoryId,
        p.TenantId,
        p.CompanyId,
        p.CreatedBy, 
        p.CreatedOn,
        p.ModifiedBy,
        p.ModifiedOn,
        --____ JOined Column ____
		isNull(b.BrandName,'--') as BrandName,
		isNull(tm.TaxName,'--') as TaxName,
		isNull(cat.CategoryName,'--') as CategoryName,
        ISNULL(dbo.fnGetProductCurrentStock(@TenantId, @ParentCompanyId, @FinancialYears, p.Id), 0) as Stock
    FROM Products p
    LEFT JOIN Brands b ON b.Id = p.BrandId
    LEFT JOIN TaxMasters tm ON tm.Id = p.TaxId
	left join Category cat on cat.Id = p.CategoryId
    WHERE 
        p.TenantId = @TenantId 
        AND p.CompanyId = @ParentCompanyId;
END;
GO








--===========================================================================================
-------------------------- Product + CurrentStock + IsPurchase -+ FinanciyalYear Stock ledger-----------------
--===========================================================================================
--- Find  >>>>>>>   fnGetProductCurrentStock  ===> go to spStockLedger.sql

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProduct_GetAllByTenantCompanyFinancialYearsId')
    DROP PROCEDURE [dbo].[spProduct_GetAllByTenantCompanyFinancialYearsId];
GO

CREATE PROCEDURE [dbo].[spProduct_GetAllByTenantCompanyFinancialYearsId]
    @TenantId INT,
    @ParentCompanyId INT,
    @FinancialYears INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Select products based on TenantId and CompanyId
    SELECT 
        p.Id, 
        p.ProductCode, 
        p.BrandId,
        p.ProductName,
        p.ProductSKU,
        p.ProductDescription,
        p.ProductPrice,
        p.isProductSale,
        p.isProductBuy,
        p.TaxId,
		p.Image,
		p.CategoryId,
        p.TenantId,
        p.CompanyId,
        p.CreatedBy, 
        p.CreatedOn,
        p.ModifiedBy,
        p.ModifiedOn,
        --____ JOined Column ____
		isNull(b.BrandName,'--') as BrandName,
		isNull(tm.TaxName,'--') as TaxName,
		isNull(cat.CategoryName,'--') as CategoryName,
        ISNULL(dbo.fnGetProductCurrentStock(@TenantId, @ParentCompanyId, @FinancialYears, p.Id), 0) as Stock
    FROM Products p
    LEFT JOIN Brands b ON b.Id = p.BrandId
    LEFT JOIN TaxMasters tm ON tm.Id = p.TaxId
	left join Category cat on cat.Id = p.CategoryId
    WHERE 
        p.TenantId = @TenantId 
		AND p.isProductSale = 1
        AND p.CompanyId = @ParentCompanyId
END;
GO


--=========================================================================================
----------------------- Product + CurrentStock + IsSale -+ FinanciyalYearStockLedger -----------------
--=========================================================================================
--- Find  >>>>>>>   fnGetProductCurrentStock  ===> go to spStockLedger.sql

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProduct_GetAllByTenantCompanyFinancialYearsIdIsSale')
    DROP PROCEDURE [dbo].[spProduct_GetAllByTenantCompanyFinancialYearsIdIsSale];
GO
CREATE PROCEDURE [dbo].[spProduct_GetAllByTenantCompanyFinancialYearsIdIsSale]
    @TenantId INT,
    @ParentCompanyId INT,
    @FinancialYears INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.Id, 
        p.ProductCode, 
        p.BrandId,
        p.ProductName,
        p.ProductSKU,
        p.ProductDescription,
        p.ProductPrice,
        p.isProductSale,
        p.isProductBuy,
        p.TaxId,
		p.Image,
		p.CategoryId,
        p.TenantId,
        p.CompanyId,
        p.CreatedBy, 
        p.CreatedOn,
        p.ModifiedBy,
        p.ModifiedOn,
        --____ JOined Column ____
		isNull(b.BrandName,'--') as BrandName,
		isNull(tm.TaxName,'--') as TaxName,
		isNull(cat.CategoryName,'--') as CategoryName,
        ISNULL(dbo.fnGetProductCurrentStock(@TenantId, @ParentCompanyId, @FinancialYears, p.Id), 0) as Stock
    FROM Products p
    LEFT JOIN Brands b ON b.Id = p.BrandId
    LEFT JOIN TaxMasters tm ON tm.Id = p.TaxId
	left join Category cat on cat.Id = p.CategoryId
    WHERE 
        p.TenantId = @TenantId 
		AND p.isProductSale = 1
        AND p.CompanyId = @ParentCompanyId
END;
GO






