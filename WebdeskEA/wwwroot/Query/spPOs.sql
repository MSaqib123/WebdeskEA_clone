-- Create Table for POs
--GO
--IF EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'POs')
--    DROP TABLE [dbo].[POs];
--GO
--CREATE TABLE [dbo].[POs]
--(
--    Id INT IDENTITY(1,1) PRIMARY KEY,        -- Primary Key with Auto Increment
--    POCode VARCHAR(50) NOT NULL,              -- Purchase Order Code
--    PODate DATETIME NOT NULL,                 -- Purchase Order Date
--    SupplierId INT NOT NULL,                  -- Supplier Id (Assumed to be linked to a Suppliers table)
--    POSubTotal DECIMAL(18,2) NOT NULL,        -- Purchase Order Subtotal
--    PODiscount DECIMAL(18,2) NOT NULL,        -- Discount on the Purchase Order
--    POTotal DECIMAL(18,2) NOT NULL,           -- Total Amount of the Purchase Order
--    PONarration VARCHAR(255),                 -- Narration/Comments for the Purchase Order
--    TenantId INT NOT NULL,                    -- Tenant Id (multi-tenancy)
--    CompanyId INT NOT NULL,                   -- Company Id
--    Active BIT NOT NULL,                      -- Active Status (1=Active, 0=Inactive)
--    CreatedOn DATETIME NOT NULL,              -- Date and Time when the record was created
--    CreatedBy VARCHAR(50) NOT NULL,           -- Created By (User who created the record)
--    ModifiedOn DATETIME,                      -- Date and Time when the record was last modified
--    ModifiedBy VARCHAR(50)                   -- Modified By (User who last modified the record)
--);
--GO




-- Insert Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPO_Insert')
    DROP PROCEDURE [dbo].[spPO_Insert];
GO
CREATE PROCEDURE spPO_Insert
    @POCode VARCHAR(50),
    @PODate DATETIME,
    @SupplierId INT,
    @POSubTotal DECIMAL(18,2),
    @PODiscount DECIMAL(18,2),
    @POTotal DECIMAL(18,2),
	@POTotalAfterVAT DECIMAL(18,2),
    @PONarration VARCHAR(255),
    @TenantId INT,
    @CompanyId INT,
	@FinancialYearId INT,
    @Active BIT,
    @CreatedOn DATETIME,
    @CreatedBy VARCHAR(50),
    @ModifiedOn DATETIME,
    @ModifiedBy VARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
	--declare @NewCode varchar(40);
	--BEGIN
	--	EXEC dbo.GenerateCode  @TableName = 'POs', @ColumnName = 'POCode', @Prefix = 'PO',  @NewCode  = @POCode OUTPUT;
	--END


    INSERT INTO POs (POCode, PODate, SupplierId, POSubTotal, PODiscount, POTotal,POTotalAfterVAT, PONarration, TenantId, CompanyId,FinancialYearId, Active, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy)
    VALUES (@POCode, @PODate, @SupplierId, @POSubTotal, @PODiscount, @POTotal,@POTotalAfterVAT, @PONarration, @TenantId, @CompanyId,@FinancialYearId, @Active, @CreatedOn, @CreatedBy, @ModifiedOn, @ModifiedBy);

    SET @Id = SCOPE_IDENTITY();
END;






-- Update Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPO_Update')
    DROP PROCEDURE [dbo].[spPO_Update];
GO
CREATE PROCEDURE spPO_Update
    @Id INT,
    @POCode VARCHAR(50),
    @PODate DATETIME,
    @SupplierId INT,
    @POSubTotal DECIMAL(18,2),
    @PODiscount DECIMAL(18,2),
    @POTotal DECIMAL(18,2),
	@POTotalAfterVAT DECIMAL(18,2),
    @PONarration VARCHAR(255),
    @TenantId INT,
    @CompanyId INT,
	@FinancialYearId int,
    @Active BIT,
	@CreatedOn DATETIME,
    @CreatedBy VARCHAR(50),
    @ModifiedOn DATETIME,
    @ModifiedBy VARCHAR(50)

AS
BEGIN
    UPDATE POs
    SET
        POCode = @POCode,
        PODate = @PODate,
        SupplierId = @SupplierId,
        POSubTotal = @POSubTotal,
        PODiscount = @PODiscount,
        POTotal = @POTotal,
		POTotalAfterVAT = @POTotalAfterVAT,
        PONarration = @PONarration,
        TenantId = @TenantId,
        CompanyId = @CompanyId,
		FinancialYearId=@FinancialYearId,
        Active = @Active,
        ModifiedOn = @ModifiedOn,
        ModifiedBy = @ModifiedBy
    WHERE Id = @Id;
END;







-- Delete Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPO_Delete')
    DROP PROCEDURE [dbo].[spPO_Delete];
GO
CREATE PROCEDURE spPO_Delete
    @Id INT
AS
BEGIN
    DELETE FROM POs
    WHERE Id = @Id;
END;

-- Get All Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPO_GetAll')
    DROP PROCEDURE [dbo].[spPO_GetAll];
GO
CREATE PROCEDURE spPO_GetAll
AS
BEGIN
    SELECT 
	po.Id	
	,po.POCode	
	,po.PODate	
	,po.SupplierId	
	,po.POSubTotal	
	,po.PODiscount	
	,po.POTotal	
	,po.POTotalAfterVAT
	,po.PONarration	
	,po.TenantId	
	,po.CompanyId	
	,po.FinancialYearId
	,po.Active	
	,po.CreatedOn	
	,po.CreatedBy	
	,po.ModifiedOn	
	,po.ModifiedBy
	-- Joined Column ---
	,COALESCE( s.Name ,'--')as SupplierName
	,CASE WHEN EXISTS (SELECT 1 FROM PIs WHERE PIs.POId = po.Id) THEN 1  ELSE 0  END AS IsPIExist
    FROM POs po
	left Join Suppliers s on s.id = po.SupplierId
	inner JOIN
		Tenants t ON t.Id = po.TenantId
	inner JOIN
		Companies com ON com.Id  = po.CompanyId
END;




-- Get By Id Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPO_GetById')
    DROP PROCEDURE [dbo].[spPO_GetById];
GO
CREATE PROCEDURE spPO_GetById
    @Id INT
AS
BEGIN
    SELECT 
	po.Id	
	,po.POCode	
	,po.PODate	
	,po.SupplierId	
	,po.POSubTotal	
	,po.PODiscount	
	,po.POTotal	
	,po.POTotalAfterVAT
	,po.PONarration	
	,po.TenantId	
	,po.CompanyId	
	,po.FinancialYearId
	,po.Active	
	,po.CreatedOn	
	,po.CreatedBy	
	,po.ModifiedOn	
	,po.ModifiedBy
	-- Joined Column ---
	,COALESCE( s.Name ,'--')as SupplierName
	,CASE WHEN EXISTS (SELECT 1 FROM PIs WHERE PIs.POId = po.Id) THEN 1  ELSE 0  END AS IsPIExist
    FROM POs po
	left Join Suppliers s on s.id = po.SupplierId
	inner JOIN
		Tenants t ON t.Id = po.TenantId
	inner JOIN
		Companies com ON com.Id  = po.CompanyId
    WHERE po.Id = @Id;
END;






Go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSO_GetByTenant')
    DROP PROCEDURE [dbo].[spSO_GetByTenant];
GO
CREATE PROCEDURE spSO_GetByTenant
    @TenantId INT
AS
BEGIN
     SELECT 
	po.Id	
	,po.POCode	
	,po.PODate	
	,po.SupplierId	
	,po.POSubTotal	
	,po.PODiscount	
	,po.POTotal	
	,po.POTotalAfterVAT
	,po.PONarration	
	,po.TenantId	
	,po.CompanyId	
	,po.FinancialYearId
	,po.Active	
	,po.CreatedOn	
	,po.CreatedBy	
	,po.ModifiedOn	
	,po.ModifiedBy
	-- Joined Column ---
	,COALESCE( s.Name ,'--')as SupplierName
	,CASE WHEN EXISTS (SELECT 1 FROM PIs WHERE PIs.POId = po.Id) THEN 1  ELSE 0  END AS IsPIExist
    FROM POs po
	left Join Suppliers s on s.id = po.SupplierId
	inner JOIN
		Tenants t ON t.Id = po.TenantId
	inner JOIN
		Companies com ON com.Id  = po.CompanyId
    WHERE po.TenantId = @TenantId;
END;





Go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPO_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spPO_GetAllByTenantAndCompanyId];
GO
CREATE PROCEDURE spPO_GetAllByTenantAndCompanyId
@TenantId int ,
@ParentCompanyId int
as
begin

	SELECT 
	po.Id	
	,po.POCode	
	,po.PODate	
	,po.SupplierId	
	,po.POSubTotal	
	,po.PODiscount	
	,po.POTotal	
	,po.POTotalAfterVAT
	,po.PONarration	
	,po.TenantId	
	,po.CompanyId	
	,po.FinancialYearId
	,po.Active	
	,po.CreatedOn	
	,po.CreatedBy	
	,po.ModifiedOn	
	,po.ModifiedBy
	-- Joined Column ---
	,COALESCE( s.Name ,'--') as SupplierName
	,CASE WHEN EXISTS (SELECT 1 FROM PIs WHERE PIs.POId = po.Id) THEN 1  ELSE 0  END AS IsPIExist
    FROM POs po
	left Join Suppliers s on s.id = po.SupplierId
	inner JOIN
		Tenants t ON t.Id = po.TenantId
	inner JOIN
		Companies com ON com.Id  = po.CompanyId
	WHERE
		po.TenantId = @TenantId
		AND po.CompanyId = @ParentCompanyId;
end




Go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPO_GetAllByTenantAndCompanyFinancialYearId')
    DROP PROCEDURE [dbo].[spPO_GetAllByTenantAndCompanyFinancialYearId];
GO
CREATE PROCEDURE spPO_GetAllByTenantAndCompanyFinancialYearId
@TenantId int ,
@ParentCompanyId int,
@FinancialYearId int
as
begin

	SELECT 
	po.Id	
	,po.POCode	
	,po.PODate	
	,po.SupplierId	
	,po.POSubTotal	
	,po.PODiscount	
	,po.POTotal	
	,po.POTotalAfterVAT
	,po.PONarration	
	,po.TenantId	
	,po.CompanyId	
	,po.FinancialYearId
	,po.Active	
	,po.CreatedOn	
	,po.CreatedBy	
	,po.ModifiedOn	
	,po.ModifiedBy
	-- Joined Column ---
	,COALESCE( s.Name ,'--') as SupplierName
	,CASE WHEN EXISTS (SELECT 1 FROM PIs WHERE PIs.POId = po.Id) THEN 1  ELSE 0  END AS IsPIExist
    FROM POs po
	left Join Suppliers s on s.id = po.SupplierId
	inner JOIN
		Tenants t ON t.Id = po.TenantId
	inner JOIN
		Companies com ON com.Id  = po.CompanyId
	WHERE
		po.TenantId = @TenantId
		AND po.CompanyId = @ParentCompanyId
		AND po.FinancialYearId = @FinancialYearId
end




Go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPO_GetAllNotInUsedByTenantCompanyId')
    DROP PROCEDURE [dbo].[spPO_GetAllNotInUsedByTenantCompanyId];
GO

CREATE PROCEDURE spPO_GetAllNotInUsedByTenantCompanyId
    @TenantId INT,
    @ParentCompanyId INT,
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        po.Id,
        po.POCode,
        po.PODate,
        po.SupplierId,
        po.POSubTotal,
        po.PODiscount,
        po.POTotal,
		po.POTotalAfterVAT,
        po.PONarration,
        po.TenantId,
        po.CompanyId,
		po.FinancialYearId,
        po.Active,
        po.CreatedOn,
        po.CreatedBy,
        po.ModifiedOn,
        po.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
		,CASE WHEN EXISTS (SELECT 1 FROM PIs WHERE PIs.POId = po.Id) THEN 1  ELSE 0  END AS IsPIExist
    FROM POs po
    LEFT JOIN Suppliers s ON s.Id = po.SupplierId
    INNER JOIN Tenants t ON t.Id = po.TenantId
    INNER JOIN Companies com ON com.Id = po.CompanyId
    WHERE po.TenantId = @TenantId
      AND po.CompanyId = @ParentCompanyId
      AND (
            (@Id IS NOT NULL AND po.Id = @Id) 
            OR NOT EXISTS (SELECT 1 FROM PIs WHERE PIs.POId = po.Id)
          );
END;
GO
