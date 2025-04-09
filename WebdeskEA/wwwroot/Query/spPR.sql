--CREATE TABLE [dbo].[PRs](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[PRCode] [nvarchar](50) NOT NULL,
--	[PRDate] [datetime] NOT NULL,
--	[PIId] [int] NULL,
--	[SupplierId] [int] NOT NULL,
--	[PRSubTotal] [decimal](18, 2) NOT NULL,
--	[PRDiscount] [decimal](18, 2) NOT NULL,
--	[PRTotal] [decimal](18, 2) NOT NULL,
--	[TenantId] [int] NOT NULL,
--	[CompanyId] [int] NOT NULL,
--	[FinancialYearId] [int] NULL,
--	[Active] [bit] NOT NULL,
--	[CreatedOn] [datetime] NOT NULL,
--	[CreatedBy] [varchar](50) NOT NULL,
--	[ModifiedOn] [datetime] NULL,
--	[ModifiedBy] [varchar](50) NULL
--)
--GO









GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPR_Insert')
    DROP PROCEDURE [dbo].[spPR_Insert];
GO
CREATE PROCEDURE spPR_Insert
    @PRCode NVARCHAR(50),
    @PRDate DATETIME,
    @PIId INT,
    @SupplierId INT,
    @PRSubTotal DECIMAL(18, 2),
    @PRDiscount DECIMAL(18, 2),
    @PRTotal DECIMAL(18, 2),
	@PRTotalAfterVAT DECIMAL(18, 2),
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
    
    INSERT INTO PRs (PRCode, PRDate, PIId, SupplierId, PRSubTotal, PRDiscount, PRTotal,PRTotalAfterVAT, TenantId, CompanyId,FinancialYearId, Active, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy)
    VALUES (@PRCode, @PRDate, @PIId, @SupplierId, @PRSubTotal, @PRDiscount, @PRTotal,@PRTotalAfterVAT, @TenantId, @CompanyId,@FinancialYearId, @Active, @CreatedOn, @CreatedBy, @ModifiedOn, @ModifiedBy);

    SET @Id = SCOPE_IDENTITY();
END;









GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPR_Update')
    DROP PROCEDURE [dbo].[spPR_Update];
GO
CREATE PROCEDURE spPR_Update
    @Id INT,
    @PRCode NVARCHAR(50),
    @PRDate DATETIME,
    @PIId INT,
    @SupplierId INT,
    @PRSubTotal DECIMAL(18, 2),
    @PRDiscount DECIMAL(18, 2),
    @PRTotal DECIMAL(18, 2),
	@PRTotalAfterVAT DECIMAL(18, 2),
    @TenantId INT,
    @CompanyId INT,
	@FinancialYearId INT,
    @Active BIT,
    @CreatedOn DATETIME,
    @CreatedBy VARCHAR(50),
    @ModifiedOn DATETIME,
    @ModifiedBy VARCHAR(50)
AS
BEGIN
    UPDATE PRs
    SET
        PRCode = @PRCode,
        PRDate = @PRDate,
        PIId = @PIId,
        SupplierId = @SupplierId,
        PRSubTotal = @PRSubTotal,
        PRDiscount = @PRDiscount,
        PRTotal = @PRTotal,
		PRTotalAfterVAT = @PRTotalAfterVAT,
        TenantId = @TenantId,
        CompanyId = @CompanyId,
		FinancialYearId = @FinancialYearId,
        Active = @Active,
        ModifiedOn = @ModifiedOn,
        ModifiedBy = @ModifiedBy
    WHERE Id = @Id;
END;





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPR_Delete')
    DROP PROCEDURE [dbo].[spPR_Delete];
GO
CREATE PROCEDURE spPR_Delete
    @Id INT
AS
BEGIN
    DELETE FROM PRs
    WHERE Id = @Id;
END;




GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPR_GetAll')
    DROP PROCEDURE [dbo].[spPR_GetAll];
GO
CREATE PROCEDURE spPR_GetAll
AS
BEGIN
    SELECT 
        PR.Id,
        PR.PRCode,
        PR.PRDate,
        PR.PIId,
        PR.SupplierId,
        PR.PRSubTotal,
        PR.PRDiscount,
        PR.PRTotal,
        PR.TenantId,
        PR.CompanyId,
		PR.FinancialYearId,
		PR.PRTotalAfterVAT,
        PR.Active,
        PR.CreatedOn,
        PR.CreatedBy,
        PR.ModifiedOn,
        PR.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
    FROM PRs PR
    LEFT JOIN Suppliers s ON s.Id = PR.SupplierId;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPR_GetById')
    DROP PROCEDURE [dbo].[spPR_GetById];
GO
CREATE PROCEDURE spPR_GetById
    @Id INT
AS
BEGIN
    SELECT 
        PR.Id,
        PR.PRCode,
        PR.PRDate,
        PR.PIId,
        PR.SupplierId,
        PR.PRSubTotal,
        PR.PRDiscount,
        PR.PRTotal,
        PR.TenantId,
        PR.CompanyId,
		PR.FinancialYearId,
		PR.PRTotalAfterVAT,
        PR.Active,
        PR.CreatedOn,
        PR.CreatedBy,
        PR.ModifiedOn,
        PR.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
    FROM PRs PR
    LEFT JOIN Suppliers s ON s.Id = PR.SupplierId
    WHERE PR.Id = @Id;
END;




GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPR_GetByPIId')
    DROP PROCEDURE [dbo].[spPR_GetByPIId];
GO
CREATE PROCEDURE spPR_GetByPIId
@Id INT
AS
BEGIN
    SELECT 
	top 1
       PR.Id,
        PR.PRCode,
        PR.PRDate,
        PR.PIId,
        PR.SupplierId,
        PR.PRSubTotal,
        PR.PRDiscount,
        PR.PRTotal,
        PR.TenantId,
        PR.CompanyId,
		PR.FinancialYearId,
		PR.PRTotalAfterVAT,
        PR.Active,
        PR.CreatedOn,
        PR.CreatedBy,
        PR.ModifiedOn,
        PR.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
    FROM PRs PR
    LEFT JOIN Suppliers s ON s.Id = PR.SupplierId
	where PR.PIId = @Id
END;




GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPR_GetByTenant')
    DROP PROCEDURE [dbo].[spPR_GetByTenant];
GO
CREATE PROCEDURE spPR_GetByTenant
    @TenantId INT
AS
BEGIN
    SELECT 
        PR.Id,
        PR.PRCode,
        PR.PRDate,
        PR.PIId,
        PR.SupplierId,
        PR.PRSubTotal,
        PR.PRDiscount,
        PR.PRTotal,
        PR.TenantId,
        PR.CompanyId,
		PR.FinancialYearId,
		PR.PRTotalAfterVAT,
        PR.Active,
        PR.CreatedOn,
        PR.CreatedBy,
        PR.ModifiedOn,
        PR.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
    FROM PRs PR
    LEFT JOIN Suppliers s ON s.Id = PR.SupplierId
    WHERE PR.TenantId = @TenantId;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPR_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spPR_GetAllByTenantAndCompanyId];
GO
CREATE PROCEDURE spPR_GetAllByTenantAndCompanyId
    @TenantId INT,
    @ParentCompanyId INT
AS
BEGIN
    SELECT 
        PR.Id,
        PR.PRCode,
        PR.PRDate,
        PR.PIId,
        PR.SupplierId,
        PR.PRSubTotal,
        PR.PRDiscount,
        PR.PRTotal,
        PR.TenantId,
        PR.CompanyId,
		PR.FinancialYearId,
		PR.PRTotalAfterVAT,
        PR.Active,
        PR.CreatedOn,
        PR.CreatedBy,
        PR.ModifiedOn,
        PR.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
    FROM PRs PR
    LEFT JOIN Suppliers s ON s.Id = PR.SupplierId
    WHERE PR.TenantId = @TenantId AND PR.CompanyId = @ParentCompanyId;
END;





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPR_GetAllByTenantAndCompanyFinancialYearId')
    DROP PROCEDURE [dbo].[spPR_GetAllByTenantAndCompanyFinancialYearId];
GO
CREATE PROCEDURE spPR_GetAllByTenantAndCompanyFinancialYearId
    @TenantId INT,
    @ParentCompanyId INT,
	@FinancialYearId Int
AS
BEGIN
    SELECT 
        PR.Id,
        PR.PRCode,
        PR.PRDate,
        PR.PIId,
        PR.SupplierId,
        PR.PRSubTotal,
        PR.PRDiscount,
        PR.PRTotal,
        PR.TenantId,
        PR.CompanyId,
		PR.FinancialYearId,
		PR.PRTotalAfterVAT,
        PR.Active,
        PR.CreatedOn,
        PR.CreatedBy,
        PR.ModifiedOn,
        PR.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
    FROM PRs PR
    LEFT JOIN Suppliers s ON s.Id = PR.SupplierId
    WHERE PR.TenantId = @TenantId AND PR.CompanyId = @ParentCompanyId AND PR.FinancialYearId = @FinancialYearId
END;
