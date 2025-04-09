--_____________________________________________ spPackageType_Insert _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackageType_Insert') DROP PROCEDURE [dbo].[spPackageType_Insert]
GO
CREATE PROCEDURE [dbo].[spPackageType_Insert]
    @PackageType NVARCHAR(100),
    @Id int out
AS
BEGIN
    INSERT INTO PackageTypes (PackageType)
    VALUES (@PackageType);
    SET @Id = SCOPE_IDENTITY();
END;
GO

--_____________________________________________ spPackageType_Update _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackageType_Update') DROP PROCEDURE [dbo].[spPackageType_Update]
GO
CREATE PROCEDURE [dbo].[spPackageType_Update]
    @Id INT,
    @PackageType NVARCHAR(100)
AS
BEGIN
    UPDATE PackageTypes
    SET PackageType = @PackageType
    WHERE Id = @Id;
END;
GO


--_____________________________________________ spPackageType_Delete _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackageType_Delete') DROP PROCEDURE [dbo].[spPackageType_Delete]
GO
CREATE PROCEDURE [dbo].[spPackageType_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM PackageTypes
    WHERE Id = @Id;
END;
GO


--_____________________________________________ spPackageType_GetAll _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackageType_GetAll') DROP PROCEDURE [dbo].[spPackageType_GetAll]
GO
CREATE PROCEDURE [dbo].[spPackageType_GetAll]
AS
BEGIN
    SELECT Id, PackageType
    FROM PackageTypes;
END;
GO


--_____________________________________________ spPackageType_GetById _____________________________________________
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPackageType_GetById') DROP PROCEDURE [dbo].[spPackageType_GetById]
GO
CREATE PROCEDURE [dbo].[spPackageType_GetById]
    @Id INT
AS
BEGIN
    SELECT Id, PackageType
    FROM PackageTypes
    WHERE Id = @Id;
END;
GO



select * from AspNetUsers
Id	Discriminator	Name	StreetAddress	City	State	PostalCode	ProfileImage	CompanyId	isTenantAdmin	roleId	Active	CreatedBy	CreatedOn	ModifiedBy	ModifiedOn	UserName	NormalizedUserName	Email	NormalizedEmail	EmailConfirmed	PasswordHash	SecurityStamp	ConcurrencyStamp	PhoneNumber	PhoneNumberConfirmed	TwoFactorEnabled	LockoutEnd	LockoutEnabled	AccessFailedCount
