--CREATE TABLE SOs (
--    Id INT IDENTITY(1,1) PRIMARY KEY, -- Auto-incrementing primary key
--    SOCode NVARCHAR(50) NOT NULL,    -- Sales Order Code
--    SODate DATE NOT NULL,            -- Sales Order Date
--    CustomerId INT NOT NULL,         -- Reference to Customer
--    SOSubTotal DECIMAL(18,2) NOT NULL, -- Subtotal of the Sales Order
--    SODiscount DECIMAL(18,2) DEFAULT 0.00, -- Discount applied
--    SOTotal DECIMAL(18,2) NOT NULL,  -- Total amount of the Sales Order
--    SONarration NVARCHAR(MAX) NULL,  -- Description or remarks
--    TenantId INT NOT NULL,           -- Reference to Tenant
--    CompanyId INT NOT NULL,          -- Reference to Company
--    Active BIT NOT NULL DEFAULT 1,   -- Status of the Sales Order (Active/Inactive)
--    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(), -- Record creation timestamp
--    CreatedBy NVARCHAR(50) NOT NULL, -- User who created the record
--    ModifiedOn DATETIME NULL,        -- Record modification timestamp
--    ModifiedBy NVARCHAR(50) NULL     -- User who modified the record
--);

--ALTER TABLE SOs
--ADD isholdPOS BIT NULL DEFAULT 0;



-- Insert Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSO_Insert')
    DROP PROCEDURE [dbo].[spSO_Insert];
GO
CREATE PROCEDURE spSO_Insert
    @SOCode NVARCHAR(50),
    @SODate DATE,
    @CustomerId INT,
    @SOSubTotal DECIMAL(18,2),
    @SODiscount DECIMAL(18,2),
    @SOTotal DECIMAL(18,2),
    @SONarration NVARCHAR(MAX),
    @TenantId INT,
    @CompanyId INT,
	@FinancialYearId INT,
	@SOTotalAfterVAT DECIMAL(18,2),
	@isholdPOS bit,
    @Active BIT,
    @CreatedOn DATETIME,
    @CreatedBy NVARCHAR(50),
	@ModifiedOn DATETIME,
    @ModifiedBy NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
	--declare @NewCode varchar(40);
	--BEGIN
	--	EXEC dbo.GenerateCode  @TableName = 'SOs', @ColumnName = 'SOCode', @Prefix = 'SO',  @NewCode  = @SOCode OUTPUT;
	--END
    INSERT INTO SOs (SOCode, SODate, CustomerId, SOSubTotal, SODiscount, SOTotal, SONarration, TenantId, CompanyId,FinancialYearId,SOTotalAfterVAT,isholdPOS, Active, CreatedOn, CreatedBy)
    VALUES (@SOCode, @SODate, @CustomerId, @SOSubTotal, @SODiscount, @SOTotal, @SONarration, @TenantId, @CompanyId,@FinancialYearId,@SOTotalAfterVAT,@isholdPOS, @Active, @CreatedOn, @CreatedBy);

    SET @Id = SCOPE_IDENTITY();
END;

-- Update Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSO_Update')
    DROP PROCEDURE [dbo].[spSO_Update];
GO
CREATE PROCEDURE spSO_Update
    @Id INT,
    @SOCode NVARCHAR(50),
    @SODate DATE,
    @CustomerId INT,
    @SOSubTotal DECIMAL(18,2),
    @SODiscount DECIMAL(18,2),
    @SOTotal DECIMAL(18,2),
    @SONarration NVARCHAR(MAX),
    @TenantId INT,
    @CompanyId INT,
	@FinancialYearId int,
	@SOTotalAfterVAT DECIMAL(18,2),
	@isholdPOS bit,
    @Active BIT,
	@CreatedOn DATETIME,
    @CreatedBy NVARCHAR(50),
    @ModifiedOn DATETIME,
    @ModifiedBy NVARCHAR(50)
AS
BEGIN
    UPDATE SOs
    SET
        SODate = @SODate,
        CustomerId = @CustomerId,
        SOSubTotal = @SOSubTotal,
        SODiscount = @SODiscount,
        SOTotal = @SOTotal,
        SONarration = @SONarration,
        TenantId = @TenantId,
        CompanyId = @CompanyId,
		isholdPOS = @isholdPOS,
		FinancialYearId = @FinancialYearId,
		SOTotalAfterVAT = @SOTotalAfterVAT,
        Active = @Active,
        ModifiedOn = @ModifiedOn,
        ModifiedBy = @ModifiedBy
    WHERE Id = @Id;
END;

-- Delete Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSO_Delete')
    DROP PROCEDURE [dbo].[spSO_Delete];
GO
CREATE PROCEDURE spSO_Delete
    @Id INT
AS
BEGIN
    DELETE FROM SOs
    WHERE Id = @Id;
END;

-- Get All Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSO_GetAll')
    DROP PROCEDURE [dbo].[spSO_GetAll];
GO
CREATE PROCEDURE spSO_GetAll
AS
BEGIN
    SELECT 
	so.Id	
	,so.SOCode	
	,so.SODate	
	,so.CustomerId	
	,so.SOSubTotal	
	,so.SODiscount	
	,so.SOTotal	
	,so.SONarration	
	,so.TenantId	
	,so.CompanyId	
	,so.FinancialYearId
	,so.SOTotalAfterVAT
	,so.isholdPOS
	,so.Active	
	,so.CreatedOn	
	,so.CreatedBy	
	,so.ModifiedOn	
	,so.ModifiedBy
	--------- Join Columns ------------
	,COALESCE(c.Name, '--') as CustomerName
	,COALESCE(t.TenantName, '-') AS TenantName
	,COALESCE(com.Name, '-') AS CompanyName
	,CASE WHEN EXISTS (SELECT 1 FROM SIs WHERE SIs.SOId = so.Id) THEN 1  ELSE 0  END AS IsSIExist
    FROM SOs so
	left join Customers c on c.Id = so.CustomerId
	LEFT JOIN
		Tenants t ON t.Id = so.TenantId
	LEFT JOIN
		Companies com ON com.Id  = so.CompanyId
END;




-- Get By Id Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSO_GetById')
    DROP PROCEDURE [dbo].[spSO_GetById];
GO
CREATE PROCEDURE spSO_GetById
    @Id INT
AS
BEGIN
    SELECT 
	so.Id	
	,so.SOCode	
	,so.SODate	
	,so.CustomerId	
	,so.SOSubTotal	
	,so.SODiscount	
	,so.SOTotal	
	,so.SONarration	
	,so.TenantId	
	,so.CompanyId	
	,so.FinancialYearId
	,so.SOTotalAfterVAT
	,so.isholdPOS
	,so.Active	
	,so.CreatedOn	
	,so.CreatedBy	
	,so.ModifiedOn	
	,so.ModifiedBy
	--------- Join Columns ------------
	,COALESCE(c.Name, '--') as CustomerName
	,COALESCE(t.TenantName, '-') AS TenantName
	,COALESCE(com.Name, '-') AS CompanyName
	,CASE WHEN EXISTS (SELECT 1 FROM SIs WHERE SIs.SOId = so.Id) THEN 1  ELSE 0  END AS IsSIExist
    FROM SOs so
	left join Customers c on c.Id = so.CustomerId
	inner JOIN
		Tenants t ON t.Id = so.TenantId
	inner JOIN
		Companies com ON com.Id  = so.CompanyId
    WHERE so.Id = @Id;
END;





Go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSO_GetByTenant')
    DROP PROCEDURE [dbo].[spSO_GetByTenant];
GO
CREATE PROCEDURE spSO_GetByTenant
    @TenantId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
	so.Id	
	,so.SOCode	
	,so.SODate	
	,so.CustomerId	
	,so.SOSubTotal	
	,so.SODiscount	
	,so.SOTotal	
	,so.SONarration	
	,so.TenantId	
	,so.CompanyId	
	,so.FinancialYearId
	,so.SOTotalAfterVAT
	,so.isholdPOS
	,so.Active	
	,so.CreatedOn	
	,so.CreatedBy	
	,so.ModifiedOn	
	,so.ModifiedBy
	--------- Join Columns ------------
	,COALESCE(c.Name, '--') as CustomerName
	,COALESCE(t.TenantName, '-') AS TenantName
    ,CASE WHEN EXISTS (SELECT 1 FROM SIs WHERE SIs.SOId = so.Id) THEN 1  ELSE 0  END AS IsSIExist
    FROM SOs so
	left join Customers c on c.Id = so.CustomerId
	Inner JOIN Tenants t ON t.Id = so.TenantId
    WHERE so.TenantId = @TenantId;
END;





Go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSO_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spSO_GetAllByTenantAndCompanyId];
GO
CREATE PROCEDURE spSO_GetAllByTenantAndCompanyId
@TenantId int ,
@ParentCompanyId int
as
begin

	SELECT 
	so.Id	
	,so.SOCode	
	,so.SODate	
	,so.CustomerId	
	,so.SOSubTotal	
	,so.SODiscount	
	,so.SOTotal	
	,so.SONarration	
	,so.TenantId	
	,so.CompanyId	
	,so.FinancialYearId
	,so.SOTotalAfterVAT
	,so.isholdPOS
	,so.Active	
	,so.CreatedOn	
	,so.CreatedBy	
	,so.ModifiedOn	
	,so.ModifiedBy
	--------- Join Columns ------------
	,COALESCE(c.Name, '--') as CustomerName
	,COALESCE(t.TenantName, '-') AS TenantName
	,COALESCE(com.Name, '-') AS CompanyName
	,CASE WHEN EXISTS (SELECT 1 FROM SIs WHERE SIs.SOId = so.Id) THEN 1  ELSE 0  END AS IsSIExist
    FROM SOs so
	left join Customers c on c.Id = so.CustomerId
	inner JOIN
		Tenants t ON t.Id = so.TenantId
	inner JOIN
		Companies com ON com.Id  = so.CompanyId
	WHERE
		so.TenantId = @TenantId
		AND so.CompanyId = @ParentCompanyId;
end




Go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSO_GetAllByTenantAndCompanyFinancialYearId')
    DROP PROCEDURE [dbo].[spSO_GetAllByTenantAndCompanyFinancialYearId];
GO
CREATE PROCEDURE spSO_GetAllByTenantAndCompanyFinancialYearId
@TenantId int ,
@ParentCompanyId int,
@FinancialYearId int
as
begin

	SELECT 
	so.Id	
	,so.SOCode	
	,so.SODate	
	,so.CustomerId	
	,so.SOSubTotal	
	,so.SODiscount	
	,so.SOTotal	
	,so.SONarration	
	,so.TenantId	
	,so.CompanyId	
	,so.FinancialYearId
	,so.SOTotalAfterVAT
	,so.isholdPOS
	,so.Active	
	,so.CreatedOn	
	,so.CreatedBy	
	,so.ModifiedOn	
	,so.ModifiedBy
	--------- Join Columns ------------
	,COALESCE(c.Name, '--') as CustomerName
	,COALESCE(t.TenantName, '-') AS TenantName
	,COALESCE(com.Name, '-') AS CompanyName
	,CASE WHEN EXISTS (SELECT 1 FROM SIs WHERE SIs.SOId = so.Id) THEN 1  ELSE 0  END AS IsSIExist
    FROM SOs so
	left join Customers c on c.Id = so.CustomerId
	inner JOIN
		Tenants t ON t.Id = so.TenantId
	inner JOIN
		Companies com ON com.Id  = so.CompanyId
	WHERE
		so.TenantId = @TenantId
		AND so.CompanyId = @ParentCompanyId
		AND so.FinancialYearId = @FinancialYearId
end





Go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSO_GetAllNotInUsedByTenantCompanyId')
    DROP PROCEDURE [dbo].[spSO_GetAllNotInUsedByTenantCompanyId];
GO
CREATE PROCEDURE spSO_GetAllNotInUsedByTenantCompanyId
    @TenantId INT,
    @ParentCompanyId INT,
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        so.Id,
        so.SOCode,
        so.SODate,
        so.CustomerId,
        so.SOSubTotal,
        so.SODiscount,
        so.SOTotal,
        so.SONarration,
        so.TenantId,
        so.CompanyId,
		so.FinancialYearId,
		so.SOTotalAfterVAT,
		so.isholdPOS,
        so.Active,
        so.CreatedOn,
        so.CreatedBy,
        so.ModifiedOn,
        so.ModifiedBy,
        COALESCE(c.Name, '--') AS CustomerName,
        COALESCE(t.TenantName, '-') AS TenantName,
        COALESCE(com.Name, '-') AS CompanyName,
        CASE WHEN EXISTS (SELECT 1 FROM SIs WHERE SIs.SOId = so.Id) THEN 1 ELSE 0 END AS IsSIExist
    FROM SOs so
    LEFT JOIN Customers c ON c.Id = so.CustomerId
    INNER JOIN Tenants t ON t.Id = so.TenantId
    INNER JOIN Companies com ON com.Id = so.CompanyId
    WHERE so.TenantId = @TenantId
      AND so.CompanyId = @ParentCompanyId
      AND (
            (@Id IS NOT NULL AND so.Id = @Id)
            OR NOT EXISTS (SELECT 1 FROM SIs WHERE SIs.SOId = so.Id)
          );
END;
GO




Go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSO_GetAllHoldPOSByTenantCompany')
    DROP PROCEDURE [dbo].[spSO_GetAllHoldPOSByTenantCompany];
GO
CREATE PROCEDURE spSO_GetAllHoldPOSByTenantCompany  
@TenantId int ,  
@ParentCompanyId int  
as  
begin  
  
 SELECT   
 so.Id   
 ,so.SOCode   
 ,so.SODate   
 ,so.CustomerId   
 ,so.SOSubTotal   
 ,so.SODiscount   
 ,so.SOTotal   
 ,so.SONarration   
 ,so.TenantId   
 ,so.CompanyId   
 ,so.FinancialYearId  
 ,so.SOTotalAfterVAT  
 ,so.isholdPOS
 ,so.Active   
 ,so.CreatedOn   
 ,so.CreatedBy   
 ,so.ModifiedOn   
 ,so.ModifiedBy  
 --------- Join Columns ------------  
 ,COALESCE(c.Name, '--') as CustomerName  
 ,COALESCE(t.TenantName, '-') AS TenantName  
 ,COALESCE(com.Name, '-') AS CompanyName  
 ,CASE WHEN EXISTS (SELECT 1 FROM SIs WHERE SIs.SOId = so.Id) THEN 1  ELSE 0  END AS IsSIExist  
    FROM SOs so  
 left join Customers c on c.Id = so.CustomerId  
 inner JOIN  
  Tenants t ON t.Id = so.TenantId  
 inner JOIN  
  Companies com ON com.Id  = so.CompanyId  
 WHERE  
  so.TenantId = @TenantId  
  AND so.CompanyId = @ParentCompanyId
  AND so.isholdPOS = 1
end  



  
  
  
  


--Go
--IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSO_GetAllByCompanyIdOrAccountType')
--    DROP PROCEDURE [dbo].[spSO_GetAllByCompanyIdOrAccountType];
--GO
--CREATE PROCEDURE spSO_GetAllByCompanyIdOrAccountType
--@ParentCompanyId int ,
--@AccountType varchar(40) = ''
--as
--begin
--	DECLARE @COATypeId INT;

--IF @AccountType = 'expense'
--    SET @COATypeId = 3;
--ELSE IF @AccountType = 'income'
--    SET @COATypeId = 5;
--ELSE
--    SET @COATypeId = NULL;

--SELECT
--    s.Id,
--    s.AccountCode,
--    s.Code,
--    s.AccountName,
--    s.ParentAccountId,
--    s.CoatypeId,
--    s.CoaTranType,
--    s.Description,
--    s.Transable,
--    s.LevelNo,
--    s.TenantId,
--    s.TenantCompanyId,
--    -- Joined Columns
--    COALESCE(parentS.AccountName, '-') AS ParentAccountName,
--    COALESCE(COAType.COATypeName, '-') AS COATypeName,
--    COALESCE(tenant.TenantName, '-') AS TenantName,
--    COALESCE(company.Name, '-') AS CompanyName
--FROM
--    SOs s
--LEFT JOIN
--    SOs parentS ON s.ParentAccountId = parentS.Id
--LEFT JOIN
--    Coatypes COAType ON s.CoatypeId = COAType.Id
--LEFT JOIN
--    Tenants tenant ON s.TenantId = tenant.Id
--LEFT JOIN
--    Companies company ON s.TenantCompanyId = company.Id
--WHERE
--    s.TenantCompanyId = @ParentCompanyId
--    AND (@COATypeId IS NULL OR s.CoatypeId = @COATypeId);
--end