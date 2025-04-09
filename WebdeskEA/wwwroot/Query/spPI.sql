
--CREATE TABLE [dbo].[PI](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[PICode] [nvarchar](50) NOT NULL,
--	[PIDate] [datetime] NOT NULL,
--	[POId] [int] NULL,
--	[SupplierId] [int] NOT NULL,
--	[PISubTotal] [decimal](18, 2) NOT NULL,
--	[PIDiscount] [decimal](18, 2) NOT NULL,
--	[PITotal] [decimal](18, 2) NOT NULL,
--	[PITotalAfterVAT] [decimal](18, 2) NOT NULL,
--	[TenantId] [int] NOT NULL,
--	[CompanyId] [int] NOT NULL,
--	[Active] [bit] NOT NULL,
--	[CreatedOn] [datetime] NOT NULL,
--	[CreatedBy] [varchar](50) NOT NULL,
--	[ModifiedOn] [datetime] NULL,
--	[ModifiedBy] [varchar](50) NULL
-- )



GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPI_Insert')
    DROP PROCEDURE [dbo].[spPI_Insert];
GO
CREATE PROCEDURE spPI_Insert
    @PICode NVARCHAR(50),
    @PIDate DATETIME,
    @POId INT,
    @SupplierId INT,
    @PISubTotal DECIMAL(18, 2),
    @PIDiscount DECIMAL(18, 2),
    @PITotal DECIMAL(18, 2),
	@PITotalAfterVAT DECIMAL(18, 2),
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
    --DECLARE @NewCode NVARCHAR(50);
    --EXEC dbo.GenerateCode @TableName = 'PI', @ColumnName = 'PICode', @Prefix = 'PI', @NewCode = @PICode OUTPUT;

    INSERT INTO PIs (PICode, PIDate, POId, SupplierId, PISubTotal, PIDiscount, PITotal,PITotalAfterVAT, TenantId, CompanyId,FinancialYearId, Active, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy)
    VALUES (@PICode, @PIDate, @POId, @SupplierId, @PISubTotal, @PIDiscount, @PITotal,@PITotalAfterVAT, @TenantId, @CompanyId,@FinancialYearId, @Active, @CreatedOn, @CreatedBy, @ModifiedOn, @ModifiedBy);

    SET @Id = SCOPE_IDENTITY();
END;









GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPI_Update')
    DROP PROCEDURE [dbo].[spPI_Update];
GO
CREATE PROCEDURE spPI_Update
    @Id INT,
    @PICode NVARCHAR(50),
    @PIDate DATETIME,
    @POId INT,
    @SupplierId INT,
    @PISubTotal DECIMAL(18, 2),
    @PIDiscount DECIMAL(18, 2),
    @PITotal DECIMAL(18, 2),
	@PITotalAfterVAT DECIMAL(18, 2),
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
    UPDATE PIs
    SET
        PICode = @PICode,
        PIDate = @PIDate,
        POId = @POId,
        SupplierId = @SupplierId,
        PISubTotal = @PISubTotal,
        PIDiscount = @PIDiscount,
        PITotal = @PITotal,
		PITotalAfterVAT= @PITotalAfterVAT,
        TenantId = @TenantId,
        CompanyId = @CompanyId,
		FinancialYearId = @FinancialYearId,
        Active = @Active,
        ModifiedOn = @ModifiedOn,
        ModifiedBy = @ModifiedBy
    WHERE Id = @Id;
END;





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPI_Delete')
    DROP PROCEDURE [dbo].[spPI_Delete];
GO
CREATE PROCEDURE spPI_Delete
    @Id INT
AS
BEGIN
    DELETE FROM PIs
    WHERE Id = @Id;
END;




GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPI_GetAll')
    DROP PROCEDURE [dbo].[spPI_GetAll];
GO
CREATE PROCEDURE spPI_GetAll
AS
BEGIN
    SELECT 
        pi.Id,
        pi.PICode,
        pi.PIDate,
        pi.POId,
        pi.SupplierId,
        pi.PISubTotal,
        pi.PIDiscount,
        pi.PITotal,
        pi.TenantId,
        pi.CompanyId,
		pi.FinancialYearId,
		pi.PITotalAfterVAT,
        pi.Active,
        pi.CreatedOn,
        pi.CreatedBy,
        pi.ModifiedOn,
        pi.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
		,CASE WHEN EXISTS (SELECT 1 FROM PRs WHERE PRs.PIId = pi.Id) THEN 1  ELSE 0  END AS IsReturnExist  
    FROM PIs pi
    LEFT JOIN Suppliers s ON s.Id = pi.SupplierId;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPI_GetById')
    DROP PROCEDURE [dbo].[spPI_GetById];
GO
CREATE PROCEDURE spPI_GetById
    @Id INT
AS
BEGIN
    SELECT 
        pi.Id,
        pi.PICode,
        pi.PIDate,
        pi.POId,
        pi.SupplierId,
        pi.PISubTotal,
        pi.PIDiscount,
        pi.PITotal,
		pi.PITotalAfterVAT,
        pi.TenantId,
        pi.CompanyId,
		pi.FinancialYearId,
        pi.Active,
        pi.CreatedOn,
        pi.CreatedBy,
        pi.ModifiedOn,
        pi.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
		,CASE WHEN EXISTS (SELECT 1 FROM PRs WHERE PRs.PIId = pi.Id) THEN 1  ELSE 0  END AS IsReturnExist  
    FROM PIs pi
    LEFT JOIN Suppliers s ON s.Id = pi.SupplierId
    WHERE pi.Id = @Id;
END;



GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPI_GetByPOId')
    DROP PROCEDURE [dbo].[spPI_GetByPOId];
GO
CREATE PROCEDURE spPI_GetByPOId
@Id INT
AS
BEGIN
    SELECT 
	top 1
       pi.Id,
        pi.PICode,
        pi.PIDate,
        pi.POId,
        pi.SupplierId,
        pi.PISubTotal,
        pi.PIDiscount,
        pi.PITotal,
		pi.PITotalAfterVAT,
        pi.TenantId,
        pi.CompanyId,
		pi.FinancialYearId,
        pi.Active,
        pi.CreatedOn,
        pi.CreatedBy,
        pi.ModifiedOn,
        pi.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
		,CASE WHEN EXISTS (SELECT 1 FROM PRs WHERE PRs.PIId = pi.Id) THEN 1  ELSE 0  END AS IsReturnExist  
    FROM PIs pi
    LEFT JOIN Suppliers s ON s.Id = pi.SupplierId
	where pi.POId = @Id
END;





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPI_GetByTenant')
    DROP PROCEDURE [dbo].[spPI_GetByTenant];
GO
CREATE PROCEDURE spPI_GetByTenant
    @TenantId INT
AS
BEGIN
    SELECT 
        pi.Id,
        pi.PICode,
        pi.PIDate,
        pi.POId,
        pi.SupplierId,
        pi.PISubTotal,
        pi.PIDiscount,
        pi.PITotal,
		pi.PITotalAfterVAT,
        pi.TenantId,
        pi.CompanyId,
		pi.FinancialYearId,
        pi.Active,
        pi.CreatedOn,
        pi.CreatedBy,
        pi.ModifiedOn,
        pi.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
		,CASE WHEN EXISTS (SELECT 1 FROM PRs WHERE PRs.PIId = pi.Id) THEN 1  ELSE 0  END AS IsReturnExist  
    FROM PIs pi
    LEFT JOIN Suppliers s ON s.Id = pi.SupplierId
    WHERE pi.TenantId = @TenantId;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPI_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spPI_GetAllByTenantAndCompanyId];
GO
CREATE PROCEDURE spPI_GetAllByTenantAndCompanyId
    @TenantId INT,
    @ParentCompanyId INT
AS
BEGIN
    SELECT 
        pi.Id,
        pi.PICode,
        pi.PIDate,
        pi.POId,
        pi.SupplierId,
        pi.PISubTotal,
        pi.PIDiscount,
        pi.PITotal,
		pi.PITotalAfterVAT,
        pi.TenantId,
        pi.CompanyId,
		pi.FinancialYearId,
        pi.Active,
        pi.CreatedOn,
        pi.CreatedBy,
        pi.ModifiedOn,
        pi.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
		,CASE WHEN EXISTS (SELECT 1 FROM PRs WHERE PRs.PIId = pi.Id) THEN 1  ELSE 0  END AS IsReturnExist  
    FROM PIs pi
    LEFT JOIN Suppliers s ON s.Id = pi.SupplierId
    WHERE pi.TenantId = @TenantId AND pi.CompanyId = @ParentCompanyId;
END;





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPI_GetAllByTenantAndCompanyFinancialYearId')
    DROP PROCEDURE [dbo].[spPI_GetAllByTenantAndCompanyFinancialYearId];
GO
CREATE PROCEDURE spPI_GetAllByTenantAndCompanyFinancialYearId
    @TenantId INT,
    @ParentCompanyId INT,
	@FinancialYearId Int
AS
BEGIN
    SELECT 
        pi.Id,
        pi.PICode,
        pi.PIDate,
        pi.POId,
        pi.SupplierId,
        pi.PISubTotal,
        pi.PIDiscount,
        pi.PITotal,
		pi.PITotalAfterVAT,
        pi.TenantId,
        pi.CompanyId,
		pi.FinancialYearId,
        pi.Active,
        pi.CreatedOn,
        pi.CreatedBy,
        pi.ModifiedOn,
        pi.ModifiedBy,
        COALESCE(s.Name, '--') AS SupplierName
		,CASE WHEN EXISTS (SELECT 1 FROM PRs WHERE PRs.PIId = pi.Id) THEN 1  ELSE 0  END AS IsReturnExist  
    FROM PIs pi
    LEFT JOIN Suppliers s ON s.Id = pi.SupplierId
    WHERE pi.TenantId = @TenantId AND pi.CompanyId = @ParentCompanyId AND pi.FinancialYearId = @FinancialYearId
END;








Go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPI_GetAllNotInUsedByTenantCompanyId')
    DROP PROCEDURE [dbo].[spPI_GetAllNotInUsedByTenantCompanyId];
GO
CREATE PROCEDURE spPI_GetAllNotInUsedByTenantCompanyId  
    @TenantId INT,  
    @ParentCompanyId INT,  
    @Id INT = NULL  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    SELECT   
        pi.Id,  
        pi.PICode,  
        pi.PIDate,  
        pi.SupplierId,  
        pi.PISubTotal,  
        pi.PIDiscount,  
        pi.PITotal,  
		pi.PITotalAfterVAT,
        pi.TenantId,  
        pi.CompanyId,  
		pi.FinancialYearId,  
        pi.Active,  
        pi.CreatedOn,  
        pi.CreatedBy,  
        pi.ModifiedOn,  
        pi.ModifiedBy,  
        COALESCE(s.Name, '--') AS SupplierName  
  ,CASE WHEN EXISTS (SELECT 1 FROM PRs WHERE PRs.PIId = pi.Id) THEN 1  ELSE 0  END AS IsReturnExist  
    FROM PIs pi  
    LEFT JOIN Suppliers s ON s.Id = pi.SupplierId  
    INNER JOIN Tenants t ON t.Id = pi.TenantId  
    INNER JOIN Companies com ON com.Id = pi.CompanyId  
    WHERE pi.TenantId = @TenantId  
      AND pi.CompanyId = @ParentCompanyId  
      AND (  
            (@Id IS NOT NULL AND pi.Id = @Id)   
            OR NOT EXISTS (SELECT 1 FROM PRs WHERE PRs.PIId = pi.Id)  
          );  
END;  










