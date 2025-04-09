--_____________________________________________ spCOA_Delete _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOA_Delete') DROP PROCEDURE [dbo].[spCOA_Delete]
GO
CREATE PROCEDURE [dbo].[spCOA_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM Coas
    WHERE Id = @Id;
END
GO

--_____________________________________________ spCOA_GetAll _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOA_GetAll') DROP PROCEDURE [dbo].[spCOA_GetAll]
GO
CREATE PROCEDURE [dbo].[spCOA_GetAll]  
AS  
BEGIN  
	select
	coa1.Id
	,coa1.AccountCode 
	,coa1.Code 
	,coa1.AccountName
	,coa1.ParentAccountId
	,coa1.CoatypeId
	,coa1.CoaCategoryId
	,coa1.CoaTranType
	,coa1.Description
	,coa1.Transable
	,coa1.LevelNo 
	,coa1.TenantId
	,coa1.TenantCompanyId,
    -- Joined Columns
    COALESCE(coa2.AccountName, '-') AS ParentAccountName,
    COALESCE(COAT.COATypeName, '-') AS COATypeName,
    COALESCE(tn.TenantName, '-') AS TenantName,
	COALESCE(cmp.Name, '-') AS CompanyName
    FROM
        Coas coa1
    LEFT JOIN
        Coas coa2 ON coa1.ParentAccountId = coa2.Id
    LEFT JOIN
        Coatypes COAT ON coa1.COATypeId = COAT.Id
    LEFT JOIN
        Tenants tn on tn.Id = coa1.TenantId
	LEFT JOIN
        Companies cmp on cmp.Id = coa1.TenantCompanyId
END
GO

--_____________________________________________ spCOA_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOA_GetById') DROP PROCEDURE [dbo].[spCOA_GetById]
GO
CREATE PROCEDURE [dbo].[spCOA_GetById]  
    @Id INT  
AS  
BEGIN  
 select
	 coa1.Id
	,coa1.AccountCode 
	,coa1.Code 
	,coa1.AccountName
	,coa1.ParentAccountId
	,coa1.CoatypeId
	,coa1.CoaCategoryId
	,coa1.CoaTranType
	,coa1.Description
	,coa1.Transable
	,coa1.LevelNo 
	,coa1.TenantId
	,coa1.TenantCompanyId,
    -- Joined Columns
    COALESCE(coa2.AccountName, '-') AS ParentAccountName,
    COALESCE(COAT.COATypeName, '-') AS COATypeName,
    COALESCE(tn.TenantName, '-') AS TenantName,
	COALESCE(cmp.Name, '-') AS CompanyName
    FROM
        Coas coa1
    LEFT JOIN
        Coas coa2 ON coa1.ParentAccountId = coa2.Id
    LEFT JOIN
        Coatypes COAT ON coa1.COATypeId = COAT.Id
    LEFT JOIN
        Tenants tn on tn.Id = coa1.TenantId
	LEFT JOIN
        Companies cmp on cmp.Id = coa1.TenantCompanyId
		where coa1.Id = @Id
END

GO
--_____________________________________________ spCOA_GetByName _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOA_GetByName') DROP PROCEDURE [dbo].[spCOA_GetByName]
GO
CREATE PROCEDURE [dbo].[spCOA_GetByName]
    @Name NVARCHAR(255)
AS
BEGIN
	select
    coa1.Id
	,coa1.AccountCode 
	,coa1.Code 
	,coa1.AccountName
	,coa1.ParentAccountId
	,coa1.CoatypeId
	,coa1.CoaCategoryId
	,coa1.CoaTranType
	,coa1.Description
	,coa1.Transable
	,coa1.LevelNo 
	,coa1.TenantId
	,coa1.TenantCompanyId,
    -- Joined Columns
    COALESCE(coa2.AccountName, '-') AS ParentAccountName,
    COALESCE(COAT.COATypeName, '-') AS COATypeName,
    COALESCE(tn.TenantName, '-') AS TenantName,
	COALESCE(cmp.Name, '-') AS CompanyName
    FROM 
    Coas coa1
    LEFT JOIN
        Coas coa2 ON coa1.ParentAccountId = coa2.Id
    LEFT JOIN
        Coatypes COAT ON coa1.COATypeId = COAT.Id
    LEFT JOIN
        Tenants tn on tn.Id = coa1.TenantId
	LEFT JOIN
        Companies cmp on cmp.Id = coa1.TenantCompanyId
        where coa1.AccountName LIKE '%' + @Name + '%';
END

GO
--_____________________________________________ spCOA_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOA_Insert')  DROP PROCEDURE [dbo].[spCOA_Insert];
GO
create PROCEDURE [dbo].[spCOA_Insert]  
    @AccountCode NVARCHAR(255) = NULL,  -- If not provided, it will be generated dynamically  
    @Code NVARCHAR(255) = NULL,         -- If not provided, it will be generated dynamically  
    @AccountName NVARCHAR(255),  
    @ParentAccountId INT = NULL,  
    @CoatypeId INT,  
	@CoaCategoryId INT,
    @CoaTranType VARCHAR(15),  
    @Description NVARCHAR(500) = NULL,  
    @Transable BIT,  
    @LevelNo INT,  
    @TenantId INT,  
    @TenantCompanyId INT,  
    @Active BIT = 1,  
    @CreatedBy NVARCHAR(100),  
    @CreatedOn DATETIME = NULL,  
    @ModifiedBy NVARCHAR(100),  
    @ModifiedOn DATETIME = NULL,  
    @Id INT OUTPUT  
AS  
BEGIN  
    SET @CreatedOn = COALESCE(@CreatedOn, GETDATE())  
    SET @ModifiedOn = COALESCE(@ModifiedOn, GETDATE())  
  
    BEGIN TRANSACTION;  
  
    BEGIN TRY  
       
        -- Insert the record into the Coas table  
        INSERT INTO Coas  
        (  
            AccountCode,  
            Code,  
            AccountName,  
            ParentAccountId,  
            CoatypeId,  
			CoaCategoryId,
            CoaTranType,  
            Description,  
            Transable,  
            LevelNo,  
            TenantId,  
            TenantCompanyId,  
            Active,  
            CreatedBy,  
            CreatedOn  
        )  
        VALUES  
        (  
            @AccountCode,  
            @Code,  
            @AccountName,  
            @ParentAccountId,  
            @CoatypeId, 
			@CoaCategoryId,
            @CoaTranType,  
            @Description,  
            @Transable,  
            @LevelNo,  
            @TenantId,  
            @TenantCompanyId,  
            @Active,  
            @CreatedBy,  
            getdate()  
        );  
  
        -- Set the output parameter to the new ID  
        SET @Id = SCOPE_IDENTITY();  
  
        -- Commit the transaction  
        COMMIT TRANSACTION;  
    END TRY  
    BEGIN CATCH  
        -- Rollback transaction on error  
        ROLLBACK TRANSACTION;  
  
        -- Raise the error  
        THROW;  
    END CATCH  
END;  
go
--_____________________________________________ spCOA_Update _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOA_Update')
    DROP PROCEDURE [dbo].[spCOA_Update];
GO
CREATE PROCEDURE [dbo].[spCOA_Update]
    @Id INT,  
    @AccountCode NVARCHAR(255) = NULL,
    @Code NVARCHAR(255) = NULL,
    @AccountName NVARCHAR(255),
    @ParentAccountId INT = NULL,
    @CoatypeId INT,
	@CoaCategoryId INT,
    @CoaTranType VARCHAR(15),
    @Description NVARCHAR(500) = NULL,
    @Transable BIT,
    @LevelNo INT,
    @TenantId INT,
    @TenantCompanyId INT,
    @Active BIT = 1,
	@CreatedBy NVARCHAR(100),
    @CreatedOn DATETIME = NULL,
    @ModifiedBy NVARCHAR(100) = 'User',
    @ModifiedOn DATETIME = NULL
AS
BEGIN
    SET @ModifiedOn = COALESCE(@ModifiedOn, GETDATE())

    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE Coas  
        SET   
            AccountCode = COALESCE(@AccountCode, AccountCode),
            Code = COALESCE(@Code, Code),
            AccountName = @AccountName,  
            ParentAccountId = @ParentAccountId,  
            CoatypeId = @CoatypeId,
			CoaCategoryId = @CoaCategoryId,
            CoaTranType = @CoaTranType,
            Description = @Description,  
            Transable = @Transable,
            LevelNo = @LevelNo,
            TenantId = @TenantId,
            TenantCompanyId = @TenantCompanyId,
            Active = @Active,
            ModifiedBy = @ModifiedBy,
            ModifiedOn = @ModifiedOn
        WHERE   
            Id = @Id;  

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO


--_____________________________________________ spCOA_GetAllByParentCompanyAndTenantId _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOA_GetAllByParentCompanyAndTenantId') DROP PROCEDURE [dbo].[spCOA_GetAllByParentCompanyAndTenantId]
GO
CREATE PROCEDURE [dbo].[spCOA_GetAllByParentCompanyAndTenantId]  
        @ParentCompanyId INT,
    @TenantId INT
AS  
BEGIN  
 select
	 coa1.Id
	,coa1.AccountCode 
	,coa1.Code 
	,coa1.AccountName
	,coa1.ParentAccountId
	,coa1.CoatypeId
	,coa1.CoaCategoryId
	,coa1.CoaTranType
	,coa1.Description
	,coa1.Transable
	,coa1.LevelNo 
	,coa1.TenantId
	,coa1.TenantCompanyId,
    -- Joined Columns
    COALESCE(coa2.AccountName, '-') AS ParentAccountName,
    COALESCE(COAT.COATypeName, '-') AS COATypeName,
    COALESCE(tn.TenantName, '-') AS TenantName,
	COALESCE(cmp.Name, '-') AS CompanyName
    FROM
        Coas coa1
    LEFT JOIN
        Coas coa2 ON coa1.ParentAccountId = coa2.Id
    LEFT JOIN
        Coatypes COAT ON coa1.COATypeId = COAT.Id
    LEFT JOIN
        Tenants tn on tn.Id = coa1.TenantId
	LEFT JOIN
        Companies cmp on cmp.Id = coa1.TenantCompanyId
		WHERE 
        coa1.TenantId = @TenantId And
        coa1.TenantCompanyId =  @ParentCompanyId
END
GO


--_____________________________________________ spCOA_GetAllByCompanyIdOrAccountType _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOA_GetAllByCompanyIdOrAccountType')
    DROP PROCEDURE [dbo].[spCOA_GetAllByCompanyIdOrAccountType];
GO
CREATE PROCEDURE [dbo].[spCOA_GetAllByCompanyIdOrAccountType]  
    @ParentCompanyId INT,
    @AccountType VARCHAR(45) = ''
AS  
BEGIN  
    -- Map AccountType to COATypeId
    DECLARE @COATypeId INT;
	
	SET @COATypeId = (SELECT TOP 1 id FROM Coatypes WHERE CoatypeName = @AccountType);
  
    -- Main Query
    SELECT
        coa1.Id,
        coa1.AccountCode,
        coa1.Code,
        coa1.AccountName,
        coa1.ParentAccountId,
        coa1.CoatypeId,
		coa1.CoaCategoryId,
        coa1.CoaTranType,
        coa1.Description,
        coa1.Transable,
        coa1.LevelNo,
        coa1.TenantId,
        coa1.TenantCompanyId,
        -- Joined Columns
        COALESCE(coa2.AccountName, '-') AS ParentAccountName,
        COALESCE(COAT.COATypeName, '-') AS COATypeName,
        COALESCE(tn.TenantName, '-') AS TenantName,
        COALESCE(cmp.Name, '-') AS CompanyName
    FROM
        Coas coa1
    LEFT JOIN
        Coas coa2 ON coa1.ParentAccountId = coa2.Id
    LEFT JOIN
        Coatypes COAT ON coa1.COATypeId = COAT.Id
    LEFT JOIN
        Tenants tn ON tn.Id = coa1.TenantId
    LEFT JOIN
        Companies cmp ON cmp.Id = coa1.TenantCompanyId
    WHERE 
        coa1.TenantCompanyId = @ParentCompanyId
        AND (@COATypeId IS NULL OR COAT.Id = @COATypeId); -- Filter by COATypeId if specified
END;
GO


--_____________________________________________ spCOA_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOA_GetById')
    DROP PROCEDURE [dbo].spCOA_GetById;
go
Create PROCEDURE [dbo].[spCOA_GetById]  
    @Id INT,
	  @CompanyId INT,
    @TenantId INT  
AS  
BEGIN  
 select
	 coa1.Id
	,coa1.AccountCode 
	,coa1.Code 
	,coa1.AccountName
	,coa1.ParentAccountId
	,coa1.CoatypeId
	,coa1.CoaCategoryId
	,coa1.CoaTranType
	,coa1.Description
	,coa1.Transable
	,coa1.LevelNo 
	,coa1.TenantId
	,coa1.TenantCompanyId,
    -- Joined Columns
    COALESCE(coa2.AccountName, '-') AS ParentAccountName,
    COALESCE(COAT.COATypeName, '-') AS COATypeName,
    COALESCE(tn.TenantName, '-') AS TenantName,
	COALESCE(cmp.Name, '-') AS CompanyName
    FROM
        Coas coa1
    LEFT JOIN
        Coas coa2 ON coa1.ParentAccountId = coa2.Id
    LEFT JOIN
        Coatypes COAT ON coa1.COATypeId = COAT.Id
    LEFT JOIN
        Tenants tn on tn.Id = coa1.TenantId
	LEFT JOIN
        Companies cmp on cmp.Id = coa1.TenantCompanyId
		where coa1.Id = @Id and coa1.TenantId = @TenantId and coa1.TenantCompanyId = @CompanyId
END

go

--__________________________________________________________________________________________________________________
--_____________________________________________ spGetMaxAccountCodeByTenantId _____________________________________________ 
--__________________________________________________________________________________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetMaxAccountCodeByTenantId')
    DROP PROCEDURE [dbo].spGetMaxAccountCodeByTenantId;
GO
Create proc spGetMaxAccountCodeByTenantId
@TenantId int,
 @COATypeId INT = null
as
select top 1 AccountCode from coas 
where (ParentAccountId is null or ParentAccountId = 0) and  tenantid = @TenantId and
(@COATypeId IS NULL OR CoatypeId = @COATypeId)
order by AccountCode desc 

go

--__________________________________________________________________________________________________________________
--_____________________________________________ spCOA_GetAllByCompanyIdOrAccountTypeId ______________________________________
--__________________________________________________________________________________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOA_GetAllByCompanyIdOrAccountTypeId')
    DROP PROCEDURE [dbo].spCOA_GetAllByCompanyIdOrAccountTypeId;
GO
create PROCEDURE [dbo].[spCOA_GetAllByCompanyIdOrAccountTypeId]  
	@TenantId int,
    @ParentCompanyId INT,
    @COATypeId INT = null
AS  
BEGIN  
    -- Main Query
    SELECT
        coa1.Id,
        coa1.AccountCode,
        coa1.Code,
        coa1.AccountName,
        coa1.ParentAccountId,
        coa1.CoatypeId,
		coa1.CoaCategoryId,
        coa1.CoaTranType,
        coa1.Description,
        coa1.Transable,
        coa1.LevelNo,
        coa1.TenantId,
        coa1.TenantCompanyId,
        -- Joined Columns
        COALESCE(coa2.AccountName, '-') AS ParentAccountName,
        COALESCE(COAT.COATypeName, '-') AS COATypeName,
        COALESCE(tn.TenantName, '-') AS TenantName,
        COALESCE(cmp.Name, '-') AS CompanyName
    FROM
        Coas coa1
    LEFT JOIN
        Coas coa2 ON coa1.ParentAccountId = coa2.Id
    LEFT JOIN
        Coatypes COAT ON coa1.COATypeId = COAT.Id
    LEFT JOIN
        Tenants tn ON tn.Id = coa1.TenantId
    LEFT JOIN
        Companies cmp ON cmp.Id = coa1.TenantCompanyId
    WHERE 
        coa1.TenantCompanyId = @ParentCompanyId
		and coa1.TenantId = @TenantId
        AND (@COATypeId IS NULL OR coa1.CoatypeId = @COATypeId); 
		END;
	


--__________________________________________________________________________________________________________________
--_____________________________________________ spGetMaxCodeByTenantId _____________________________________________ 
--__________________________________________________________________________________________________________________
go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetMaxCodeByTenantId')
    DROP PROCEDURE [dbo].spGetMaxCodeByTenantId;
GO
create PROCEDURE [dbo].[spGetMaxCodeByTenantId]  
    @TenantId INT,  
    @TableName NVARCHAR(128),  
    @ColumnName NVARCHAR(128),
	@IdColumnName  NVARCHAR(128),
	@TenantIdColumnName  NVARCHAR(128),
	@CompanyColumnName  NVARCHAR(128) = null,
	@CompanyId int = null
AS  
BEGIN  
    SET NOCOUNT ON;

    -- Declare a variable to hold the dynamic SQL
    DECLARE @SQL NVARCHAR(MAX);

    -- Construct the dynamic SQL query
 SET @SQL = N'SELECT TOP 1 ' + QUOTENAME(@ColumnName) + N' AS MaxCode ' +
           N'FROM ' + QUOTENAME(@TableName) + N' ' +
           N'WHERE ' + QUOTENAME(@TenantIdColumnName) + N' = @TenantId ' +
           CASE 
               WHEN @CompanyColumnName IS NOT NULL 
               THEN N' AND ' + QUOTENAME(@CompanyColumnName) + N' = @CompanyId ' 
               ELSE N'' 
           END +
           N'ORDER BY ' + QUOTENAME(@IdColumnName) + N' DESC';

    -- Execute the dynamic SQL with parameter substitution
    EXEC sp_executesql 
        @SQL,
        N'@TenantId INT, @CompanyId INT',
        @TenantId,@CompanyId;
END;

--__________________________________________________________________________________________________________________
--_____________________________________________ spGetMaxAccountCodeByParentIdTenantId _____________________________________________ 
--__________________________________________________________________________________________________________________
go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetMaxAccountCodeByParentIdTenantId')
    DROP PROCEDURE [dbo].spGetMaxAccountCodeByParentIdTenantId;
GO
Create proc spGetMaxAccountCodeByParentIdTenantId
@TenantId int,
 @COATypeId INT = null,
 @ParentAccountId int
as
select top 1 AccountCode from coas 
where 
parentAccountId = @ParentAccountId and
tenantid = @TenantId and
(@COATypeId IS NULL OR CoatypeId = @COATypeId)
order by AccountCode desc 


