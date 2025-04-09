--_____________________________________________ spCompany_BulkInsert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompany_BulkInsert') DROP PROCEDURE [dbo].[spCompany_BulkInsert]
GO
CREATE PROCEDURE [dbo].[spCompany_BulkInsert]
    @jsonInput NVARCHAR(MAX)
AS
BEGIN
    -- BulkInsert By Json  
	-- handle all logic in  SQL
	print 'Copay CompanyUserBulkInsert  procceduer to add custom logices'
END
GO
--_____________________________________________ spCompany_Delete _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompany_Delete') DROP PROCEDURE [dbo].[spCompany_Delete]
GO
CREATE PROCEDURE [dbo].[spCompany_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM Companies
    WHERE Id = @Id;
END;
GO
--_____________________________________________ spCompany_GetAll _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompany_GetAll') DROP PROCEDURE [dbo].[spCompany_GetAll]
GO
CREATE PROCEDURE [dbo].[spCompany_GetAll]  
AS  
BEGIN  
    SELECT Id, CompanyCode, TenantId, Name, ParentCompanyId, Address, Description, Phone, Mobile, Email, Website, PostalCode, Logo, Trn, CountryId, StateId, CityId, IsMainCompany, Active, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM Companies;  
END;
GO

--_____________________________________________ spCompany_GetAllByTenantId _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompany_GetAllByTenantId') DROP PROCEDURE [dbo].[spCompany_GetAllByTenantId]
GO
CREATE PROCEDURE [dbo].[spCompany_GetAllByTenantId]  
@TenantId int
AS  
BEGIN  
    SELECT Id, CompanyCode, TenantId, Name, ParentCompanyId, Address, Description, Phone, Mobile, Email, Website, PostalCode, Logo, Trn, CountryId, StateId, CityId, IsMainCompany, Active, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM Companies
    where TenantId = @TenantId 
END;
GO

--_____________________________________________ spCompany_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompany_GetById') DROP PROCEDURE [dbo].[spCompany_GetById]
GO
CREATE PROCEDURE [dbo].[spCompany_GetById]  
    @Id INT  
AS  
BEGIN  
    SELECT Id, CompanyCode, TenantId, Name, ParentCompanyId, Address, Description, Phone, Mobile, Email, Website, PostalCode, Logo, Trn, CountryId, StateId, CityId, IsMainCompany, Active, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM Companies  
    WHERE Id = @Id;  
END;
GO
--_____________________________________________ spCompany_GetByName _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompany_GetByName') DROP PROCEDURE [dbo].[spCompany_GetByName]
GO
CREATE PROCEDURE [dbo].[spCompany_GetByName]
    @Name NVARCHAR(100)
AS
BEGIN
    SELECT Id, CompanyCode, TenantId, Name, ParentCompanyId, Address, Description, Phone, Mobile, Email, Website, PostalCode, Logo, Trn, CountryId, StateId, CityId, IsMainCompany, Active, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM Companies
    WHERE Name LIKE '%' + @Name + '%';
END;
GO
--_____________________________________________ spCompany_GetPaginated _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompany_GetPaginated') DROP PROCEDURE [dbo].[spCompany_GetPaginated]
GO
CREATE PROCEDURE [dbo].[spCompany_GetPaginated]
    @PageIndex INT,
    @PageSize INT,
    @Filter NVARCHAR(100)
AS
BEGIN
    SELECT Id, CompanyCode, TenantId, Name, ParentCompanyId, Address, Description, Phone, Mobile, Email, Website, PostalCode, Logo, Trn, CountryId, StateId, CityId, IsMainCompany, Active, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM Companies
    WHERE Name LIKE '%' + @Filter + '%'
    ORDER BY Id
    OFFSET @PageIndex * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO
--_____________________________________________ spCompany_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompany_Insert') DROP PROCEDURE [dbo].[spCompany_Insert]
GO
CCreate PROCEDURE [dbo].[spCompany_Insert]      
    @CompanyCode NVARCHAR(50),  
    @TenantId INT,  
    @Name NVARCHAR(100),  
    @ParentCompanyId INT,  
    @Address NVARCHAR(500)=NULL,  
    @Description NVARCHAR(500)=NULL,  
    @Phone NVARCHAR(15),  
    @Mobile NVARCHAR(15)=NULL,  
    @Email NVARCHAR(100),  
    @Website NVARCHAR(100)=NULL,  
    @PostalCode NVARCHAR(10)=NULL,  
    @Logo NVARCHAR(200),  
    @Trn NVARCHAR(50)=NULL,  
    @CountryId INT,  
    @StateId INT,  
    @CityId INT,  
    @IsMainCompany BIT,  
    @Active BIT,  
    @CreatedBy NVARCHAR(50),  
 @CreatedOn DateTime,  
 @ModifiedBy NVARCHAR(50),  
 @ModifiedOn DateTime,  
 @Id INT OUTPUT  
  
AS      
BEGIN      
 declare @NewCode varchar(40);  
  
    ---- Check for uniqueness of Name  
    --IF EXISTS (SELECT 1 FROM Companies WHERE Name = @Name AND Id <> @Id)  
    --BEGIN  
    --    THROW 50000, 'Company name already exists.', 1;  
    --END  
  
    -- Generate CompanyCode if it's NULL  
    IF @CompanyCode IS NULL  
    BEGIN  
        EXEC dbo.GenerateCode  @TableName = 'Companies', @ColumnName = 'CompanyCode', @Prefix = 'CMP',  @NewCode = @CompanyCode OUTPUT;  
    END  
  
    -- Check if the generated ----   
    IF EXISTS (SELECT 1 FROM Companies WHERE CompanyCode = @CompanyCode AND Id <> @Id)  
    BEGIN  
        THROW 50001, 'CompanyCode already exists.', 1;  
    END  
  
    INSERT INTO Companies (CompanyCode, TenantId, Name, ParentCompanyId, Address, Description, Phone, Mobile, Email, Website, PostalCode, Logo, Trn, CountryId, StateId, CityId, IsMainCompany, Active, CreatedBy, CreatedOn)  
    VALUES (@CompanyCode, @TenantId, @Name, @ParentCompanyId, @Address, @Description, @Phone, @Mobile, @Email, @Website, @PostalCode, @Logo, @Trn, @CountryId, @StateId, @CityId, @IsMainCompany, @Active, @CreatedBy, GETDATE());  
          
 SET @Id = SCOPE_IDENTITY();  
END;  
GO
--_____________________________________________ spCompany_Update _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompany_Update') DROP PROCEDURE [dbo].[spCompany_Update]
GO
CREATE PROCEDURE [dbo].[spCompany_Update]  
    @Id INT,
    @CompanyCode NVARCHAR(50),  
    @TenantId INT,
    @Name NVARCHAR(100),
    @ParentCompanyId INT,
    @Address NVARCHAR(500) = NULL,
    @Description NVARCHAR(500) = NULL,
    @Phone NVARCHAR(15),
    @Mobile NVARCHAR(15) = NULL,
    @Email NVARCHAR(100),
    @Website NVARCHAR(100) = NULL,
    @PostalCode NVARCHAR(10) = NULL,
    @Logo NVARCHAR(200),
    @Trn NVARCHAR(50) = NULL,
    @CountryId INT,
    @StateId INT,
    @CityId INT,
    @IsMainCompany BIT,
    @Active BIT,
    @CreatedBy NVARCHAR(50),
    @CreatedOn DateTime,
    @ModifiedBy NVARCHAR(50),
    @ModifiedOn DateTime
AS  
BEGIN  
    -- Update the company record with the provided or generated values
    UPDATE Companies  
    SET 
        --TenantId = @TenantId,
        --ParentCompanyId = @ParentCompanyId,
        Name = @Name,
        Address = @Address,
        Description = @Description,
        Phone = @Phone,
        Mobile = @Mobile,
        Email = @Email,
        Website = @Website,
        PostalCode = @PostalCode,
        Logo = @Logo,
        Trn = @Trn,
        CountryId = @CountryId,
        StateId = @StateId,
        CityId = @CityId,
        IsMainCompany = @IsMainCompany,
        Active = @Active,
        ModifiedBy = @ModifiedBy,
        ModifiedOn = GETDATE()
    WHERE Id = @Id;
END;


Go

-- Drop existing procedure if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompany_GetAllByParentCompanyAndTenantId')
    DROP PROCEDURE [dbo].[spCompany_GetAllByParentCompanyAndTenantId];
GO

CREATE PROCEDURE [dbo].[spCompany_GetAllByParentCompanyAndTenantId]
    @ParentCompanyId INT,
    @TenantId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Select parent company and child companies in a single result set
    SELECT 
        Id,
        CompanyCode,
        TenantId,
        Name,
        ParentCompanyId,
        Address,
        Description,
        Phone,
        Mobile,
        Email,
        Website,
        PostalCode,
        Logo,
        Trn,
        CountryId,
        StateId,
        CityId,
        IsMainCompany,
        Active,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM 
        dbo.Companies
    WHERE 
        (Id = @ParentCompanyId OR ParentCompanyId = @ParentCompanyId)
        OR TenantId = @TenantId;
END;
GO
