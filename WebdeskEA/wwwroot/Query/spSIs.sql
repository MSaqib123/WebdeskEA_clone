--CREATE TABLE [dbo].[SIs](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[SICode] [nvarchar](50) NOT NULL,
--	[SIDate] [datetime] NOT NULL,
--	[SOId] [int] NULL,
--	[CustomerId] [int] NOT NULL,
--	[SISubTotal] [decimal](18, 2) NOT NULL,
--	[SIDiscount] [decimal](18, 2) NOT NULL,
--	[SITotal] [decimal](18, 2) NOT NULL,
--	[SITotalAfterVAT] [decimal](18, 2) NOT NULL,
--	[TenantId] [int] NOT NULL,
--	[CompanyId] [int] NOT NULL,
--	[Active] [bit] NOT NULL,
--	[CreatedOn] [datetime] NOT NULL,
--	[CreatedBy] [varchar](50) NOT NULL,
--	[ModifiedOn] [datetime] NULL,
--	[ModifiedBy] [varchar](50) NULL,
--)
--GO





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSI_Insert')
    DROP PROCEDURE [dbo].[spSI_Insert];
GO
CREATE PROCEDURE spSI_Insert
    @SICode NVARCHAR(50),
    @SIDate DATETIME,
    @SOId INT,
    @CustomerId INT,
    @SISubTotal DECIMAL(18, 2),
    @SIDiscount DECIMAL(18, 2),
    @SITotal DECIMAL(18, 2),
	@SITotalAfterVAT DECIMAL(18, 2),
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
    --EXEC dbo.GenerateCode @TableName = 'SIs', @ColumnName = 'SICode', @Prefix = 'SI', @NewCode = @SICode OUTPUT;

    INSERT INTO SIs (SICode, SIDate, SOId, CustomerId, SISubTotal, SIDiscount, SITotal,SITotalAfterVAT, TenantId, CompanyId,FinancialYearId, Active, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy)
    VALUES (@SICode, @SIDate, @SOId, @CustomerId, @SISubTotal, @SIDiscount, @SITotal,@SITotalAfterVAT, @TenantId, @CompanyId,@FinancialYearId, @Active, @CreatedOn, @CreatedBy, @ModifiedOn, @ModifiedBy);

    SET @Id = SCOPE_IDENTITY();
END;









GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSI_Update')
    DROP PROCEDURE [dbo].[spSI_Update];
GO
CREATE PROCEDURE spSI_Update
    @Id INT,
    @SICode NVARCHAR(50),
    @SIDate DATETIME,
    @SOId INT,
    @CustomerId INT,
    @SISubTotal DECIMAL(18, 2),
    @SIDiscount DECIMAL(18, 2),
    @SITotal DECIMAL(18, 2),
	@SITotalAfterVAT DECIMAL(18, 2),
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
    UPDATE SIs
    SET
        SICode = @SICode,
        SIDate = @SIDate,
        SOId = @SOId,
        CustomerId = @CustomerId,
        SISubTotal = @SISubTotal,
        SIDiscount = @SIDiscount,
        SITotal = @SITotal,
		SITotalAfterVAT = @SITotalAfterVAT,
        TenantId = @TenantId,
        CompanyId = @CompanyId,
		FinancialYearId = @FinancialYearId,
        Active = @Active,
        ModifiedOn = @ModifiedOn,
        ModifiedBy = @ModifiedBy
    WHERE Id = @Id;
END;






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSI_Delete')
    DROP PROCEDURE [dbo].[spSI_Delete];
GO
CREATE PROCEDURE spSI_Delete
    @Id INT
AS
BEGIN
    DELETE FROM SIs
    WHERE Id = @Id;
END;




GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSI_GetAll')
    DROP PROCEDURE [dbo].[spSI_GetAll];
GO
CREATE PROCEDURE spSI_GetAll
AS
BEGIN
    SELECT 
        si.Id,
        si.SICode,
        si.SIDate,
        si.SOId,
        si.CustomerId,
        si.SISubTotal,
        si.SIDiscount,
        si.SITotal,
		si.SITotalAfterVAT,
        si.TenantId,
        si.CompanyId,
		si.FinancialYearId,
        si.Active,
        si.CreatedOn,
        si.CreatedBy,
        si.ModifiedOn,
        si.ModifiedBy,
        COALESCE(c.Name, '--') AS CustomerName
		,CASE WHEN EXISTS (SELECT 1 FROM SRs WHERE SRs.SIId = si.Id) THEN 1  ELSE 0  END AS IsReturnExist
    FROM SIs si
    LEFT JOIN Customers c ON c.Id = si.CustomerId;
END;



GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSI_GetById')
    DROP PROCEDURE [dbo].[spSI_GetById];
GO
CREATE PROCEDURE spSI_GetById
@Id INT
AS
BEGIN
    SELECT 
        si.Id,
        si.SICode,
        si.SIDate,
        si.SOId,
        si.CustomerId,
        si.SISubTotal,
        si.SIDiscount,
        si.SITotal,
		si.SITotalAfterVAT,
        si.TenantId,
        si.CompanyId,
		si.FinancialYearId,
        si.Active,
        si.CreatedOn,
        si.CreatedBy,
        si.ModifiedOn,
        si.ModifiedBy,
        COALESCE(c.Name, '--') AS CustomerName
		,CASE WHEN EXISTS (SELECT 1 FROM SRs WHERE SRs.SIId = si.Id) THEN 1  ELSE 0  END AS IsReturnExist
    FROM SIs si
    LEFT JOIN Customers c ON c.Id = si.CustomerId
	where si.Id = @Id
END;




GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSI_GetBySOId')
    DROP PROCEDURE [dbo].[spSI_GetBySOId];
GO
CREATE PROCEDURE spSI_GetBySOId
@Id INT
AS
BEGIN
    SELECT 
	top 1
        si.Id,
        si.SICode,
        si.SIDate,
        si.SOId,
        si.CustomerId,
        si.SISubTotal,
        si.SIDiscount,
        si.SITotal,
		si.SITotalAfterVAT,
        si.TenantId,
        si.CompanyId,
		si.FinancialYearId,
        si.Active,
        si.CreatedOn,
        si.CreatedBy,
        si.ModifiedOn,
        si.ModifiedBy,
        COALESCE(c.Name, '--') AS CustomerName
		,CASE WHEN EXISTS (SELECT 1 FROM SRs WHERE SRs.SIId = si.Id) THEN 1  ELSE 0  END AS IsReturnExist
    FROM SIs si
    LEFT JOIN Customers c ON c.Id = si.CustomerId
	where si.SOId = @Id
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSI_GetByTenant')
    DROP PROCEDURE [dbo].[spSI_GetByTenant];
GO
CREATE PROCEDURE spSI_GetByTenant
    @TenantId INT
AS
BEGIN
    SELECT 
        si.Id,
        si.SICode,
        si.SIDate,
        si.SOId,
        si.CustomerId,
        si.SISubTotal,
        si.SIDiscount,
        si.SITotal,
		si.SITotalAfterVAT,
        si.TenantId,
        si.CompanyId,
		si.FinancialYearId,
        si.Active,
        si.CreatedOn,
        si.CreatedBy,
        si.ModifiedOn,
        si.ModifiedBy,
        COALESCE(c.Name, '--') AS CustomerName
		,CASE WHEN EXISTS (SELECT 1 FROM SRs WHERE SRs.SIId = si.Id) THEN 1  ELSE 0  END AS IsReturnExist
    FROM SIs si
    LEFT JOIN Customers c ON c.Id = si.CustomerId
    WHERE si.TenantId = @TenantId;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSI_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spSI_GetAllByTenantAndCompanyId];
GO
CREATE PROCEDURE spSI_GetAllByTenantAndCompanyId  
    @TenantId INT,  
    @ParentCompanyId int
AS  
BEGIN  
    SELECT   
        si.Id,  
        si.SICode,  
        si.SIDate,  
        si.SOId,  
        si.CustomerId,  
        si.SISubTotal,  
        si.SIDiscount,  
        si.SITotal,  
		si.SITotalAfterVAT,
        si.TenantId,  
        si.CompanyId, 
		si.FinancialYearId,
        si.Active,  
        si.CreatedOn,  
        si.CreatedBy,  
        si.ModifiedOn,  
        si.ModifiedBy,  
        COALESCE(c.Name, '--') AS CustomerName 
		,CASE WHEN EXISTS (SELECT 1 FROM SRs WHERE SRs.SIId = si.Id) THEN 1  ELSE 0  END AS IsReturnExist 
    FROM SIs si  
    LEFT JOIN Customers c ON c.Id = si.CustomerId  
    WHERE si.TenantId = @TenantId AND si.CompanyId = @ParentCompanyId;  
END;  






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSI_GetAllByTenantAndCompanyFinancialYearId')
    DROP PROCEDURE [dbo].[spSI_GetAllByTenantAndCompanyFinancialYearId];
GO
CREATE PROCEDURE spSI_GetAllByTenantAndCompanyFinancialYearId  
    @TenantId INT,  
    @ParentCompanyId int,
	@FinancialYearId int
AS  
BEGIN  
    SELECT   
        si.Id,  
        si.SICode,  
        si.SIDate,  
        si.SOId,  
        si.CustomerId,  
        si.SISubTotal,  
        si.SIDiscount,  
        si.SITotal,  
		si.SITotalAfterVAT,
        si.TenantId,  
        si.CompanyId, 
		si.FinancialYearId,
        si.Active,  
        si.CreatedOn,  
        si.CreatedBy,  
        si.ModifiedOn,  
        si.ModifiedBy,  
        COALESCE(c.Name, '--') AS CustomerName 
		,CASE WHEN EXISTS (SELECT 1 FROM SRs WHERE SRs.SIId = si.Id) THEN 1  ELSE 0  END AS IsReturnExist 
    FROM SIs si  
    LEFT JOIN Customers c ON c.Id = si.CustomerId  
    WHERE si.TenantId = @TenantId AND si.CompanyId = @ParentCompanyId And si.FinancialYearId = @FinancialYearId
END;  










Go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSI_GetAllNotInUsedByTenantCompanyId')
    DROP PROCEDURE [dbo].[spSI_GetAllNotInUsedByTenantCompanyId];
GO

CREATE PROCEDURE spSI_GetAllNotInUsedByTenantCompanyId
    @TenantId INT,
    @ParentCompanyId INT,
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        si.Id,
        si.SICode,
        si.SIDate,
        si.CustomerId,
        si.SISubTotal,
        si.SIDiscount,
        si.SITotal,
		si.SITotalAfterVAT,
        si.TenantId,
        si.CompanyId,
		si.FinancialYearId,
        si.Active,
        si.CreatedOn,
        si.CreatedBy,
        si.ModifiedOn,
        si.ModifiedBy,
        COALESCE(s.Name, '--') AS CustomerName
		,CASE WHEN EXISTS (SELECT 1 FROM SRs WHERE SRs.SIId = si.Id) THEN 1  ELSE 0  END AS IsReturnExist
    FROM SIs si
    LEFT JOIN Suppliers s ON s.Id = si.CustomerId
    INNER JOIN Tenants t ON t.Id = si.TenantId
    INNER JOIN Companies com ON com.Id = si.CompanyId
    WHERE si.TenantId = @TenantId
      AND si.CompanyId = @ParentCompanyId
      AND (
            (@Id IS NOT NULL AND si.Id = @Id) 
            OR NOT EXISTS (SELECT 1 FROM SRs WHERE SRs.SIId = si.Id)
          );
END;
GO