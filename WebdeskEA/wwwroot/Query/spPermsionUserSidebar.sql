--___________________________________________________________________________________________________________________
--_____________________________________________ spGetUserSidebarDetails _____________________________________________ 
--___________________________________________________________________________________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetUserSidebarDetails') DROP PROCEDURE [dbo].[spGetUserSidebarDetails]
GO
--CREATE PROCEDURE [dbo].[spGetUserSidebarDetails]
--    @UserId UNIQUEIDENTIFIER
--AS
--BEGIN
--    SELECT
--        m.Id,
--        m.ModuleName,
--		m.IsForm,
--		m.IsSubModule,
--		m.IsForm,
--		m.IsTab,
--		m.IsExpand,
--		m.ParentModuleId,
--        m.ModuleUrl,
--        m.ModulePath,
--        m.ModuleIcon
--    FROM Modules m
--    JOIN UserRights ur ON ur.ModuleId = m.Id
--    WHERE ur.UserId = @UserId AND m.Active = 1 AND ur.Active = 1
--END
--GO

--_____________________________________________ spSideBarMenu _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSideBarMenu') DROP PROCEDURE [dbo].[spSideBarMenu]
GO
--CREATE PROCEDURE [dbo].[spSideBarMenu]
--    @UserId UNIQUEIDENTIFIER
--AS
--BEGIN
--    SELECT 
--        m.moduleName,
--        m.moduleUrl,
--        m.parentModuleId,
--        m.ModuleIcon
--    FROM 
--        UserRights ur
--    JOIN 
--        Modules m ON ur.moduleId = m.id
--    WHERE 
--        ur.active = 1
--        AND m.isForm = 1
--        AND ur.userId = @UserId
--END
--GO


--___________________________________________________________________________________________________________________
--_____________________________________________ Sidebar Permissin _____________________________________________ 
--___________________________________________________________________________________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetPermTenantUserSideBarByUserId')
    DROP PROCEDURE [dbo].[spGetPermTenantUserSideBarByUserId]
GO
CREATE PROCEDURE [dbo].[spGetPermTenantUserSideBarByUserId]      
    @UserId UNIQUEIDENTIFIER      
AS      
BEGIN      
    -- Declare a variable to store TenantId    
    DECLARE @TenantId INT;    
    
    -- Get the TenantId for the given UserId    
    SELECT @TenantId = TenantId      
    FROM aspnetusers      
    WHERE Id = @UserId;    
    
    -- Fetch modules for the user based on TenantPermissions and RolePermissions    
    SELECT DISTINCT    
        m.Id,    
        m.ModuleName,    
        m.IsModule,    
        m.IsSubModule,    
        m.IsForm,    
        m.SubForm,    
        m.ParentModuleId,    
        m.ModuleUrl,    
        m.ModulePath,    
        m.IsTab,    
		m.isLabel,  
		m.LabelText,  
        [m].[Order] AS [Order], -- Correct usage of "order" as a column
        m.ModuleIcon,    
        m.IsExpand,    
        m.Active,    
        m.CreatedOn,    
        m.CreatedBy,    
        m.ModifiedOn,    
        m.ModifiedBy    
    FROM     
        TenantPermissions tp    
    INNER JOIN AspNetUsers u ON u.Id = @UserId     
    INNER JOIN RolePermissions rp ON rp.RoleId = u.RoleId      
    INNER JOIN Modules m ON tp.ModuleId = m.Id AND tp.ModuleId = rp.ModuleId      
    WHERE       
        tp.TenantId = @TenantId     
        AND tp.isModuleActive = 1      
        AND m.Active = 1    
        AND (    
            -- Check if the module is 'Subscription & Upgrades' and user is tenant admin    
            (m.ModuleName = 'Subscription & Upgrades' AND u.isTenantAdmin = 1)    
            -- Include other modules if needed    
            OR (m.ModuleName <> 'Subscription & Upgrades')    
        )    
END 
GO
