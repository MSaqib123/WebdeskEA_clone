--_____________________________________________ spPackage_Insert _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackage_Insert') DROP PROCEDURE [dbo].[spPackage_Insert]
GO
CREATE PROCEDURE [dbo].[spPackage_Insert]
    @PackageTypeId INT,
    @PackageName NVARCHAR(100),
    @TotalCompany INT,
    @TotalUser INT,
    @Active BIT,
	@CreatedBy NVARCHAR(100),
	@CreatedOn DATETIME,
	@ModifiedBy NVARCHAR(100),
	@ModifiedOn DATETIME,
	@Id INT OUTPUT
AS
BEGIN
    INSERT INTO Packages (PackageTypeId, PackageName, TotalCompany, TotalUser, Active, CreatedOn, CreatedBy)
    VALUES (@PackageTypeId, @PackageName, @TotalCompany, @TotalUser, @Active, GETDATE(), @CreatedBy);
	SET @Id = SCOPE_IDENTITY();
END;
GO


--_____________________________________________ spPackage_Update _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackage_Update') DROP PROCEDURE [dbo].[spPackage_Update]
GO
CREATE PROCEDURE [dbo].[spPackage_Update]
    @Id INT,
    @PackageTypeId INT,
    @PackageName NVARCHAR(100),
    @TotalCompany INT,
    @TotalUser INT,
    @Active BIT,
    @CreatedBy NVARCHAR(100),
	@CreatedOn DATETIME,
	@ModifiedBy NVARCHAR(100),
	@ModifiedOn DATETIME
AS
BEGIN
    UPDATE Packages
    SET 
        PackageTypeId = @PackageTypeId,
        PackageName = @PackageName,
        TotalCompany = @TotalCompany,
        TotalUser = @TotalUser,
        Active = @Active,
        ModifiedOn = GETDATE(),
        ModifiedBy = @ModifiedBy
    WHERE Id = @Id;
END;
GO

--_____________________________________________ spPackage_Delete _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackage_Delete') DROP PROCEDURE [dbo].[spPackage_Delete]
GO
CREATE PROCEDURE [dbo].[spPackage_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM Packages
    WHERE Id = @Id;
END;
GO


--_____________________________________________ spPackage_GetAll _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackage_GetAll') DROP PROCEDURE [dbo].[spPackage_GetAll]
GO
CREATE PROCEDURE [dbo].[spPackage_GetAll]
AS
BEGIN
    SELECT 
	p.Id, 
	p.PackageTypeId, 
	p.PackageName, 
	p.TotalCompany, 
	p.TotalUser, 
	p.Active, 
	p.CreatedOn,
	p.CreatedBy,
	p.ModifiedOn,
	p.ModifiedBy,
	--____ Join Columns ____  
	isnull(pt.PackageType,'--') as PackageTypeName
    FROM Packages p
	left join PackageTypes pt on pt.id = p.PackageTypeId
END;
GO


--_____________________________________________ spPackage_GetById _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackage_GetById') DROP PROCEDURE [dbo].[spPackage_GetById]
GO
CREATE PROCEDURE [dbo].[spPackage_GetById]
    @Id INT
AS
BEGIN
    SELECT 
	p.Id, 
	p.PackageTypeId, 
	p.PackageName, 
	p.TotalCompany, 
	p.TotalUser, 
	p.Active, 
	p.CreatedOn,
	p.CreatedBy,
	p.ModifiedOn,
	p.ModifiedBy,
	--____ Join Columns ____  
	isnull(pt.PackageType,'--') as PackageTypeName
    FROM Packages p
	left join PackageTypes pt on pt.id = p.PackageTypeId
    WHERE p.Id = @Id;
END;
GO