--_____________________________________________ spGetPermissionsForUser _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetUserSubscriptionPackageRoleModulePermissionsInfo') 
DROP PROCEDURE [dbo].[spGetUserSubscriptionPackageRoleModulePermissionsInfo]
GO
CREATE PROCEDURE [dbo].[spGetUserSubscriptionPackageRoleModulePermissionsInfo]      
    @UserId NVARCHAR(450)      
AS      
BEGIN          
SELECT       
	distinct
 p.TotalCompany,p.TotalUser,p.Id as PackageId,p.PackageName,r.roleName,m.ModuleName,      
    CASE       
        WHEN       
            rp.permInsert = 0 AND rp.permUpdate = 0 AND  rp.permView = 0 AND rp.permPrint = 0 AND rp.permDelete = 0 AND rp.permEmail = 0 AND rp.permNotification = 0      
        THEN 'No permission given'      
        ELSE SUBSTRING(      
            CASE WHEN rp.permInsert = 1 THEN 'Insert, ' ELSE '' END +      
            CASE WHEN rp.permUpdate = 1 THEN 'Update, ' ELSE '' END +      
            CASE WHEN rp.permView = 1 THEN 'View, ' ELSE '' END +      
            CASE WHEN rp.permPrint = 1 THEN 'Print, ' ELSE '' END +      
            CASE WHEN rp.permDelete = 1 THEN 'Delete, ' ELSE '' END +      
            CASE WHEN rp.permEmail = 1 THEN 'Email, ' ELSE '' END +      
            CASE WHEN rp.permNotification = 1 THEN 'Notification, ' ELSE '' END,      
            1, LEN(      
            CASE WHEN rp.permInsert = 1 THEN 'Insert, ' ELSE '' END +      
            CASE WHEN rp.permUpdate = 1 THEN 'Update, ' ELSE '' END +      
            CASE WHEN rp.permView = 1 THEN 'View, ' ELSE '' END +      
            CASE WHEN rp.permPrint = 1 THEN 'Print, ' ELSE '' END +      
            CASE WHEN rp.permDelete = 1 THEN 'Delete, ' ELSE '' END +      
            CASE WHEN rp.permEmail = 1 THEN 'Email, ' ELSE '' END +      
            CASE WHEN rp.permNotification = 1 THEN 'Notification, ' ELSE '' END      
        ) - 1)      
    END AS Permissions      
 FROM TenantPermissions tp      
 INNER JOIN AspNetUsers u ON u.Id = @UserId      
 INNER JOIN PackagePermissions pp ON pp.ModuleId = tp.ModuleId      
 INNER JOIN Packages p ON p.id = pp.PackageId      
 INNER JOIN Modules m ON m.Id = pp.ModuleId and m.IsForm = 1 
 INNER JOIN RolePermissions rp ON rp.ModuleId = tp.ModuleId      
 INNER JOIN Roles r ON r.id = rp.RoleId      
 WHERE     
 --tp.TenantId = 2   And    
 rp.RoleId = 1;      
END 
GO
