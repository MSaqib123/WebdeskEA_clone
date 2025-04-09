--_____________________________________________ spTenantPermission_Insert _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenantPermission_Insert') 
DROP PROCEDURE [dbo].[spTenantPermission_Insert]
GO
create PROCEDURE [dbo].[spTenantPermission_Insert]  
    @TenantId INT,  
    @ModuleId INT,  
    @IsModuleActive BIT,  
    @Id INT OUTPUT  
AS  
BEGIN  
    INSERT INTO TenantPermissions (TenantId, ModuleId, IsModuleActive)  
    VALUES (@TenantId, @ModuleId, @IsModuleActive);  

    -- Set the output parameter to the newly inserted identity value
    SET @Id = CAST(@@IDENTITY AS INT);
END;
GO
--_____________________________________________ spTenantPermission_Update _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenantPermission_Update') 
DROP PROCEDURE [dbo].[spTenantPermission_Update]
GO
CREATE PROCEDURE [dbo].[spTenantPermission_Update]
    @Id INT,
    @TenantId INT,
    @ModuleId INT,
    @IsModuleActive BIT
AS
BEGIN
    UPDATE TenantPermissions
    SET TenantId = @TenantId,
        ModuleId = @ModuleId,
        IsModuleActive = @IsModuleActive
    WHERE Id = @Id;
END;
GO
--_____________________________________________ spTenantPermission_Delete _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenantPermission_Delete') 
DROP PROCEDURE [dbo].[spTenantPermission_Delete]
GO
CREATE PROCEDURE [dbo].[spTenantPermission_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM TenantPermissions
    WHERE Id = @Id;
END;
GO
--_____________________________________________ spTenantPermission_GetAll _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenantPermission_GetAll') 
DROP PROCEDURE [dbo].[spTenantPermission_GetAll]
GO
CREATE PROCEDURE [dbo].[spTenantPermission_GetAll]
AS
BEGIN
    SELECT Id, TenantId, ModuleId, IsModuleActive
    FROM TenantPermissions;
END;
GO
--_____________________________________________ spTenantPermission_GetById _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenantPermission_GetById') 
DROP PROCEDURE [dbo].[spTenantPermission_GetById]
CREATE PROCEDURE [dbo].[spTenantPermission_GetById]
    @Id INT
AS
BEGIN
    SELECT Id, TenantId, ModuleId, IsModuleActive
    FROM TenantPermissions
    WHERE Id = @Id;
END;
GO

--_____________________________________________ spTenantPermission_DeleteByTenant _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTenantPermission_DeleteByTenant') 
DROP PROCEDURE [dbo].[spTenantPermission_DeleteByTenant]
GO
CREATE PROCEDURE [dbo].[spTenantPermission_DeleteByTenant]  
    @TenantId INT  
AS  
BEGIN  
    DELETE FROM TenantPermissions  
    WHERE TenantId = @TenantId;  
END;  

