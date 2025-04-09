--_____________________________________________ spCustomer_BulkInsert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCustomer_BulkInsert') DROP PROCEDURE [dbo].[spCustomer_BulkInsert]
GO
CREATE PROCEDURE [dbo].[spCustomer_BulkInsert]
    @jsonInput NVARCHAR(MAX)
AS
BEGIN
    -- BulkInsert By Json for Customer
    PRINT 'Customer BulkInsert Procedure'
    -- Handle any custom logic here
END;
GO

--_____________________________________________ spCustomer_Delete _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCustomer_Delete') DROP PROCEDURE [dbo].[spCustomer_Delete]
GO
CREATE PROCEDURE [dbo].[spCustomer_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM Customers
    WHERE Id = @Id;
END;
GO
--_____________________________________________ spCustomer_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCustomer_Insert') DROP PROCEDURE [dbo].[spCustomer_Insert]
GO
CREATE PROCEDURE [dbo].[spCustomer_Insert]
    @Code NVARCHAR(1000),
    @Name NVARCHAR(100),
    @PhoneNo NVARCHAR(15),
    @Email NVARCHAR(100),
    @CreditLimit DECIMAL(18, 2),
    @COAId INT,
    @TenantId INT,
    @CompanyId INT,
    @CountryId INT,
    @StateProvinceId INT,
    @CityId INT,
	@IsCustomerLogin BIT,
    @CustomerLoginLink NVARCHAR(200),
    @CustomerUserName NVARCHAR(100),
    @CustomerPassword NVARCHAR(100),
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
	--	EXEC dbo.GenerateCode  @TableName = 'Customers', @ColumnName = 'Code', @Prefix = 'CUS',  @NewCode  = @Code OUTPUT;
	--END
	
    INSERT INTO Customers (Code, Name, PhoneNo, Email, CreditLimit, COAId, TenantId, CompanyId, CountryId, StateProvinceId, CityId,IsCustomerLogin, CustomerLoginLink, CustomerUserName, CustomerPassword,  Active, CreatedOn, CreatedBy)
    VALUES (@Code, @Name, @PhoneNo, @Email, @CreditLimit, @COAId, @TenantId, @CompanyId, @CountryId, @StateProvinceId, @CityId, 
	@IsCustomerLogin, 
	@CustomerLoginLink, 
	@CustomerUserName, 
	@CustomerPassword, 
	@Active, GETDATE(), @CreatedBy);
    
    SET @Id = SCOPE_IDENTITY();
END;
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCustomer_Update') DROP PROCEDURE [dbo].[spCustomer_Update]
GO
CREATE PROCEDURE [dbo].[spCustomer_Update]
    @Id INT,
    @Code NVARCHAR(10),
    @Name NVARCHAR(100),
    @PhoneNo NVARCHAR(15),
    @Email NVARCHAR(100),
    @CreditLimit DECIMAL(18, 2),
    @COAId INT,
    @TenantId INT,
    @CompanyId INT,
    @CountryId INT,
    @StateProvinceId INT,
    @CityId INT,
	@IsCustomerLogin BIT,
    @CustomerLoginLink NVARCHAR(200),
    @CustomerUserName NVARCHAR(100),
    @CustomerPassword nvarchar(100),
    @Active BIT,
    @CreatedBy NVARCHAR(100),
    @CreatedOn DATETIME,
    @ModifiedBy NVARCHAR(100),
    @ModifiedOn DATETIME
AS
BEGIN
    UPDATE Customers
    SET 
        Name = @Name,
        PhoneNo = @PhoneNo,
        Email = @Email,
        CreditLimit = @CreditLimit,
        COAId = @COAId,
        TenantId = @TenantId,
        CompanyId = @CompanyId,
        CountryId = @CountryId,
        StateProvinceId = @StateProvinceId,
        CityId = @CityId,
		IsCustomerLogin = @IsCustomerLogin,
        CustomerLoginLink = @CustomerLoginLink,
        CustomerUserName = @CustomerUserName,
        CustomerPassword = @CustomerPassword,
        Active = @Active,
        ModifiedOn = GETDATE(),
        ModifiedBy = @ModifiedBy
    WHERE Id = @Id;
END;
GO



IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCustomer_GetAll') DROP PROCEDURE [dbo].[spCustomer_GetAll]
GO
CREATE PROCEDURE [dbo].[spCustomer_GetAll]
AS
BEGIN
    SELECT 
        c.Id,
        c.Code,
        c.Name,
        c.PhoneNo,
        c.Email,
        c.CreditLimit,
        c.COAId,
        c.TenantId,
        c.CompanyId,
        c.CountryId,
        c.StateProvinceId,
        c.CityId,
		c.IsCustomerLogin,
        c.CustomerLoginLink,
        c.CustomerUserName,
        c.CustomerPassword,
        c.Active,
        c.CreatedOn,
        c.CreatedBy,
        c.ModifiedOn,
        c.ModifiedBy,
		--- Joined ---
        ISNULL(t.TenantName, '--') AS TenantName,
		ISNULL(co.CountryName ,'--') as CountryName,
		ISNULL(sp.StateProvinceName   ,'--') as StateName,
		ISNULL(ci.CityName    ,'--') as CityName,    
		ISNULL(comp.Name ,'--') as CompanyName ,
		ISNULL(coa.AccountName ,'--') as AccountName 
    FROM Customers c
    LEFT JOIN Tenants t ON t.Id = c.TenantId
	Left Join Countries co on co.Id = c.CountryId
	Left Join StateProvinces sp on sp.Id = c.StateProvinceId
	Left Join Cities ci on ci.Id = c.CityId
	Left Join Companies comp on comp.Id = c.CompanyId
	Left Join Coas coa on coa.Id = c.COAId
END;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCustomer_GetById') DROP PROCEDURE [dbo].[spCustomer_GetById]
GO
CREATE PROCEDURE [dbo].[spCustomer_GetById]
    @Id INT
AS
BEGIN
     SELECT 
        c.Id,
        c.Code,
        c.Name,
        c.PhoneNo,
        c.Email,
        c.CreditLimit,
        c.COAId,
        c.TenantId,
        c.CompanyId,
        c.CountryId,
        c.StateProvinceId,
        c.CityId,
		c.IsCustomerLogin,
        c.CustomerLoginLink,
        c.CustomerUserName,
        c.CustomerPassword,
        c.Active,
        c.CreatedOn,
        c.CreatedBy,
        c.ModifiedOn,
        c.ModifiedBy,
		--- Joined ---
        ISNULL(t.TenantName, '--') AS TenantName,
		ISNULL(co.CountryName ,'--') as CountryName,
		ISNULL(sp.StateProvinceName   ,'--') as StateName,
		ISNULL(ci.CityName    ,'--') as CityName,    
		ISNULL(comp.Name ,'--') as CompanyName ,
		ISNULL(coa.AccountName ,'--') as AccountName 
    FROM Customers c
    LEFT JOIN Tenants t ON t.Id = c.TenantId
	Left Join Countries co on co.Id = c.CountryId
	Left Join StateProvinces sp on sp.Id = c.StateProvinceId
	Left Join Cities ci on ci.Id = c.CityId
	Left Join Companies comp on comp.Id = c.CompanyId
	Left Join Coas coa on coa.Id = c.COAId
    WHERE c.Id = @Id;
END;
GO



GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCustomer_GetAllByParentCompanyAndTenantId') DROP PROCEDURE [dbo].[spCustomer_GetAllByParentCompanyAndTenantId]
GO
CREATE PROCEDURE [dbo].[spCustomer_GetAllByParentCompanyAndTenantId]  
    @ParentCompanyId INT,  
    @TenantId INT  
AS  
BEGIN  
     SELECT   
        c.Id,  
        c.Code,  
        c.Name,  
        c.PhoneNo,  
        c.Email,  
        c.CreditLimit,  
        c.COAId,  
        c.TenantId,  
        c.CompanyId,  
        c.CountryId,  
        c.StateProvinceId,  
        c.CityId,  
  c.IsCustomerLogin,  
        c.CustomerLoginLink,  
        c.CustomerUserName,  
        c.CustomerPassword,  
        c.Active,  
        c.CreatedOn,  
        c.CreatedBy,  
        c.ModifiedOn,  
        c.ModifiedBy,  
	--- Joined ---  
      ISNULL(t.TenantName, '--') AS TenantName,  
	  ISNULL(co.CountryName ,'--') as CountryName,  
	  ISNULL(sp.StateProvinceName   ,'--') as StateName,  
	  ISNULL(ci.CityName    ,'--') as CityName,      
	  ISNULL(comp.Name ,'--') as CompanyName ,  
	  ISNULL(coa.AccountName ,'--') as AccountName   
    FROM Customers c  
    LEFT JOIN Tenants t ON t.Id = c.TenantId  
 Left Join Countries co on co.Id = c.CountryId  
 Left Join StateProvinces sp on sp.Id = c.StateProvinceId  
 Left Join Cities ci on ci.Id = c.CityId  
 Left Join Companies comp on comp.Id = c.CompanyId  
 Left Join Coas coa on coa.Id = c.COAId  
    WHERE   
        (c.CompanyId = @ParentCompanyId)
        AND c.TenantId = @TenantId;  
END;  