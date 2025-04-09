--_____________________________________________ spTenantType_Delete _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenantType_Delete') DROP PROCEDURE [dbo].[spTenantType_Delete]
GO
CREATE PROCEDURE [dbo].[spTenantType_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM TenantTypes
    WHERE Id = @Id;
END
GO

--_____________________________________________ spTenantType_GetAll _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenantType_GetAll') DROP PROCEDURE [dbo].[spTenantType_GetAll]
GO
CREATE PROCEDURE [dbo].[spTenantType_GetAll]
AS
BEGIN
    SELECT Id, TenantTypeName, Active, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM TenantTypes;
END
GO

--_____________________________________________ spTenantType_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenantType_GetById') DROP PROCEDURE [dbo].[spTenantType_GetById]
GO
CREATE PROCEDURE [dbo].[spTenantType_GetById]
    @Id INT
AS
BEGIN
    SELECT Id, TenantTypeName, Active, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM TenantTypes
    WHERE Id = @Id;
END
GO

--_____________________________________________ spTenantType_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenantType_Insert') DROP PROCEDURE [dbo].[spTenantType_Insert]
GO
create PROCEDURE [dbo].[spTenantType_Insert]  
    @TenantTypeName NVARCHAR(100),  
    @Active BIT,  
    @CreatedBy NVARCHAR(100),  
    @CreatedOn DATETIME,  
	@ModifiedBy NVARCHAR(100) ,  
    @ModifiedOn DATETIME,  
 @Id INT OUTPUT  
AS  
BEGIN  
    INSERT INTO TenantTypes (TenantTypeName, Active, CreatedBy, CreatedOn)  
    VALUES (@TenantTypeName, @Active, @CreatedBy, @CreatedOn);  
  
    SET @Id = SCOPE_IDENTITY();  
END  
GO

--_____________________________________________ spTenantType_Update _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenantType_Update') DROP PROCEDURE [dbo].[spTenantType_Update]
GO
create PROCEDURE [dbo].[spTenantType_Update]  
    @Id INT,  
    @TenantTypeName NVARCHAR(100),  
    @Active BIT,  
    @ModifiedBy NVARCHAR(100),  
    @ModifiedOn DATETIME,  
	@CreatedBy NVARCHAR(100),  
    @CreatedOn DATETIME  
AS  
BEGIN  
    UPDATE TenantTypes  
    SET TenantTypeName = @TenantTypeName,  
        Active = @Active,  
        ModifiedBy = @ModifiedBy,  
        ModifiedOn = @ModifiedOn  
    WHERE Id = @Id;  
END  
GO
