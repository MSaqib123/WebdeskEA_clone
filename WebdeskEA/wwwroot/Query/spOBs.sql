--===========================================================
--========================  OB Section  =====================
--===========================================================

--===========================================
-- OB Insert
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOB_Insert')
    DROP PROCEDURE [dbo].[spOB_Insert];
GO
CREATE PROCEDURE spOB_Insert
    @OBCode VARCHAR(50),
    @OBDate DATETIME,
    @TenantId INT,
    @CompanyId INT,
    @FinancialYearId INT,
    @Active BIT,
    @CreatedOn DATETIME,
    @CreatedBy VARCHAR(50),
    @ModifiedOn DATETIME,
    @ModifiedBy VARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    DECLARE @NewCode VARCHAR(50);
    EXEC dbo.GenerateCode @TableName='OBs', @ColumnName='OBCode', @Prefix='OB', @NewCode=@OBCode OUTPUT;

    INSERT INTO OBs (OBCode, OBDate, TenantId, CompanyId, FinancialYearId, Active, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy)
    VALUES (@OBCode, @OBDate, @TenantId, @CompanyId, @FinancialYearId, @Active, @CreatedOn, @CreatedBy, @ModifiedOn, @ModifiedBy);

    SET @Id = SCOPE_IDENTITY();
END;

--===========================================
-- OB Update
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOB_Update')
    DROP PROCEDURE [dbo].[spOB_Update];
GO
CREATE PROCEDURE spOB_Update
    @Id INT,
    @OBCode VARCHAR(50),
    @OBDate DATETIME,
    @TenantId INT,
    @CompanyId INT,
    @FinancialYearId INT,
    @Active BIT,
    @CreatedOn DATETIME,
    @CreatedBy VARCHAR(50),
    @ModifiedOn DATETIME,
    @ModifiedBy VARCHAR(50)
AS
BEGIN
    UPDATE OBs
    SET 
        OBCode = @OBCode,
        OBDate = @OBDate,
        TenantId = @TenantId,
        CompanyId = @CompanyId,
        FinancialYearId = @FinancialYearId,
        Active = @Active,
        ModifiedOn = @ModifiedOn,
        ModifiedBy = @ModifiedBy
    WHERE Id = @Id;
END;

--===========================================
-- OB Delete
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOB_Delete')
    DROP PROCEDURE [dbo].[spOB_Delete];
GO
CREATE PROCEDURE spOB_Delete
    @Id INT
AS
BEGIN
    DELETE FROM OBs
    WHERE Id = @Id;
END;

--===========================================
-- OB GetAll
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOB_GetAll')
    DROP PROCEDURE [dbo].[spOB_GetAll];
GO
CREATE PROCEDURE spOB_GetAll
AS
BEGIN
    SELECT 
        ob.Id,
        ob.OBCode,
        ob.OBDate,
        ob.TenantId,
        ob.CompanyId,
        ob.FinancialYearId,
        ob.Active,
        ob.CreatedOn,
        ob.CreatedBy,
        ob.ModifiedOn,
        ob.ModifiedBy
    FROM OBs ob
    INNER JOIN Tenants t ON t.Id = ob.TenantId
    INNER JOIN Companies c ON c.Id = ob.CompanyId;
END;

--===========================================
-- OB GetById
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOB_GetById')
    DROP PROCEDURE [dbo].[spOB_GetById];
GO
CREATE PROCEDURE spOB_GetById
    @Id INT
AS
BEGIN
    SELECT 
        ob.Id,
        ob.OBCode,
        ob.OBDate,
        ob.TenantId,
        ob.CompanyId,
        ob.FinancialYearId,
        ob.Active,
        ob.CreatedOn,
        ob.CreatedBy,
        ob.ModifiedOn,
        ob.ModifiedBy
    FROM OBs ob
    INNER JOIN Tenants t ON t.Id = ob.TenantId
    INNER JOIN Companies c ON c.Id = ob.CompanyId
    WHERE ob.Id = @Id;
END;

--===========================================
-- OB GetByTenant
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOB_GetByTenant')
    DROP PROCEDURE [dbo].[spOB_GetByTenant];
GO
CREATE PROCEDURE spOB_GetByTenant
    @TenantId INT
AS
BEGIN
     SELECT 
        ob.Id,
        ob.OBCode,
        ob.OBDate,
        ob.TenantId,
        ob.CompanyId,
        ob.FinancialYearId,
        ob.Active,
        ob.CreatedOn,
        ob.CreatedBy,
        ob.ModifiedOn,
        ob.ModifiedBy
     FROM OBs ob
     INNER JOIN Tenants t ON t.Id = ob.TenantId
     INNER JOIN Companies c ON c.Id = ob.CompanyId
     WHERE ob.TenantId = @TenantId;
END;

--===========================================
-- OB GetAllByTenantAndCompanyId
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOB_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spOB_GetAllByTenantAndCompanyId];
GO
CREATE PROCEDURE spOB_GetAllByTenantAndCompanyId
    @TenantId INT,
    @ParentCompanyId INT
AS
BEGIN
    SELECT 
        ob.Id,
        ob.OBCode,
        ob.OBDate,
        ob.TenantId,
        ob.CompanyId,
        ob.FinancialYearId,
        ob.Active,
        ob.CreatedOn,
        ob.CreatedBy,
        ob.ModifiedOn,
        ob.ModifiedBy
    FROM OBs ob
    INNER JOIN Tenants t ON t.Id = ob.TenantId
    INNER JOIN Companies com ON com.Id = ob.CompanyId
    WHERE
        ob.TenantId = @TenantId
        AND ob.CompanyId = @ParentCompanyId;
END;



--===========================================
-- OB GetAllByTenantAndCompanyFinancialYearId
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOB_GetAllByTenantAndCompanyFinancialYearId')
    DROP PROCEDURE [dbo].[spOB_GetAllByTenantAndCompanyFinancialYearId];
GO
CREATE PROCEDURE spOB_GetAllByTenantAndCompanyFinancialYearId
    @TenantId INT,
    @ParentCompanyId INT,
	@FinancialYearId int
AS
BEGIN
    SELECT 
        ob.Id,
        ob.OBCode,
        ob.OBDate,
        ob.TenantId,
        ob.CompanyId,
        ob.FinancialYearId,
        ob.Active,
        ob.CreatedOn,
        ob.CreatedBy,
        ob.ModifiedOn,
        ob.ModifiedBy
    FROM OBs ob
    INNER JOIN Tenants t ON t.Id = ob.TenantId
    INNER JOIN Companies com ON com.Id = ob.CompanyId
    WHERE
        ob.TenantId = @TenantId
        AND ob.CompanyId = @ParentCompanyId
		AND ob.FinancialYearId = @FinancialYearId
END;
--===========================================
-- OB GetAllNotInUsedByTenantCompanyId
-- NOTE: Replace ??? with appropriate reference table and column
--===========================================
--GO
--IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOB_GetAllNotInUsedByTenantCompanyId')
--    DROP PROCEDURE [dbo].[spOB_GetAllNotInUsedByTenantCompanyId];
--GO
--CREATE PROCEDURE spOB_GetAllNotInUsedByTenantCompanyId
--    @TenantId INT,
--    @ParentCompanyId INT,
--    @Id INT = NULL
--AS
--BEGIN
--    SET NOCOUNT ON;

--    SELECT 
--        ob.Id,
--        ob.OBCode,
--        ob.OBDate,
--        ob.TenantId,
--        ob.CompanyId,
--        ob.FinancialYearId,
--        ob.Active,
--        ob.CreatedOn,
--        ob.CreatedBy,
--        ob.ModifiedOn,
--        ob.ModifiedBy
--        -- Add any join if required
--    FROM OBs ob
--    INNER JOIN Tenants t ON t.Id = ob.TenantId
--    INNER JOIN Companies com ON com.Id = ob.CompanyId
--    WHERE ob.TenantId = @TenantId
--      AND ob.CompanyId = @ParentCompanyId
--      AND (
--            (@Id IS NOT NULL AND ob.Id = @Id) 
--            OR NOT EXISTS (
--                SELECT 1 FROM ??? -- Replace ??? with a table referencing OB (e.g., OIs?)
--                WHERE ???.OBId = ob.Id
--            )
--          );
--END;

