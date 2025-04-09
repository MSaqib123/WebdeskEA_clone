--===========================================
-- Step 1: Drop the vwOS View if it Exists
--===========================================
IF EXISTS (
    SELECT * FROM sys.views 
    WHERE name = 'vwOS' 
    AND schema_id = SCHEMA_ID('dbo')
)
    DROP VIEW [dbo].[vwOS];
GO

--===============================================================================
-- Step 2: Create vwOS View
--===============================================================================
CREATE VIEW vwOS AS
SELECT 
    os.Id, -- OS record identifier
    os.OSCode, -- OS code for the transaction
    os.OSDate, -- OS date of the transaction
    os.TenantId, -- Tenant identifier
    os.CompanyId, -- Company identifier
    os.FinancialYearId, -- Financial year identifier
    os.CreatedOn, -- Record creation date
    os.CreatedBy, -- Record created by user
    os.Active, -- Active status flag
    osd.Id AS osDtlId, -- Detail identifier
    osd.ProductId, -- Product identifier
    osd.OSDtlQty -- Quantity of product in detail
FROM OSs os
INNER JOIN OSDtls osd 
    ON osd.OSId = os.Id; -- Joining OS with its details
GO

--===============================================================================
-- Step 3: Drop spGetProductCurrentStock Procedure if it Exists
--===============================================================================
IF EXISTS (
    SELECT * FROM sys.objects 
    WHERE type = 'P' 
    AND name = 'spGetProductCurrentStock'
)
    DROP PROCEDURE [dbo].[spGetProductCurrentStock];
GO

--===============================================================================
-- Step 4: Create spGetProductCurrentStock Procedure
--===============================================================================
CREATE proc spGetProductCurrentStock  
@TenantId int,  
@CompanyId int,  
@FinancialYearId int,  
@ProductId int  
as  
--declare @maxDate date;  
--declare @TenantId int = 3;  
--declare @CompanyId int = 33;  
--declare @FinancialYearId int = 3;  
--declare @ProductId int = 2;  
--spGetProductCurrentStock 3,33,3,2  
  
with cte as(  
select distinct sum(osDtlQty) as SumProdQty,sl.osDate,sl.ProductId from vwOS sl  
where ProductId = @ProductId and TenantId = @TenantId and CompanyId = @CompanyId and FinancialYearId = @FinancialYearId and  
sl.Id = (Select top 1 Id from vwOS where ProductId = @ProductId and TenantId = @TenantId and CompanyId = @CompanyId order by Id desc)  
group by sl.osDate,sl.ProductId  
)  
select distinct isnull(sum(sl.StockIn),0) +   
isnull(sum(cte.SumProdQty),0) - sum(isnull(sl.StockOut,0)) as CurrentStock   
from products prod   
left join (select distinct  SumProdQty,osDate,ProductId from cte) cte on prod.Id = cte.productId  
left join (select sum(StockIn) as StockIn,sum(StockOut) as StockOut,ProductId,max(CreatedOn) as CreatedOn from StockLedger  
where FinancialYearId = @FinancialYearId    
and TenantId = @TenantId and CompanyId = @CompanyId   
group by ProductId--,FinancialYearId,CompanyId,TenantId,CreatedOn  
) sl on   prod.Id = sl.ProductId and  (cte.OSDate IS NULL OR sl.CreatedOn >= cte.OSDate)  
where Prod.id = @ProductId and prod.TenantId = @TenantId and prod.CompanyId = @CompanyId   


 go


--===============================================================================
-- Step 4: Create spGetProductCurrentStock Function
--===============================================================================
CREATE FUNCTION dbo.fnGetProductCurrentStock (
    @TenantId INT, -- Tenant parameter
    @CompanyId INT, -- Company parameter
    @FinancialYearId INT, -- Financial year parameter
    @ProductId INT -- Product parameter
)
RETURNS int -- You can adjust the precision and scale if needed
AS
BEGIN
    -- Declare the variable to hold the calculated stock value
    DECLARE @CurrentStock DECIMAL(18, 2);

    -- Step 4a: Define a Common Table Expression (CTE) for Product Quantity
    WITH cte AS (
        SELECT 
            SUM(osDtlQty) AS SumProdQty, -- Summing the product quantity
            sl.OSDate, -- OS date
            sl.ProductId -- Product identifier
        FROM vwOS sl
        WHERE 
            sl.ProductId = @ProductId 
            AND sl.TenantId = @TenantId 
            AND sl.CompanyId = @CompanyId 
            AND sl.FinancialYearId = @FinancialYearId
            -- Fetching the most recent OS entry for the product
            AND sl.Id = (
                SELECT TOP 1 Id 
                FROM vwOS 
                WHERE ProductId = @ProductId 
                AND TenantId = @TenantId 
                AND CompanyId = @CompanyId 
                ORDER BY Id DESC
            )
        GROUP BY sl.OSDate, sl.ProductId
    )

    -- Step 4b: Calculate the Current Stock
    select distinct isnull(sum(sl.StockIn),0) +   
    isnull(sum(cte.SumProdQty),0) - sum(isnull(sl.StockOut,0)) as CurrentStock   
    from products prod   
    LEFT JOIN cte 
        ON prod.Id = cte.ProductId
    LEFT JOIN StockLedger sl 
        ON prod.Id = sl.ProductId 
        AND sl.FinancialYearId = @FinancialYearId
        AND sl.TenantId = @TenantId
        AND sl.CompanyId = @CompanyId
        -- Considering only transactions after the latest OS date
        AND (cte.OSDate IS NULL OR sl.CreatedOn >= cte.OSDate)
    WHERE 
        prod.Id = @ProductId 
        AND prod.TenantId = @TenantId 
        AND prod.CompanyId = @CompanyId;

    -- Return the calculated stock value
    RETURN ISNULL(@CurrentStock, 0);
END
GO











IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spStockLedger_Insert')
    DROP PROCEDURE [dbo].[spStockLedger_Insert];
GO
CREATE PROCEDURE [dbo].[spStockLedger_Insert]
    @ProductId INT,
    @InvoiceType VARCHAR(50),
    @InvoiceCode VARCHAR(50),
    @StockIn DECIMAL(18,2),
    @StockOut DECIMAL(18,2),
    @TenantId INT,
    @CompanyId INT,
    @FinancialYearId INT,
    @CreatedOn DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- If you have a code generation logic similar to OBCode:
    -- EXEC dbo.GenerateCode @TableName='StockLedger', @ColumnName='InvoiceCode', @Prefix='INV', @NewCode=@InvoiceCode OUTPUT;

    INSERT INTO [StockLedger]
        ([ProductId], [InvoiceType], [InvoiceCode], [StockIn], [StockOut], [TenantId], [CompanyId], [FinancialYearId], [CreatedOn])
    VALUES
        (@ProductId, @InvoiceType, @InvoiceCode, @StockIn, @StockOut, @TenantId, @CompanyId, @FinancialYearId, @CreatedOn);

    SET @Id = SCOPE_IDENTITY();
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spStockLedger_Update')
    DROP PROCEDURE [dbo].[spStockLedger_Update];
GO
CREATE PROCEDURE [dbo].[spStockLedger_Update]
    @Id INT,
    @ProductId INT,
    @InvoiceType VARCHAR(50),
    @InvoiceCode VARCHAR(50),
    @StockIn DECIMAL(18,2),
    @StockOut DECIMAL(18,2),
    @TenantId INT,
    @CompanyId INT,
    @FinancialYearId INT,
    @CreatedOn DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [StockLedger]
    SET 
        [ProductId] = @ProductId,
        [InvoiceType] = @InvoiceType,
        [InvoiceCode] = @InvoiceCode,
        [StockIn] = @StockIn,
        [StockOut] = @StockOut,
        [TenantId] = @TenantId,
        [CompanyId] = @CompanyId,
        [FinancialYearId] = @FinancialYearId,
        [CreatedOn] = @CreatedOn
    WHERE [Id] = @Id;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spStockLedger_Delete')
    DROP PROCEDURE [dbo].[spStockLedger_Delete];
GO
CREATE PROCEDURE [dbo].[spStockLedger_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [StockLedger]
    WHERE [Id] = @Id;
END;
GO





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spStockLedger_GetAll')
    DROP PROCEDURE [dbo].[spStockLedger_GetAll];
GO
CREATE PROCEDURE [dbo].[spStockLedger_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        sl.[Id],
        sl.[ProductId],
        sl.[InvoiceType],
        sl.[InvoiceCode],
        sl.[StockIn],
        sl.[StockOut],
        sl.[TenantId],
        sl.[CompanyId],
        sl.[FinancialYearId],
        sl.[CreatedOn]
    FROM [StockLedger] sl
    INNER JOIN [Tenants] t ON t.[Id] = sl.[TenantId]
    INNER JOIN [Companies] c ON c.[Id] = sl.[CompanyId];
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spStockLedger_GetById')
    DROP PROCEDURE [dbo].[spStockLedger_GetById];
GO
CREATE PROCEDURE [dbo].[spStockLedger_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        sl.[Id],
        sl.[ProductId],
        sl.[InvoiceType],
        sl.[InvoiceCode],
        sl.[StockIn],
        sl.[StockOut],
        sl.[TenantId],
        sl.[CompanyId],
        sl.[FinancialYearId],
        sl.[CreatedOn]
    FROM [StockLedger] sl
    INNER JOIN [Tenants] t ON t.[Id] = sl.[TenantId]
    INNER JOIN [Companies] c ON c.[Id] = sl.[CompanyId]
    WHERE sl.[Id] = @Id;
END;
GO













GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spStockLedger_GetByTenant')
    DROP PROCEDURE [dbo].[spStockLedger_GetByTenant];
GO
CREATE PROCEDURE [dbo].[spStockLedger_GetByTenant]
    @TenantId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        sl.[Id],
        sl.[ProductId],
        sl.[InvoiceType],
        sl.[InvoiceCode],
        sl.[StockIn],
        sl.[StockOut],
        sl.[TenantId],
        sl.[CompanyId],
        sl.[FinancialYearId],
        sl.[CreatedOn]
    FROM [StockLedger] sl
    INNER JOIN [Tenants] t ON t.[Id] = sl.[TenantId]
    INNER JOIN [Companies] c ON c.[Id] = sl.[CompanyId]
    WHERE sl.[TenantId] = @TenantId;
END;
GO









GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spStockLedger_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spStockLedger_GetAllByTenantAndCompanyId];
GO
CREATE PROCEDURE [dbo].[spStockLedger_GetAllByTenantAndCompanyId]
    @TenantId INT,
    @ParentCompanyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        sl.[Id],
        sl.[ProductId],
        sl.[InvoiceType],
        sl.[InvoiceCode],
        sl.[StockIn],
        sl.[StockOut],
        sl.[TenantId],
        sl.[CompanyId],
        sl.[FinancialYearId],
        sl.[CreatedOn]
    FROM [StockLedger] sl
    INNER JOIN [Tenants] t ON t.[Id] = sl.[TenantId]
    INNER JOIN [Companies] com ON com.[Id] = sl.[CompanyId]
    WHERE sl.[TenantId] = @TenantId
      AND sl.[CompanyId] = @ParentCompanyId;
END;
GO








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spStockLedger_GetAllByTenantAndCompanyFinancialYearId')
    DROP PROCEDURE [dbo].[spStockLedger_GetAllByTenantAndCompanyFinancialYearId];
GO
CREATE PROCEDURE [dbo].[spStockLedger_GetAllByTenantAndCompanyFinancialYearId]
    @TenantId INT,
    @ParentCompanyId INT,
    @FinancialYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        sl.[Id],
        sl.[ProductId],
        sl.[InvoiceType],
        sl.[InvoiceCode],
        sl.[StockIn],
        sl.[StockOut],
        sl.[TenantId],
        sl.[CompanyId],
        sl.[FinancialYearId],
        sl.[CreatedOn]
    FROM [StockLedger] sl
    INNER JOIN [Tenants] t ON t.[Id] = sl.[TenantId]
    INNER JOIN [Companies] com ON com.[Id] = sl.[CompanyId]
    WHERE sl.[TenantId] = @TenantId
      AND sl.[CompanyId] = @ParentCompanyId
      AND sl.[FinancialYearId] = @FinancialYearId;
END;
GO


GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spStockLedger_DeleteByInvoiceCode')
    DROP PROCEDURE [dbo].[spStockLedger_DeleteByInvoiceCode];
GO
CREATE PROCEDURE [dbo].[spStockLedger_DeleteByInvoiceCode]
    @TenantId INT,
	@CompanyId INT,
	@InvoiceCode varchar(500)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [StockLedger]
    WHERE InvoiceCode = @InvoiceCode and TenantId = @TenantId and CompanyId = @CompanyId;
END;

