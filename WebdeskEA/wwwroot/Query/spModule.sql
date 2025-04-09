--_____________________________________________ spModule_Delete _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spModule_Delete') DROP PROCEDURE [dbo].[spModule_Delete]
GO
CREATE PROCEDURE [dbo].[spModule_Delete]
    @Id INT
AS
BEGIN
    UPDATE Modules
    SET 
        Active = 0,
        ModifiedOn = GETDATE()
    WHERE Id = @Id;
END
GO
--_____________________________________________ spModule_GetAll _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spModule_GetAll') DROP PROCEDURE [dbo].[spModule_GetAll]
GO
CREATE PROCEDURE [dbo].[spModule_GetAll]
AS
BEGIN
    SELECT 
        Id,
        ModuleName,
        IsModule,
        IsSubModule,
        IsForm,
        SubForm,
        ParentModuleId,
        ModuleUrl,
        ModulePath,
        IsTab,
        ModuleIcon,
        IsExpand,
        IsLabel,
        LabelText,
        Active,
        CreatedOn,
        CreatedBy,
        ModifiedOn,
        ModifiedBy
    FROM Modules
    WHERE Active = 1;  -- Optionally filter only active records
END
GO
--_____________________________________________ spModule_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spModule_GetById') DROP PROCEDURE [dbo].[spModule_GetById]
GO
CREATE PROCEDURE [dbo].[spModule_GetById]
    @Id INT
AS
BEGIN
    SELECT 
        Id,
        ModuleName,
        IsModule,
        IsSubModule,
        IsForm,
        SubForm,
        ParentModuleId,
        ModuleUrl,
        ModulePath,
        IsTab,
        ModuleIcon,
        IsExpand,
        IsLabel,
        LabelText,
        Active,
        CreatedOn,
        CreatedBy,
        ModifiedOn,
        ModifiedBy
    FROM Modules
    WHERE Id = @Id AND Active = 1;  -- Optionally filter only active records
END
GO
--_____________________________________________ spModule_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spModule_Insert') DROP PROCEDURE [dbo].[spModule_Insert]
GO
CREATE PROCEDURE [dbo].[spModule_Insert]  
    @ModuleName NVARCHAR(100),  
    @IsModule BIT,  
    @IsSubModule BIT,  
    @IsForm BIT,  
    @SubForm BIT,  
    @ParentModuleId INT = 0,  
    @ModuleUrl NVARCHAR(500) = '',  
    @ModulePath NVARCHAR(500) = '',  
    @IsTab BIT,  
    @ModuleIcon NVARCHAR(200) = '',  
    @IsExpand BIT,  
    @Active BIT,  
    @CreatedOn DATETIME,  
    @CreatedBy INT,  
	@ModifiedOn DateTime,
	@ModifiedBy int,
    @Id INT OUTPUT  
AS  
BEGIN  
    INSERT INTO Modules (ModuleName, IsModule, IsSubModule, IsForm, SubForm, ParentModuleId, ModuleUrl, ModulePath, IsTab, ModuleIcon, IsExpand, Active, CreatedOn, CreatedBy)  
    VALUES (@ModuleName, @IsModule, @IsSubModule, @IsForm, @SubForm, @ParentModuleId, @ModuleUrl, @ModulePath, @IsTab, @ModuleIcon, @IsExpand, @Active, getDate(), 0);  
      
    SET @Id = SCOPE_IDENTITY();  
END 
GO
--_____________________________________________ spModule_Update _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spModule_Update') DROP PROCEDURE [dbo].[spModule_Update]
GO
CREATE PROCEDURE [dbo].[spModule_Update]  
    @Id INT,  
    @ModuleName NVARCHAR(100),  
    @IsModule BIT,  
    @IsSubModule BIT,  
    @IsForm BIT,  
    @SubForm BIT,  
    @ParentModuleId INT = 0,  
    @ModuleUrl NVARCHAR(500) = '',  
    @ModulePath NVARCHAR(500) = '',  
    @IsTab BIT,  
    @ModuleIcon NVARCHAR(200) = '',  
    @IsExpand BIT,  
    @Active BIT, 
	@CreatedOn DATETIME,  
    @CreatedBy INT,  
    @ModifiedOn DATETIME,  
    @ModifiedBy INT  
AS  
BEGIN  
	-- Validation
	IF @ModuleUrl IS NULL
	BEGIN
		SET @ModuleUrl = ''
	END

	IF @ModulePath IS NULL
	BEGIN
		SET @ModulePath = ''
	END

	UPDATE Modules
    SET   
        ModuleName = @ModuleName,  
        IsModule = @IsModule,  
        IsSubModule = @IsSubModule,  
        IsForm = @IsForm,  
        SubForm = @SubForm,  
        ParentModuleId = @ParentModuleId,  
        ModuleUrl = @ModuleUrl,  
        ModulePath = @ModulePath,  
        IsTab = @IsTab,  
        ModuleIcon = @ModuleIcon,  
        IsExpand = @IsExpand,  
        Active = @Active,  
        ModifiedOn = @ModifiedOn,  
        ModifiedBy = 0
    WHERE Id = @Id;  

	SET @Id = SCOPE_IDENTITY();  
END
