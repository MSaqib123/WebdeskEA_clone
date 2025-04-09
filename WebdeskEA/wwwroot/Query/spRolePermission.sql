--_____________________________________________ spRolePermission_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRolePermission_Insert') DROP PROCEDURE [dbo].[spRolePermission_Insert]
GO
CREATE PROCEDURE [dbo].[spRolePermission_Insert]
    @RoleId INT,  
    @ModuleId INT,  
    @PermInsert BIT,  
    @PermUpdate BIT,  
    @PermView BIT,  
    @PermPrint BIT,  
    @PermDelete BIT,  
    @PermEmail BIT,  
    @PermNotification BIT,
    @Id INT OUT
AS
BEGIN
    -- Insert data into RolePermissions table
    INSERT INTO RolePermissions 
    (
        RoleId, 
        ModuleId, 
        permInsert, 
        permUpdate, 
        permView, 
        permPrint, 
        permDelete, 
        permEmail, 
        permNotification
    )
    VALUES 
    (
        ISNULL(@RoleId, 0), 
        ISNULL(@ModuleId, 0), 
        ISNULL(@PermInsert, 0), 
        ISNULL(@PermUpdate, 0), 
        ISNULL(@PermView, 0), 
        ISNULL(@PermPrint, 0), 
        ISNULL(@PermDelete, 0), 
        ISNULL(@PermEmail, 0), 
        ISNULL(@PermNotification, 0)
    );

    -- Set output parameter to the new ID
    SET @Id = SCOPE_IDENTITY();
END;
GO


--_____________________________________________ spRolePermission_Update _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRolePermission_Update') DROP PROCEDURE [dbo].[spRolePermission_Update]
GO
CREATE PROCEDURE [dbo].[spRolePermission_Update]    
    @Id INT,
    @PermInsert BIT,
    @PermUpdate BIT,
    @PermView BIT,
    @PermPrint BIT,
    @PermDelete BIT,
    @PermEmail BIT,
    @PermNotification BIT
AS    
BEGIN    
    UPDATE RolePermissions
    SET 
        permInsert = @PermInsert,
        permUpdate = @PermUpdate,
        permView = @PermView,
        permPrint = @PermPrint,
        permDelete = @PermDelete,
        permEmail = @PermEmail,
        permNotification = @PermNotification
    WHERE Id = @Id;
END;
GO



--_____________________________________________ spRolePermission_Delete _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRolePermission_Delete') DROP PROCEDURE [dbo].[spRolePermission_Delete]
GO
CREATE PROCEDURE [dbo].[spRolePermission_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM RolePermissions
    WHERE Id = @Id;
END;
GO

--_____________________________________________ spRolePermission_DeleteByRoleId _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRolePermission_DeleteByRoleId') DROP PROCEDURE [dbo].[spRolePermission_DeleteByRoleId]
GO
CREATE PROCEDURE [dbo].[spRolePermission_DeleteByRoleId]  
    @RoleId INT  
AS  
BEGIN  
    DELETE FROM RolePermissions  
    WHERE RoleId = @RoleId;  
END;  

GO
--_____________________________________________ spRolePermission_GetAll _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRolePermission_GetAll') DROP PROCEDURE [dbo].[spRolePermission_GetAll]
GO
CREATE PROCEDURE [dbo].[spRolePermission_GetAll]  
AS  
BEGIN  
    SELECT Id, RoleId, ModuleId, permInsert, permUpdate, permView, permPrint, permDelete, permEmail, permNotification
    FROM RolePermissions;  
END;
GO


--_____________________________________________ spRolePermission_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRolePermission_GetById') DROP PROCEDURE [dbo].[spRolePermission_GetById]
GO
CREATE PROCEDURE [dbo].[spRolePermission_GetById]  
    @Id INT  
AS  
BEGIN  
    SELECT Id, RoleId, ModuleId, permInsert, permUpdate, permView, permPrint, permDelete, permEmail, permNotification
    FROM RolePermissions
    WHERE Id = @Id;  
END;
GO


--_____________________________________________ spRolePermission_GetAllByRoleIdAsync _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRolePermission_GetAllByRoleIdAsync') 
DROP PROCEDURE [dbo].[spRolePermission_GetAllByRoleIdAsync]
GO
CREATE PROCEDURE [dbo].[spRolePermission_GetAllByRoleIdAsync]
    @RoleId NVARCHAR(450)
AS
BEGIN
    SELECT 
        m.Id AS ModuleId,
        ISNULL(rp.RoleId, @RoleId) AS RoleId,  -- Ensure the RoleId is returned, even if it's not mapped
        ISNULL(rp.permInsert, 0) AS permInsert,
        ISNULL(rp.permUpdate, 0) AS permUpdate,
        ISNULL(rp.permView, 0) AS permView,
        ISNULL(rp.permPrint, 0) AS permPrint,
        ISNULL(rp.permDelete, 0) AS permDelete,
        ISNULL(rp.permEmail, 0) AS permEmail,
        ISNULL(rp.permNotification, 0) AS permNotification,
		--____ Joined Column ____
        isNull(r.RoleName,'--') as RoleName,
        m.ModuleName
    FROM 
        Modules m
    LEFT JOIN 
        RolePermissions rp ON m.Id = rp.ModuleId AND rp.RoleId = @RoleId  -- Left join to include all modules
    LEFT JOIN 
        Roles r ON rp.RoleId = r.Id
END

