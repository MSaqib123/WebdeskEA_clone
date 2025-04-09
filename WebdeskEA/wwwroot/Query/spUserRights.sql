--_____________________________________________ spUserRights_BulkInsert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spUserRights_BulkInsert') DROP PROCEDURE [dbo].[spUserRights_BulkInsert]
GO
CREATE PROCEDURE [dbo].[spUserRights_BulkInsert]  
    @jsonInput NVARCHAR(MAX) -- JSON input containing user rights  
AS  
BEGIN  
    -- Parse the JSON input into a table variable  
    DECLARE @UserRightsTable TABLE  
    (  
        Id INT,  
        RoleId INT,  
        TenantId INT,  
        ModuleId INT,  
        UserId NVARCHAR(450),  
        RightInsert BIT,  
        RightUpdate BIT,  
        RightView BIT,  
        RightPrint BIT,  
        RightDelete BIT,  
        RightApprove BIT,  
        RightEmail BIT,  
        RightNotification BIT,  
        Active BIT,  
        CreatedOn DATETIME,  
        CreatedBy NVARCHAR(450),  
        ModifiedOn DATETIME,  
        ModifiedBy NVARCHAR(450)  
    );  
  
    -- Insert JSON data into the table variable  
    INSERT INTO @UserRightsTable  
    SELECT   
        Id, RoleId, TenantId, ModuleId, UserId, RightInsert, RightUpdate, RightView,   
        RightPrint, RightDelete, RightApprove, RightEmail, RightNotification,  
        -- Set Active based on rights check:  
        CASE   
            WHEN RightInsert = 0 AND RightUpdate = 0 AND RightView = 0 AND RightPrint = 0 AND RightDelete = 0 AND RightApprove = 0 AND RightEmail = 0 AND RightNotification = 0  
            THEN 0 -- All rights unchecked, set Active to false  
            ELSE 1 -- At least one right is checked, set Active to true  
        END AS Active,  
        CASE   
            WHEN TRY_CAST(CreatedOn AS DATETIME) IS NOT NULL THEN TRY_CAST(CreatedOn AS DATETIME)  
            ELSE GETDATE() -- Use current datetime if CreatedOn is null  
        END AS CreatedOn,  
        CreatedBy,  
        CASE   
            WHEN TRY_CAST(ModifiedOn AS DATETIME) IS NOT NULL THEN TRY_CAST(ModifiedOn AS DATETIME)  
            ELSE GETDATE() -- Use current datetime if ModifiedOn is null  
        END AS ModifiedOn,  
        ModifiedBy  
    FROM OPENJSON(@jsonInput)  
    WITH (  
        Id INT,  
        RoleId INT,  
        TenantId INT,  
        ModuleId INT,  
        UserId NVARCHAR(450),  
        RightInsert BIT,  
        RightUpdate BIT,  
        RightView BIT,  
        RightPrint BIT,  
        RightDelete BIT,  
        RightApprove BIT,  
        RightEmail BIT,  
        RightNotification BIT,  
        CreatedOn NVARCHAR(50), -- Initially store as string to check  
        CreatedBy NVARCHAR(450),  
        ModifiedOn NVARCHAR(50), -- Initially store as string to check  
        ModifiedBy NVARCHAR(450)  
    );  
  
    -- Get the UserId from the table  
    DECLARE @UserId NVARCHAR(450);  
    SELECT TOP 1 @UserId = UserId FROM @UserRightsTable;  
  
    -- Delete existing user rights for the user  
    DELETE FROM UserRights  
    WHERE UserId = @UserId;  
  
    -- Insert new rights from the parsed JSON input  
    INSERT INTO UserRights  
    (  
        RoleId, TenantId, ModuleId, UserId, RightInsert, RightUpdate, RightView,   
        RightPrint, RightDelete, RightApprove, RightEmail, RightNotification, Active,   
        CreatedOn, CreatedBy, ModifiedOn, ModifiedBy  
    )  
    SELECT   
        RoleId, TenantId, ModuleId, UserId, RightInsert, RightUpdate, RightView,   
        RightPrint, RightDelete, RightApprove, RightEmail, RightNotification, Active,  
        CreatedOn, CreatedBy, ModifiedOn, ModifiedBy  
    FROM @UserRightsTable;  
  
    -- Return the number of inserted rows  
    SELECT @@ROWCOUNT AS InsertedRows;  
END;
GO

--_____________________________________________ spUserRights_Delete _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spUserRights_Delete') DROP PROCEDURE [dbo].[spUserRights_Delete]
GO
create PROCEDURE [dbo].[spUserRights_Delete]
    @Id INT
AS
BEGIN
    UPDATE UserRights
    SET 
        Active = 0,
        ModifiedOn = GETDATE()
    WHERE Id = @Id;
END
GO
--_____________________________________________ spUserRights_DeletesAllRightsOfUser _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spUserRights_DeletesAllRightsOfUser') DROP PROCEDURE [dbo].[spUserRights_DeletesAllRightsOfUser]
GO
create proc [dbo].[spUserRights_DeletesAllRightsOfUser]
@UserId varchar(50)
as
begin
	delete from UserRights
	where UserId = @UserId
end
GO
--_____________________________________________ spUserRights_GetAll _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spUserRights_GetAll') DROP PROCEDURE [dbo].[spUserRights_GetAll]
GO
CREATE PROCEDURE [dbo].[spUserRights_GetAll]  
AS  
BEGIN  
    SELECT   
        ur.Id,  
        ur.ModuleId,  
        ur.UserId,  
        ur.RightInsert,  
        ur.RightUpdate,  
        ur.RightView,  
        ur.RightPrint,  
        ur.RightDelete,  
        ur.RightApprove,  
        ur.Active,  
        ur.CreatedOn,  
        ur.CreatedBy,  
        ur.ModifiedOn,  
        ur.ModifiedBy,
		--____ Relation Columns ___
		u.UserName,
		m.ModuleName
    FROM UserRights ur
	left join AspNetUsers u  on u.Id = ur.UserId
	left join Modules m on m.id = ur.ModuleId
	WHERE ur.Active = 1;
END  
GO
--_____________________________________________ spUserRights_GetAllByUserId _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spUserRights_GetAllByUserId') DROP PROCEDURE [dbo].[spUserRights_GetAllByUserId]
GO
CREATE PROCEDURE [dbo].[spUserRights_GetAllByUserId]
    @User VARCHAR(MAX)
AS  
BEGIN  
    -- Select all modules and check if the user has rights assigned to each one
    SELECT 
        ISNULL(ur.Id, 0) AS Id, -- Return 0 if no UserRights record exists
        m.Id AS ModuleId,       -- Get the ModuleId from Modules table
        @User AS UserId,      -- Ensure we show rights for the specific user
        ISNULL(ur.RightInsert, 0) AS RightInsert,  -- If no rights exist, default to 0
        ISNULL(ur.RightUpdate, 0) AS RightUpdate,
        ISNULL(ur.RightView, 0) AS RightView,
        ISNULL(ur.RightPrint, 0) AS RightPrint,
        ISNULL(ur.RightDelete, 0) AS RightDelete,
        ISNULL(ur.RightApprove, 0) AS RightApprove,
        ISNULL(ur.Active, 0) AS Active,            -- Default to 0 if no rights exist
        ISNULL(ur.CreatedOn, GETDATE()) AS CreatedOn,
        ISNULL(ur.CreatedBy, '') AS CreatedBy,
        ISNULL(ur.ModifiedOn, '') AS ModifiedOn,
        ISNULL(ur.ModifiedBy, '') AS ModifiedBy,
        -- Related columns for display
        u.UserName,            -- Display user name
        m.ModuleName           -- Display module name
    FROM Modules m
    LEFT JOIN UserRights ur ON m.Id = ur.ModuleId AND ur.UserId = @User  -- Left join to bring in user rights
    LEFT JOIN AspNetUsers u ON u.Id = @User                              -- Left join to get user details
    WHERE m.Active= 1;                                                  -- Ensure only active modules are returned
END
GO
--_____________________________________________ spUserRights_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spUserRights_GetById') DROP PROCEDURE [dbo].[spUserRights_GetById]
GO
CREATE PROCEDURE [dbo].[spUserRights_GetById]  
    @Id INT  
AS  
BEGIN  
	SELECT     
        ur.Id,    
        ur.ModuleId,    
        ur.UserId,    
        ur.RightInsert,    
        ur.RightUpdate,    
        ur.RightView,    
        ur.RightPrint,    
        ur.RightDelete,    
        ur.RightApprove,    
        ur.Active,    
        ur.CreatedOn,    
        ur.CreatedBy,    
        ur.ModifiedOn,    
        ur.ModifiedBy,  
	  --____ Relation Columns ___  
	  u.UserName,  
	  m.ModuleName  
		FROM UserRights ur  
	 left join AspNetUsers u  on u.Id = ur.UserId  
	 left join Modules m on m.id = ur.ModuleId  
	 WHERE ur.Id = @Id AND ur.Active = 1;  -- Optionally filter only active records  
END  
GO
--_____________________________________________ spUserRights_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spUserRights_Insert') DROP PROCEDURE [dbo].[spUserRights_Insert]
GO
Create PROCEDURE [dbo].[spUserRights_Insert]
    @ModuleId INT,
    @UserId NVARCHAR(450),  -- Adjust size based on your UserId field length
    @RightInsert BIT,
    @RightUpdate BIT,
    @RightView BIT,
    @RightPrint BIT,
    @RightDelete BIT,
    @RightApprove BIT,
    @Active BIT,
    @CreatedOn DATETIME,
    @CreatedBy INT,
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO UserRights (ModuleId, UserId, RightInsert, RightUpdate, RightView, RightPrint, RightDelete,   RightApprove, Active, CreatedOn, CreatedBy)
    VALUES (@ModuleId, @UserId, @RightInsert, @RightUpdate, @RightView, @RightPrint, @RightDelete, @RightApprove, @Active, @CreatedOn, @CreatedBy);
    
    SET @Id = SCOPE_IDENTITY();
END
GO
--_____________________________________________ spUserRights_Update _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spUserRights_Update') DROP PROCEDURE [dbo].[spUserRights_Update]
GO
CREATE PROCEDURE [dbo].[spUserRights_Update]
    @Id INT,
    @ModuleId INT,
    @UserId NVARCHAR(450),  -- Adjust size based on your UserId field length
    @RightInsert BIT,
    @RightUpdate BIT,
    @RightView BIT,
    @RightPrint BIT,
    @RightDelete BIT,
    @RightApprove BIT,
    @Active BIT,
    @ModifiedOn DATETIME,
    @ModifiedBy INT
AS
BEGIN
    UPDATE UserRights
    SET 
        ModuleId = @ModuleId,
        UserId = @UserId,
        RightInsert = @RightInsert,
        RightUpdate = @RightUpdate,
        RightView = @RightView,
        RightPrint = @RightPrint,
        RightDelete = @RightDelete,
        RightApprove = @RightApprove,
        Active = @Active,
        ModifiedOn = @ModifiedOn,
        ModifiedBy = @ModifiedBy
    WHERE Id = @Id;
END
GO
