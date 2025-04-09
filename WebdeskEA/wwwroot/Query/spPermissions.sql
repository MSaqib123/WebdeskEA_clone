--_____________________________________________ GetAll Roles _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRole_GetAllRolePolicy') DROP PROCEDURE [dbo].[spRole_GetAllRolePolicy]
GO
CREATE PROCEDURE [dbo].[spRole_GetAllRolePolicy]  
AS  
BEGIN  
    SELECT * FROM Roles
END;
GO

--===========================================================================-==========================================================
--========================================================== Tenant Permission -==========================================================
--===========================================================================-==========================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetPermTenantListByUserId') 
DROP PROCEDURE [dbo].[spGetPermTenantListByUserId]
GO
CREATE PROCEDURE [dbo].[spGetPermTenantListByUserId]
    @UserId NVARCHAR(450)
AS
BEGIN
    -- Step 1: Get the roleId and CompanyId for the given user
    DECLARE @roleId INT, @CompanyId INT , @TenantId Int;

    SELECT @roleId = roleId, @CompanyId = CompanyId, @TenantId = TenantId
    FROM AspNetUsers
    WHERE Id = @UserId;

    -- Step 2: Join RolePermissions and TenantPermissions based on matching ModuleId and Tenant
    SELECT 
		distinct
        rp.ModuleId,
		rp.RoleId,
		ISNULL(rp.permInsert, 0) AS permInsert,
        ISNULL(rp.permInsert,0) AS permInsert,
        ISNULL(rp.permUpdate,0) AS permUpdate,
        ISNULL(rp.permView ,0)AS permView,
        ISNULL(rp.permPrint,0) AS permPrint ,
        ISNULL(rp.permDelete,0) AS permDelete,
        ISNULL(rp.permEmail,0) AS permEmail,
        ISNULL(rp.permNotification,0) AS permNotification,
		--____ Joined Column ____
		u.id as UserId,
		u.UserName ,
		t.TenantName,
		c.Name as CompanyName,
		r.roleName as RoleName,
		m.ModuleName ,
		t.TenantUsers,
		t.TenantCompanies
    FROM 
	RolePermissions rp
    INNER JOIN TenantPermissions tp ON rp.ModuleId = tp.ModuleId
	Inner Join Tenants t on t.Id = tp.TenantId
	inner join Modules m on m.Id = tp.ModuleId
	Inner Join AspNetUsers u on u.TenantId = tp.TenantId and u.Id = @UserId
	inner join Companies c on c.Id = u.CompanyId
	inner join Roles r on r.id = rp.RoleId
    WHERE 
        rp.RoleId = @roleId
        AND tp.TenantId = @TenantId;
END
GO


----__________________ spGetAllUserNotExistInCompanyUser __________________ 
--IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetAllUserNotExistInCompanyUser') DROP PROCEDURE [dbo].[spGetAllUserNotExistInCompanyUser]
--GO
--CREATE PROCEDURE [dbo].[spGetAllUserNotExistInCompanyUser]
--AS
--BEGIN
--    SELECT u.Id, u.UserName
--    FROM AspNetUsers u
--    LEFT JOIN CompanyUsers cu ON u.Id = cu.UserId
--    WHERE cu.UserId IS NULL;
--END;
--GO

----__________________ spGetExistingPermissionListDynamic __________________ 
--IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetExistingPermissionListDynamic') DROP PROCEDURE [dbo].[spGetExistingPermissionListDynamic]
--GO
--CREATE PROCEDURE [dbo].[spGetExistingPermissionListDynamic]
--AS
--BEGIN
--    -- Dynamically fetch all columns that start with 'Right' from the UserRights table
--    --SELECT DISTINCT 
--    --    REPLACE(COLUMN_NAME, 'Right', '') AS PermissionName
--    --FROM INFORMATION_SCHEMA.COLUMNS
--    --WHERE TABLE_NAME = 'UserRights'
--    --  AND COLUMN_NAME LIKE 'Right%';

--	SELECT DISTINCT 
--        REPLACE(COLUMN_NAME, 'perm', '') AS PermissionName
--    FROM INFORMATION_SCHEMA.COLUMNS
--    WHERE TABLE_NAME = 'RolePermissions' AND COLUMN_NAME LIKE 'perm%';
--END
--GO

----__________________  spGetPermissionsByModuleId __________________ 
--IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetPermissionsByModuleId') DROP PROCEDURE [dbo].[spGetPermissionsByModuleId]
--GO
--CREATE PROCEDURE [dbo].[spGetPermissionsByModuleId]  
--    @ModuleId INT  
--AS  
--BEGIN  
--    SET NOCOUNT ON;  

--    SELECT DISTINCT Permission
--    FROM (  
--        SELECT 'Insert' AS Permission
--        FROM UserRights  
--        WHERE ModuleId = @ModuleId  
--        AND RightInsert = 1  
--        AND Active = 1  

--        UNION ALL  

--        SELECT 'Update' AS Permission
--        FROM UserRights  
--        WHERE ModuleId = @ModuleId  
--        AND RightUpdate = 1  
--        AND Active = 1  

--        UNION ALL  

--        SELECT 'View' AS Permission
--        FROM UserRights  
--        WHERE ModuleId = @ModuleId  
--        AND RightView = 1  
--        AND Active = 1  

--        UNION ALL  

--        SELECT 'Print' AS Permission
--        FROM UserRights  
--        WHERE ModuleId = @ModuleId  
--        AND RightPrint = 1  
--        AND Active = 1  

--        UNION ALL  

--        SELECT 'Delete' AS Permission
--        FROM UserRights  
--        WHERE ModuleId = @ModuleId  
--        AND RightDelete = 1  
--        AND Active = 1  


--        UNION ALL  

--        SELECT 'Approve' AS Permission
--        FROM UserRights  
--        WHERE ModuleId = @ModuleId  
--        AND RightApprove = 1  
--        AND Active = 1  
--    ) AS Permissions;  
--END  
--GO
----_____________________________________________ spGetPermissionsByModuleIdIndexing _____________________________________________ 
--IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetPermissionsByModuleIdIndexing') DROP PROCEDURE [dbo].[spGetPermissionsByModuleIdIndexing]
--GO
--CREATE PROCEDURE [dbo].[spGetPermissionsByModuleIdIndexing]
--    @ModuleId INT  
--AS  
--BEGIN  
--    SET NOCOUNT ON;  

--    SELECT DISTINCT Permission, PermissionIndex  
--    FROM (  
--        SELECT 'Insert' AS Permission, 0 AS PermissionIndex
--        FROM UserRights  
--        WHERE ModuleId = @ModuleId  
--        AND RightInsert = 1  
--        AND Active = 1  

--        UNION ALL  

--        SELECT 'Update' AS Permission, 1 AS PermissionIndex
--        FROM UserRights  
--        WHERE ModuleId = @ModuleId  
--        AND RightUpdate = 1  
--        AND Active = 1  

--        UNION ALL  

--        SELECT 'View' AS Permission, 2 AS PermissionIndex
--        FROM UserRights  
--        WHERE ModuleId = @ModuleId  
--        AND RightView = 1  
--        AND Active = 1  

--        UNION ALL  

--        SELECT 'Print' AS Permission, 3 AS PermissionIndex
--        FROM UserRights  
--        WHERE ModuleId = @ModuleId  
--        AND RightPrint = 1  
--        AND Active = 1  

--        UNION ALL  

--        SELECT 'Delete' AS Permission, 4 AS PermissionIndex
--        FROM UserRights  
--        WHERE ModuleId = @ModuleId  
--        AND RightDelete = 1  
--        AND Active = 1  

--        UNION ALL  

--        SELECT 'Approve' AS Permission, 6 AS PermissionIndex
--        FROM UserRights  
--        WHERE ModuleId = @ModuleId  
--        AND RightApprove = 1  
--        AND Active = 1  
--    ) AS Permissions;  
--END  


