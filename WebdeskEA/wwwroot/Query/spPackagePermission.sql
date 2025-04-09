--_____________________________________________ spPackagePermission_Insert _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackagePermission_Insert') DROP PROCEDURE [dbo].[spPackagePermission_Insert]
GO
CREATE PROCEDURE [dbo].[spPackagePermission_Insert]
    @PackageId INT,
    @ModuleId INT,
    @IsModuleActive BIT,
    @Id INT OUTPUT  -- Declare an output parameter to capture the inserted Id
AS
BEGIN
    -- Insert the record
    INSERT INTO PackagePermissions (PackageId, ModuleId, IsModuleActive)
    VALUES (@PackageId, @ModuleId, @IsModuleActive);

    -- Get the last inserted identity value and assign it to the output parameter
    SET @Id = SCOPE_IDENTITY();
END;
GO


--_____________________________________________ spPackagePermission_Update _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackagePermission_Update') DROP PROCEDURE [dbo].[spPackagePermission_Update]
GO
CREATE PROCEDURE [dbo].[spPackagePermission_Update]
    @Id int,
    @PackageId INT,
    @ModuleId INT,
    @IsModuleActive BIT
AS
BEGIN
    UPDATE PackagePermissions
    SET 
	PackageId = @PackageId,
	ModuleId = @ModuleId,
	IsModuleActive = @IsModuleActive
    WHERE Id = @Id;
END;
GO


--_____________________________________________ spPackagePermission_Delete _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackagePermission_Delete') DROP PROCEDURE [dbo].[spPackagePermission_Delete]
GO
CREATE PROCEDURE [dbo].[spPackagePermission_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM PackagePermissions
    WHERE Id = @Id;
END;
GO

--_____________________________________________ spPackagePermission_Delete _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spDeletePackagePermission_ByPackageId') DROP PROCEDURE [dbo].[spDeletePackagePermission_ByPackageId]
GO
CREATE PROCEDURE [dbo].[spDeletePackagePermission_ByPackageId]
    @PackageId INT
AS
BEGIN
    DELETE FROM PackagePermissions
    WHERE PackageId = @PackageId;
END;
GO


--_____________________________________________ spPackagePermission_GetAll _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackagePermission_GetAll') DROP PROCEDURE [dbo].[spPackagePermission_GetAll]
GO
CREATE PROCEDURE [dbo].[spPackagePermission_GetAll]
AS
BEGIN
    SELECT 
	pp.Id, 
	pp.PackageId, 
	pp.ModuleId,
	pp.IsModuleActive,
	--___ Joind ___
	Isnull(m.ModuleName,'Úndefined') as ModuleName,
	Isnull(p.PackageName ,'Úndefined') as PackageName 
    FROM PackagePermissions pp
	left join Modules m on m.Id = pp.ModuleId
	left join Packages p on p.Id = pp.PackageId
END;
GO


--_____________________________________________ spPackagePermission_GetById _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackagePermission_GetById') DROP PROCEDURE [dbo].[spPackagePermission_GetById]
GO
CREATE PROCEDURE [dbo].[spPackagePermission_GetById]
    @Id INT
AS
BEGIN
    SELECT 
	pp.Id, 
	pp.PackageId, 
	pp.ModuleId,
	pp.IsModuleActive,
	--___ Joind ___
	Isnull(m.ModuleName,'Úndefined') as ModuleName,
	Isnull(p.PackageName ,'Úndefined') as PackageName 
    FROM PackagePermissions pp
	left join Modules m on m.Id = pp.ModuleId
	left join Packages p on p.Id = pp.PackageId
    WHERE pp.Id = @Id;
END;
GO


--_____________________________________________ spPackagePermission_GetAllByPackageId _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackagePermission_GetAllByPackageId') DROP PROCEDURE [dbo].[spPackagePermission_GetAllByPackageId]
GO
CREATE PROCEDURE [dbo].[spPackagePermission_GetAllByPackageId]
    @PackageId INT
AS
BEGIN
    SELECT 
	pp.Id, 
	pp.PackageId, 
	pp.ModuleId,
	pp.IsModuleActive,
	--___ Joind ___
	Isnull(m.ModuleName,'Úndefined') as ModuleName,
	Isnull(p.PackageName ,'Úndefined') as PackageName 
    FROM PackagePermissions pp
	left join Modules m on m.Id = pp.ModuleId
	left join Packages p on p.Id = pp.PackageId
    WHERE pp.PackageId = @PackageId;
END;
GO

--IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBulkAddPackagePermission') 
--    DROP PROCEDURE [dbo].[spBulkAddPackagePermission]
--GO
--CREATE PROCEDURE [dbo].[spBulkAddPackagePermission]
--    @jsonInput NVARCHAR(MAX)
--AS
--BEGIN
--    DECLARE @compatibility_level INT;

--    SELECT @compatibility_level = compatibility_level 
--    FROM sys.databases 
--    WHERE name = DB_NAME(); 

--    IF (@compatibility_level >= 130)
--    BEGIN
--        DECLARE @parsedJson AS TABLE (
--            PackageId INT,
--            ModuleId INT,
--            isModuleActive BIT
--        );

--        -- Parse the JSON input and insert it into the table variable
--        INSERT INTO @parsedJson (PackageId, ModuleId, isModuleActive)
--        SELECT 
--            JSON_VALUE(value, '$.PackageId') AS PackageId,
--            JSON_VALUE(value, '$.ModuleId') AS ModuleId,
--            1 AS isModuleActive  
--        FROM OPENJSON(@jsonInput) AS jsonData;

--        INSERT INTO PackagePermissions (PackageId, ModuleId, isModuleActive)
--        SELECT PackageId, ModuleId, isModuleActive 
--        FROM @parsedJson;
--    END
--    ELSE
--    BEGIN
--        RAISERROR('The database compatibility level is less than 130. JSON functions are not supported.', 16, 1);
--    END
--END;
--G