IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSupplier_Insert') DROP PROCEDURE [dbo].[spSupplier_Insert]
GO
create PROCEDURE [dbo].[spSupplier_Insert]  
    @Code NVARCHAR(1000),  
    @Name NVARCHAR(100),  
    @PhoneNo NVARCHAR(15),  
    @Email NVARCHAR(100),  
    @TenantId INT,  
    @CompanyId INT,  
    @COAId INT,  
	@CreditLimit Int,
    @CountryId INT,  
    @StateProvinceId INT,  
    @CityId INT,  
	@IsSupplierLogin  BIT,  
    @SupplierLoginLink NVARCHAR(200),  
    @SupplierUserName  NVARCHAR(100),  
    @SupplierPassword  NVARCHAR(100),  
    @Active BIT,  
    @CreatedBy NVARCHAR(100),  
    @CreatedOn DATETIME,  
    @ModifiedBy NVARCHAR(100),  
    @ModifiedOn DATETIME,  
    @Id INT OUTPUT  
AS  
BEGIN  
 --declare @NewCode varchar(40);  
 --BEGIN  
 -- EXEC dbo.GenerateCode  @TableName = 'Suppliers', @ColumnName = 'Code', @Prefix = 'SUP',  @NewCode  = @Code OUTPUT;  
 --END  
  
    INSERT INTO Suppliers (Code, Name, PhoneNo, Email, TenantId, CompanyId, COAId,CreditLimit, CountryId, StateProvinceId, CityId,  IsSupplierLogin, SupplierLoginLink, SupplierUserName, SupplierPassword,Active, CreatedOn, CreatedBy)  
    VALUES (  
 @Code   
 ,@Name  
 ,@PhoneNo  
 ,@Email  
 ,@TenantId   
 ,@CompanyId  
 ,@COAId  
 ,@CreditLimit
 ,@CountryId  
 ,@StateProvinceId  
 ,@CityId  
 ,@IsSupplierLogin    
 ,@SupplierLoginLink  
 ,@SupplierUserName   
 ,@SupplierPassword   
 ,@Active, GETDATE(), @CreatedBy  
 );  
    SET @Id = SCOPE_IDENTITY();  
END;  
GO




IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSupplier_Update') DROP PROCEDURE [dbo].[spSupplier_Update]
GO
Create PROCEDURE [dbo].[spSupplier_Update]  
    @Id INT,  
    @Code NVARCHAR(10),  
    @Name NVARCHAR(100),  
    @PhoneNo NVARCHAR(15),  
    @Email NVARCHAR(100),  
    @TenantId INT,  
    @CompanyId INT,  
    @COAId INT,  
 @CreditLimit Int,  
    @CountryId INT,  
    @StateProvinceId INT,  
    @CityId INT,  
    @IsSupplierLogin BIT,  
    @SupplierLoginLink NVARCHAR(200),  
    @SupplierUserName NVARCHAR(100),  
    @SupplierPassword NVARCHAR(100),  
    @Active BIT,  
     @CreatedBy NVARCHAR(100),  
    @CreatedOn DATETIME,  
    @ModifiedBy NVARCHAR(100),  
    @ModifiedOn DATETIME  
AS  
BEGIN  
    UPDATE Suppliers  
    SET   
        Name = @Name,  
        PhoneNo = @PhoneNo,  
        Email = @Email,  
        TenantId = @TenantId,  
        CompanyId = @CompanyId,  
        COAId = @COAId,  
  CreditLimit=@CreditLimit,  
        CountryId = @CountryId,  
        StateProvinceId = @StateProvinceId,  
        CityId = @CityId,  
        IsSupplierLogin = @IsSupplierLogin,  
        SupplierLoginLink = @SupplierLoginLink,  
        SupplierUserName = @SupplierUserName,  
        SupplierPassword = @SupplierPassword,  
        Active = @Active,  
        ModifiedOn = GETDATE(),  
        ModifiedBy = @ModifiedBy  
    WHERE Id = @Id;  
END;  
GO


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSupplier_Delete') DROP PROCEDURE [dbo].[spSupplier_Delete]
GO
CREATE PROCEDURE [dbo].[spSupplier_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM Suppliers
    WHERE Id = @Id;
END;
GO



IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSupplier_GetAll') DROP PROCEDURE [dbo].[spSupplier_GetAll]
GO
CREATE PROCEDURE [dbo].[spSupplier_GetAll]
AS
BEGIN
    SELECT 
        s.Id,
        s.Code,
        s.Name,
        s.PhoneNo,
        s.Email,
        s.TenantId,
        s.CompanyId,
        s.COAId,
		s.CreditLimit,
        s.CountryId,
        s.StateProvinceId,
        s.CityId,
		s.IsSupplierLogin,
        s.SupplierLoginLink,
        s.SupplierUserName,
        s.SupplierPassword,
        s.Active,
        s.CreatedOn,
        s.CreatedBy,
        s.ModifiedOn,
        s.ModifiedBy,
        -- Join Column for Tenant Name
        ISNULL(t.TenantName, '--') AS TenantName,
		ISNULL(co.CountryName ,'--') as CountryName,
		ISNULL(sp.StateProvinceName   ,'--') as StateName,
		ISNULL(ci.CityName    ,'--') as CityName,    
		ISNULL(comp.Name ,'--') as CompanyName ,
		ISNULL(coa.AccountName ,'--') as AccountName 
    FROM Suppliers s
    LEFT JOIN Tenants t ON t.Id = s.TenantId
	Left Join Countries co on co.Id = s.CountryId
	Left Join StateProvinces sp on sp.Id = s.StateProvinceId
	Left Join Cities ci on ci.Id = s.CityId
	Left Join Companies comp on comp.Id = s.CompanyId
	Left Join Coas coa on  coa.Id = s.COAId
END;
GO



IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSupplier_GetById') DROP PROCEDURE [dbo].[spSupplier_GetById]
GO
CREATE PROCEDURE [dbo].[spSupplier_GetById]
    @Id INT
AS
BEGIN
    SELECT 
        s.Id,
        s.Code,
        s.Name,
        s.PhoneNo,
        s.Email,
        s.TenantId,
        s.CompanyId,
        s.COAId,
		s.CreditLimit,
        s.CountryId,
        s.StateProvinceId,
        s.CityId,
		s.IsSupplierLogin,
        s.SupplierLoginLink,
        s.SupplierUserName,
        s.SupplierPassword,
        s.Active,
        s.CreatedOn,
        s.CreatedBy,
        s.ModifiedOn,
        s.ModifiedBy,
        -- Join Column for Tenant Name
        ISNULL(t.TenantName, '--') AS TenantName,
		ISNULL(co.CountryName ,'--') as CountryName,
		ISNULL(sp.StateProvinceName   ,'--') as StateName,
		ISNULL(ci.CityName    ,'--') as CityName,    
		ISNULL(comp.Name ,'--') as CompanyName ,
		ISNULL(coa.AccountName ,'--') as AccountName 
    FROM Suppliers s
    LEFT JOIN Tenants t ON t.Id = s.TenantId
	Left Join Countries co on co.Id = s.CountryId
	Left Join StateProvinces sp on sp.Id = s.StateProvinceId
	Left Join Cities ci on ci.Id = s.CityId
	Left Join Companies comp on comp.Id = s.CompanyId
	Left Join Coas coa on  coa.Id = s.COAId
    WHERE s.Id = @Id;
END;
GO





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSupplier_GetAllByParentCompanyAndTenantId') DROP PROCEDURE [dbo].[spSupplier_GetAllByParentCompanyAndTenantId]
GO
CREATE PROCEDURE [dbo].[spSupplier_GetAllByParentCompanyAndTenantId]  
    @ParentCompanyId INT,  
    @TenantId INT  
AS  
BEGIN  
      SELECT   
        s.Id,  
        s.Code,  
        s.Name,  
        s.PhoneNo,  
        s.Email,  
        s.TenantId,  
        s.CompanyId,  
        s.COAId,  
		s.CreditLimit,
        s.CountryId,  
        s.StateProvinceId,  
        s.CityId,  
		s.IsSupplierLogin,  
        s.SupplierLoginLink,  
        s.SupplierUserName,  
        s.SupplierPassword,  
        s.Active,  
        s.CreatedOn,  
        s.CreatedBy,  
        s.ModifiedOn,  
        s.ModifiedBy,  
        -- Join Column for Tenant Name  
        ISNULL(t.TenantName, '--') AS TenantName,  
  ISNULL(co.CountryName ,'--') as CountryName,  
  ISNULL(sp.StateProvinceName   ,'--') as StateName,  
  ISNULL(ci.CityName    ,'--') as CityName,      
  ISNULL(comp.Name ,'--') as CompanyName ,  
  ISNULL(coa.AccountName ,'--') as AccountName   
    FROM Suppliers s  
    LEFT JOIN Tenants t ON t.Id = s.TenantId  
 Left Join Countries co on co.Id = s.CountryId  
 Left Join StateProvinces sp on sp.Id = s.StateProvinceId  
 Left Join Cities ci on ci.Id = s.CityId  
 Left Join Companies comp on comp.Id = s.CompanyId  
 Left Join Coas coa on  coa.Id = s.COAId  
    WHERE   
        (s.CompanyId = @ParentCompanyId)
        AND s.TenantId = @TenantId;  
END;  
