--_____________________________________________ spFinancialYear_Insert _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spFinancialYear_Insert') DROP PROCEDURE [dbo].[spFinancialYear_Insert]
GO
Create PROCEDURE [dbo].[spFinancialYear_Insert]
    @FYDescription NVARCHAR(100),
    @FYCode NVARCHAR(10),
    @FYStartDate DATE,
    @FYEndDate DATE,
    @IsCurrentYear BIT,
    @IsLock BIT,
    @TenantId INT,
	@CompanyId INT,
    @Active BIT,
    @CreatedBy NVARCHAR(100),
    @CreatedOn DATETIME,
    @ModifiedBy NVARCHAR(100),
    @ModifiedOn DATETIME,
	@CodePrefix nvarchar(100),
    @Id INT OUTPUT
AS
BEGIN
	--declare @NewCode varchar(40);
 --   BEGIN
 --       EXEC dbo.GenerateCode  @TableName = 'FinancialYears', @ColumnName = 'FYCode', @Prefix = 'FY',  @NewCode  = @FYCode OUTPUT;
 --   END

    INSERT INTO FinancialYears (FYDescription, FYCode, FYStartDate, FYEndDate, IsCurrentYear, IsLock, TenantId,CompanyId, Active, CreatedOn, CreatedBy,CodePrefix)
    VALUES (@FYDescription, @FYCode , @FYStartDate, @FYEndDate, @IsCurrentYear, @IsLock, @TenantId,@CompanyId, @Active, GETDATE(), @CreatedBy,@CodePrefix);
    
    SET @Id = SCOPE_IDENTITY();
END;
GO

--_____________________________________________ spFinancialYear_Update _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spFinancialYear_Update') DROP PROCEDURE [dbo].[spFinancialYear_Update]
GO
create PROCEDURE [dbo].[spFinancialYear_Update]
    @Id INT,
    @FYDescription NVARCHAR(100),
    @FYCode NVARCHAR(10),
    @FYStartDate DATE,
    @FYEndDate DATE,
    @IsCurrentYear BIT,
    @IsLock BIT,
    @TenantId INT,
	@CompanyId int,
    @Active BIT,
    @CreatedBy NVARCHAR(100),
    @CreatedOn DATETIME,
    @ModifiedBy NVARCHAR(100),
    @ModifiedOn DATETIME,
	@CodePrefix nvarchar(100)
AS
BEGIN
    UPDATE FinancialYears
    SET 
        FYDescription = @FYDescription,
        FYStartDate = @FYStartDate,
        FYEndDate = @FYEndDate,
        IsCurrentYear = @IsCurrentYear,
        IsLock = @IsLock,
        TenantId = @TenantId,
		CompanyId = @CompanyId,
        Active = @Active,
        ModifiedOn = GETDATE(),
        ModifiedBy = @ModifiedBy,CodePrefix = @CodePrefix
    WHERE Id = @Id;
END;

GO

--_____________________________________________ spFinancialYear_Delete _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spFinancialYear_Delete') DROP PROCEDURE [dbo].[spFinancialYear_Delete]
GO
CREATE PROCEDURE [dbo].[spFinancialYear_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM FinancialYears
    WHERE Id = @Id;
END;
GO


--_____________________________________________ spFinancialYear_GetAll _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spFinancialYear_GetAll') DROP PROCEDURE [dbo].[spFinancialYear_GetAll]
GO
CREATE PROCEDURE [dbo].[spFinancialYear_GetAll]
AS
BEGIN
    SELECT 
        fy.Id, 
        fy.FYDescription, 
        fy.FYCode, 
        fy.FYStartDate, 
        fy.FYEndDate, 
        fy.IsCurrentYear, 
        fy.IsLock, 
        fy.TenantId, 
		fy.CompanyId,
        fy.Active, 
        fy.CreatedOn, 
        fy.CreatedBy, 
        fy.ModifiedOn, 
        fy.ModifiedBy,
        --____ Join Columns ____  
        ISNULL(t.TenantName, '--') AS TenantName
    FROM FinancialYears fy
    LEFT JOIN Tenants t ON t.Id = fy.TenantId;
END;
GO

--_____________________________________________ spFinancialYear_GetById _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spFinancialYear_GetById') DROP PROCEDURE [dbo].[spFinancialYear_GetById]
GO
CREATE PROCEDURE [dbo].[spFinancialYear_GetById]
    @Id INT
AS
BEGIN
    SELECT 
        fy.Id, 
        fy.FYDescription, 
        fy.FYCode, 
        fy.FYStartDate, 
        fy.FYEndDate, 
        fy.IsCurrentYear, 
        fy.IsLock, 
        fy.TenantId, 
		fy.CompanyId,
        fy.Active, 
        fy.CreatedOn, 
        fy.CreatedBy, 
        fy.ModifiedOn, 
        fy.ModifiedBy,
        --____ Join Columns ____  
        ISNULL(t.TenantName, '--') AS TenantName
    FROM FinancialYears fy
    LEFT JOIN Tenants t ON t.Id = fy.TenantId
    WHERE fy.Id = @Id;
END;
GO



IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spFinancialYear_GetAllByTenantCompanyId') 
DROP PROCEDURE [dbo].[spFinancialYear_GetAllByTenantCompanyId];
GO
create PROCEDURE [dbo].[spFinancialYear_GetAllByTenantCompanyId]
    @ParentCompanyId INT,
    @TenantId INT
AS
BEGIN
    SELECT 
        fy.Id, 
        fy.FYDescription, 
        fy.FYCode, 
        fy.FYStartDate, 
        fy.FYEndDate, 
        fy.IsCurrentYear, 
        fy.IsLock, 
        fy.TenantId, 
		fy.CompanyId,
        fy.Active, 
        fy.CreatedOn, 
        fy.CreatedBy, 
        fy.ModifiedOn, 
        fy.ModifiedBy,fy.CodePrefix,
        --____ Join Columns ____  
        ISNULL(t.TenantName, '--') AS TenantName
    FROM FinancialYears fy
    LEFT JOIN Tenants t ON t.Id = fy.TenantId
    WHERE 
        fy.TenantId = @TenantId And
        fy.CompanyId =  @ParentCompanyId

END;
GO


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spFinancialYear_GetAllByCompanyId') 
DROP PROCEDURE [dbo].[spFinancialYear_GetAllByCompanyId];
GO
CREATE PROCEDURE [dbo].[spFinancialYear_GetAllByCompanyId]  
    @ParentCompanyId INT
AS  
BEGIN  
    SELECT   
        fy.Id,   
        fy.FYDescription,   
        fy.FYCode,   
        fy.FYStartDate,   
        fy.FYEndDate,   
        fy.IsCurrentYear,   
        fy.IsLock,   
        fy.TenantId,   
        fy.CompanyId,  
        fy.Active,   
        fy.CreatedOn,   
        fy.CreatedBy,   
        fy.ModifiedOn,   
        fy.ModifiedBy,  
        --____ Join Columns ____    
        ISNULL(t.TenantName, '--') AS TenantName  
    FROM FinancialYears fy  
    LEFT JOIN Tenants t ON t.Id = fy.TenantId  
    WHERE   
        fy.CompanyId =  @ParentCompanyId  
END;  





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spFinancialYear_UpdateIsLocakByCompanyAndFYId')
    DROP PROCEDURE [dbo].[spFinancialYear_UpdateIsLocakByCompanyAndFYId];
GO

CREATE PROCEDURE [dbo].[spFinancialYear_UpdateIsLocakByCompanyAndFYId]
    @Id INT,
    @CompanyId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Step 1: Set all records for the CompanyId to isLock = 0
    UPDATE FinancialYears
    SET isLock = 0
    WHERE CompanyId = @CompanyId;

    -- Step 2: Set isLock = 1 for the specific FYId and CompanyId
    UPDATE FinancialYears
    SET isLock = 1
    WHERE Id = @Id AND CompanyId = @CompanyId;
END;
GO


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spFinancialYear_GetById')
    DROP PROCEDURE [dbo].[spFinancialYear_GetById];
GO
create PROCEDURE [dbo].[spFinancialYear_GetById]
    @Id INT
AS
BEGIN
    SELECT 
        fy.Id, 
        fy.FYDescription, 
        fy.FYCode, 
        fy.FYStartDate, 
        fy.FYEndDate, 
        fy.IsCurrentYear, 
        fy.IsLock, 
        fy.TenantId, 
		fy.CompanyId,
        fy.Active, 
        fy.CreatedOn, 
        fy.CreatedBy, 
        fy.ModifiedOn, 
        fy.ModifiedBy,fy.CodePrefix,
        --____ Join Columns ____  
        ISNULL(t.TenantName, '--') AS TenantName
    FROM FinancialYears fy
    LEFT JOIN Tenants t ON t.Id = fy.TenantId
    WHERE fy.Id = @Id;
END;
