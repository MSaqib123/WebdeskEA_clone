--_____________________________________________ spRole_BulkInsert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRole_BulkInsert') DROP PROCEDURE [dbo].[spRole_BulkInsert]
GO
CREATE PROCEDURE [dbo].[spRole_BulkInsert]
    @jsonInput NVARCHAR(MAX)
AS
BEGIN
    -- Bulk insert logic for roles
    PRINT 'RoleBulkInsert procedure to add custom logic'
END
GO

--_____________________________________________ spRole_Delete _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRole_Delete') DROP PROCEDURE [dbo].[spRole_Delete]
GO
CREATE PROCEDURE [dbo].[spRole_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM Roles
    WHERE Id = @Id;
END;
GO

--_____________________________________________ spRole_GetAll _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRole_GetAll') DROP PROCEDURE [dbo].[spRole_GetAll]
GO
CREATE PROCEDURE [dbo].[spRole_GetAll]  
AS  
BEGIN  
    SELECT * FROM Roles
	where roleName not in ('SuperAdmin')
END;
GO

--_____________________________________________ spRole_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRole_GetById') DROP PROCEDURE [dbo].[spRole_GetById]
GO
CREATE PROCEDURE [dbo].[spRole_GetById]  
    @Id INT  
AS  
BEGIN  
    SELECT *
    FROM Roles  
    WHERE roleName not in ('SuperAdmin') and Id = @Id
END;
GO

--_____________________________________________ spRole_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRole_Insert') DROP PROCEDURE [dbo].[spRole_Insert]
GO
CREATE PROCEDURE [dbo].[spRole_Insert]    
    @RoleName NVARCHAR(100),
    @Active BIT,
	@CreatedBy NVARCHAR(100),
	@CreatedOn DATETIME,
	@ModifiedBy NVARCHAR(100),
	@ModifiedOn DATETIME,
	@Id INT OUT
AS    
BEGIN    
    INSERT INTO Roles (RoleName, Active, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn)
    VALUES (@RoleName, @Active, @CreatedBy, GETDATE(), @ModifiedBy, GETDATE());
	SET @Id = SCOPE_IDENTITY();
END;
GO
--_____________________________________________ spRole_Update _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRole_Update') DROP PROCEDURE [dbo].[spRole_Update]
GO
CREATE PROCEDURE [dbo].[spRole_Update]    
    @Id INT,
    @RoleName NVARCHAR(100),
    @Active BIT,
	@CreatedBy NVARCHAR(100),
	@CreatedOn DATETIME,
	@ModifiedBy NVARCHAR(100),
	@ModifiedOn DATETIME
AS    
BEGIN    
    UPDATE Roles
    SET RoleName = @RoleName,
        Active = @Active,
        ModifiedBy = @ModifiedBy,
        ModifiedOn = GETDATE()
    WHERE Id = @Id;
END;
GO
