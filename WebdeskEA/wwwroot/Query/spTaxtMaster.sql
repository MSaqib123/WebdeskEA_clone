--CREATE TABLE TaxMasters
--(
--    Id INT IDENTITY(1,1) PRIMARY KEY,
--    TaxName NVARCHAR(100) NOT NULL,
--	  COAId int null,
--    TaxValue DECIMAL(18,2) NOT NULL,
--    IsInclusive BIT NOT NULL,
--    IsExclusive BIT NOT NULL,
--    IsPercentage BIT NOT NULL,
--    IsFix BIT NOT NULL,
--    TenantId INT NOT NULL,
--    TenantCompanyId INT NOT NULL,
--    Active BIT NOT NULL,
--    CreatedBy NVARCHAR(100) NOT NULL,
--    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),
--    ModifiedBy NVARCHAR(100) NULL,
--    ModifiedOn DATETIME NULL
--);


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTaxMaster_BulkInsert')
    DROP PROCEDURE [dbo].[spTaxMaster_BulkInsert];
GO

CREATE PROCEDURE [dbo].[spTaxMaster_BulkInsert]
    @jsonInput NVARCHAR(MAX)
AS
BEGIN
    --SET NOCOUNT ON;

    --INSERT INTO TaxMasters (TaxName, TaxValue, IsInclusive, IsExclusive, IsPercentage, IsFix, TenantId, TenantCompanyId, Active, CreatedBy, CreatedOn)
    --SELECT 
    --    TaxName, TaxValue, IsInclusive, IsExclusive, IsPercentage, IsFix, TenantId, TenantCompanyId, Active, CreatedBy, GETDATE()
    --FROM OPENJSON(@jsonInput)
    --WITH (
    --    TaxName NVARCHAR(100),
    --    TaxValue DECIMAL(18,2),
    --    IsInclusive BIT,
    --    IsExclusive BIT,
    --    IsPercentage BIT,
    --    IsFix BIT,
    --    TenantId INT,
    --    TenantCompanyId INT,
    --    Active BIT,
    --    CreatedBy NVARCHAR(100)
    --);
	print ''
END;
GO





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTaxMaster_Delete')
    DROP PROCEDURE [dbo].[spTaxMaster_Delete];
GO

CREATE PROCEDURE [dbo].[spTaxMaster_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM TaxMasters WHERE Id = @Id;
END;
GO


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTaxMaster_Insert')
    DROP PROCEDURE [dbo].[spTaxMaster_Insert];
GO

CREATE PROCEDURE [dbo].[spTaxMaster_Insert]
    @TaxName NVARCHAR(100),
    @TaxValue DECIMAL(18,2),
	@COAId int,
    @IsInclusive BIT,
    @IsExclusive BIT,
    @IsPercentage BIT,
    @IsFix BIT,
    @TenantId INT,
    @TenantCompanyId INT,
    @Active BIT,
	@CreatedOn datetime,
	@CreatedBy NVARCHAR(100),
	@ModifiedOn datetime,
	@ModifiedBy NVARCHAR(100),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO TaxMasters (TaxName,COAId, TaxValue, IsInclusive, IsExclusive, IsPercentage, IsFix, TenantId, TenantCompanyId, Active, CreatedBy, CreatedOn)
    VALUES (@TaxName,@COAId, @TaxValue, @IsInclusive, @IsExclusive, @IsPercentage, @IsFix, @TenantId, @TenantCompanyId, @Active, @CreatedBy, GETDATE());

    SET @Id = SCOPE_IDENTITY();
END;
GO





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTaxMaster_Update')
    DROP PROCEDURE [dbo].[spTaxMaster_Update];
GO

CREATE PROCEDURE [dbo].[spTaxMaster_Update]
    @Id INT,
    @TaxName NVARCHAR(100),
	@COAId int,
    @TaxValue DECIMAL(18,2),
    @IsInclusive BIT,
    @IsExclusive BIT,
    @IsPercentage BIT,
    @IsFix BIT,
    @TenantId INT,
    @TenantCompanyId INT,
    @Active bit,
	@CreatedOn datetime,
	@CreatedBy NVARCHAR(100),
	@ModifiedOn datetime,
	@ModifiedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE TaxMasters
    SET 
        TaxName = @TaxName,
		COAId = @COAId,
        TaxValue = @TaxValue,
        IsInclusive = @IsInclusive,
        IsExclusive = @IsExclusive,
        IsPercentage = @IsPercentage,
        IsFix = @IsFix,
        TenantId = @TenantId,
        TenantCompanyId = @TenantCompanyId,
        Active = @Active,
        ModifiedBy = @ModifiedBy,
        ModifiedOn = GETDATE()
    WHERE 
        Id = @Id;
END;
GO







IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTaxMaster_GetAll')
    DROP PROCEDURE [dbo].[spTaxMaster_GetAll];
GO

CREATE PROCEDURE [dbo].[spTaxMaster_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.Id,
        t.TaxName,
		t.COAId,
        t.TaxValue,
        t.IsInclusive,
        t.IsExclusive,
        t.IsPercentage,
        t.IsFix,
        t.TenantId,
        t.TenantCompanyId,
        t.Active,
        t.CreatedBy,
        t.CreatedOn,
        t.ModifiedBy,
        t.ModifiedOn,
		---- JOined column ----
		coalesce(coas.AccountName,'') as AccountName 
    FROM TaxMasters t
	left join coas on coas.Id = t.COAId
END;
GO






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTaxMaster_GetById')
    DROP PROCEDURE [dbo].[spTaxMaster_GetById];
GO

CREATE PROCEDURE [dbo].[spTaxMaster_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.Id,
        t.TaxName,
		t.COAId,
        t.TaxValue,
        t.IsInclusive,
        t.IsExclusive,
        t.IsPercentage,
        t.IsFix,
        t.TenantId,
        t.TenantCompanyId,
        t.Active,
        t.CreatedBy,
        t.CreatedOn,
        t.ModifiedBy,
        t.ModifiedOn,
		---- JOined column ----
		coalesce(coas.AccountName,'') as AccountName
    FROM TaxMasters t
	left join coas on coas.Id = t.COAId
    WHERE 
        t.Id = @Id;
END;
GO





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTaxMaster_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spTaxMaster_GetAllByTenantAndCompanyId];
GO

CREATE PROCEDURE [dbo].[spTaxMaster_GetAllByTenantAndCompanyId]
    @TenantId INT,
    @ParentCompanyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.Id,
        t.TaxName,
		t.COAId,
        t.TaxValue,
        t.IsInclusive,
        t.IsExclusive,
        t.IsPercentage,
        t.IsFix,
        t.TenantId,
        t.TenantCompanyId,
        t.Active,
        t.CreatedBy,
        t.CreatedOn,
        t.ModifiedBy,
        t.ModifiedOn,
		---- JOined column ----
		coalesce(coas.AccountName,'') as AccountName
    FROM TaxMasters t
	left join coas on coas.Id = t.COAId
    WHERE 
        t.TenantId = @TenantId AND t.TenantCompanyId = @ParentCompanyId;
END;
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spTaxMaster_GetByIdTenantId')
    DROP PROCEDURE [dbo].[spTaxMaster_GetByIdTenantId];
GO
CREATE PROCEDURE [dbo].[spTaxMaster_GetByIdTenantId]
    @Id INT,
	@TenantId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.Id,
        t.TaxName,
		t.COAId,
        t.TaxValue,
        t.IsInclusive,
        t.IsExclusive,
        t.IsPercentage,
        t.IsFix,
        t.TenantId,
        t.TenantCompanyId,
        t.Active,
        t.CreatedBy,
        t.CreatedOn,
        t.ModifiedBy,
        t.ModifiedOn,
		---- JOined column ----
		coalesce(coas.AccountName,'') as AccountName
    FROM TaxMasters t
	left join coas on coas.Id = t.COAId
    WHERE 
        t.Id = @Id and t.TenantId = @TenantId;
END;
