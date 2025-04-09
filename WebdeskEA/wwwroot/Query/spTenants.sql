--_____________________________________________ spTenant_Delete _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenant_Delete') DROP PROCEDURE [dbo].[spTenant_Delete]
GO
CREATE PROCEDURE [dbo].[spTenant_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM Tenants
    WHERE Id = @Id;
END
GO

--_____________________________________________ spTenant_GetAll _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenant_GetAll') DROP PROCEDURE [dbo].[spTenant_GetAll]
GO
CREATE PROCEDURE [dbo].[spTenant_GetAll]
AS
BEGIN
    SELECT 
	t.Id, 
	t.TenantTypeId,
	t.TenantName,
	t.TenantEmail,
	t.TenantExpiryDate,
	t.TenantCompanies,
	t.TenantUsers,
	t.Active,
	t.CreatedBy,
	t.CreatedOn, 
	t.ModifiedBy,
	t.ModifiedOn,
	--____ Join Columns ____
	tt.TenantTypeName
    FROM Tenants t
	left join TenantTypes tt on tt.Id = t.TenantTypeId  
END
GO

--_____________________________________________ spTenant_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenant_GetById') DROP PROCEDURE [dbo].[spTenant_GetById]
GO
CREATE PROCEDURE [dbo].[spTenant_GetById]  
    @Id INT  
AS  
BEGIN  
    SELECT   
 t.Id,   
 t.TenantTypeId,  
 t.TenantName,  
 t.TenantEmail,  
 t.TenantExpiryDate,  
 t.TenantCompanies,  
 t.TenantUsers,  
 t.Active,  
 t.CreatedBy,  
 t.CreatedOn,   
 t.ModifiedBy,  
 t.ModifiedOn,  
 --____ Join Columns ____  
 tt.TenantTypeName  
 FROM Tenants t  
 left join TenantTypes tt on tt.Id = t.TenantTypeId  
 WHERE t.Id = @Id;  
END 
GO

--_____________________________________________ spTenant_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenant_Insert') DROP PROCEDURE [dbo].[spTenant_Insert]
GO
CREATE PROCEDURE [dbo].[spTenant_Insert]
    @TenantTypeId INT,
    @TenantName NVARCHAR(100),
    @TenantEmail NVARCHAR(100),
    @TenantExpiryDate DATETIME,
    @TenantCompanies INT,
    @TenantUsers INT,
    @Active BIT,
    @CreatedBy NVARCHAR(100),
    @CreatedOn DATETIME,
	@ModifiedBy NVARCHAR(100),
    @ModifiedOn DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO Tenants (TenantTypeId, TenantName, TenantEmail, TenantExpiryDate, TenantCompanies, TenantUsers, Active, CreatedBy, CreatedOn)
    VALUES (@TenantTypeId, @TenantName, @TenantEmail, @TenantExpiryDate, @TenantCompanies, @TenantUsers, @Active, @CreatedBy, @CreatedOn);

    SET @Id = SCOPE_IDENTITY();
END
GO

--_____________________________________________ spTenant_Update _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenant_Update') DROP PROCEDURE [dbo].[spTenant_Update]
GO
CREATE PROCEDURE [dbo].[spTenant_Update]
    @Id INT,
    @TenantTypeId INT,
    @TenantName NVARCHAR(100),
    @TenantEmail NVARCHAR(100),
    @TenantExpiryDate DATETIME,
    @TenantCompanies INT,
    @TenantUsers INT,
    @Active BIT,
	@CreatedBy NVARCHAR(100),
    @CreatedOn DATETIME,
    @ModifiedBy NVARCHAR(100),
    @ModifiedOn DATETIME
AS
BEGIN
    UPDATE Tenants
    SET TenantTypeId = @TenantTypeId,
        TenantName = @TenantName,
        TenantEmail = @TenantEmail,
        TenantExpiryDate = @TenantExpiryDate,
        TenantCompanies = @TenantCompanies,
        TenantUsers = @TenantUsers,
        Active = @Active,
        ModifiedBy = @ModifiedBy,
        ModifiedOn = @ModifiedOn
    WHERE Id = @Id;
END
GO
