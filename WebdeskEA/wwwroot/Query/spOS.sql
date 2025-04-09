--===========================================================
--========================  OS Section  ======================
--===========================================================

--===========================================
-- OS Insert
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOS_Insert')
    DROP PROCEDURE [dbo].[spOS_Insert];
GO
CREATE PROCEDURE spOS_Insert
    @OSCode VARCHAR(50),
    @OSDate DATETIME,
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
    INSERT INTO OSs (OSCode, OSDate, TenantId, CompanyId, FinancialYearId, Active, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy)
    VALUES (@OSCode, @OSDate, @TenantId, @CompanyId, @FinancialYearId, @Active, @CreatedOn, @CreatedBy, @ModifiedOn, @ModifiedBy);

    SET @Id = SCOPE_IDENTITY();
END;

--===========================================
-- OS Update
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOS_Update')
    DROP PROCEDURE [dbo].[spOS_Update];
GO
CREATE PROCEDURE spOS_Update
    @Id INT,
    @OSCode VARCHAR(50),
    @OSDate DATETIME,
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
    UPDATE OSs
    SET 
        OSCode = @OSCode,
        OSDate = @OSDate,
        TenantId = @TenantId,
        CompanyId = @CompanyId,
        FinancialYearId = @FinancialYearId,
        Active = @Active,
        ModifiedOn = @ModifiedOn,
        ModifiedBy = @ModifiedBy
    WHERE Id = @Id;
END;

--===========================================
-- OS Delete
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOS_Delete')
    DROP PROCEDURE [dbo].[spOS_Delete];
GO
CREATE PROCEDURE spOS_Delete
    @Id INT
AS
BEGIN
    DELETE FROM OSs
    WHERE Id = @Id;
END;

--===========================================
-- OS GetAll
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOS_GetAll')
    DROP PROCEDURE [dbo].[spOS_GetAll];
GO
CREATE PROCEDURE spOS_GetAll
AS
BEGIN
    SELECT 
        os.Id,
        os.OSCode,
        os.OSDate,
        os.TenantId,
        os.CompanyId,
        os.FinancialYearId,
        os.Active,
        os.CreatedOn,
        os.CreatedBy,
        os.ModifiedOn,
        os.ModifiedBy
    FROM OSs os
    INNER JOIN Tenants t ON t.Id = os.TenantId
    INNER JOIN Companies c ON c.Id = os.CompanyId;
END;

--===========================================
-- OS GetById
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOS_GetById')
    DROP PROCEDURE [dbo].[spOS_GetById];
GO
CREATE PROCEDURE spOS_GetById
    @Id INT
AS
BEGIN
    SELECT 
        os.Id,
        os.OSCode,
        os.OSDate,
        os.TenantId,
        os.CompanyId,
        os.FinancialYearId,
        os.Active,
        os.CreatedOn,
        os.CreatedBy,
        os.ModifiedOn,
        os.ModifiedBy
    FROM OSs os
    INNER JOIN Tenants t ON t.Id = os.TenantId
    INNER JOIN Companies c ON c.Id = os.CompanyId
    WHERE os.Id = @Id;
END;

--===========================================
-- OS GetByTenant
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOS_GetByTenant')
    DROP PROCEDURE [dbo].[spOS_GetByTenant];
GO
CREATE PROCEDURE spOS_GetByTenant
    @TenantId INT
AS
BEGIN
     SELECT 
        os.Id,
        os.OSCode,
        os.OSDate,
        os.TenantId,
        os.CompanyId,
        os.FinancialYearId,
        os.Active,
        os.CreatedOn,
        os.CreatedBy,
        os.ModifiedOn,
        os.ModifiedBy
     FROM OSs os
     INNER JOIN Tenants t ON t.Id = os.TenantId
     INNER JOIN Companies c ON c.Id = os.CompanyId
     WHERE os.TenantId = @TenantId;
END;

--===========================================
-- OS GetAllByTenantAndCompanyId
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOS_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spOS_GetAllByTenantAndCompanyId];
GO
CREATE PROCEDURE spOS_GetAllByTenantAndCompanyId
    @TenantId INT,
    @ParentCompanyId INT
AS
BEGIN
    SELECT 
        os.Id,
        os.OSCode,
        os.OSDate,
        os.TenantId,
        os.CompanyId,
        os.FinancialYearId,
        os.Active,
        os.CreatedOn,
        os.CreatedBy,
        os.ModifiedOn,
        os.ModifiedBy
    FROM OSs os
    INNER JOIN Tenants t ON t.Id = os.TenantId
    INNER JOIN Companies com ON com.Id = os.CompanyId
    WHERE
        os.TenantId = @TenantId
        AND os.CompanyId = @ParentCompanyId;
END;



GO
--===========================================
-- OS spOS_GetAllByTenantAndCompanyFinancialYearId
--===========================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOS_GetAllByTenantAndCompanyFinancialYearId')
    DROP PROCEDURE [dbo].[spOS_GetAllByTenantAndCompanyFinancialYearId];
GO
CREATE PROCEDURE spOS_GetAllByTenantAndCompanyFinancialYearId
    @TenantId INT,
    @ParentCompanyId INT,
	@FinancialYearId int
AS
BEGIN
    SELECT 
        os.Id,
        os.OSCode,
        os.OSDate,
        os.TenantId,
        os.CompanyId,
        os.FinancialYearId,
        os.Active,
        os.CreatedOn,
        os.CreatedBy,
        os.ModifiedOn,
        os.ModifiedBy
    FROM OSs os
    INNER JOIN Tenants t ON t.Id = os.TenantId
    INNER JOIN Companies com ON com.Id = os.CompanyId
    WHERE
        os.TenantId = @TenantId
        AND os.CompanyId = @ParentCompanyId
		AND os.FinancialYearId = @FinancialYearId
END;

--===========================================
-- OS GetAllNotInUsedByTenantCompanyId
-- NOTE: Replace ??? with appropriate reference table and column
--===========================================
--GO
--IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOS_GetAllNotInUsedByTenantCompanyId')
--    DROP PROCEDURE [dbo].[spOS_GetAllNotInUsedByTenantCompanyId];
--GO
--CREATE PROCEDURE spOS_GetAllNotInUsedByTenantCompanyId
--    @TenantId INT,
--    @ParentCompanyId INT,
--    @Id INT = NULL
--AS
--BEGIN
--    SET NOCOUNT ON;

--    SELECT 
--        os.Id,
--        os.OSCode,
--        os.OSDate,
--        os.TenantId,
--        os.CompanyId,
--        os.FinancialYearId,
--        os.Active,
--        os.CreatedOn,
--        os.CreatedBy,
--        os.ModifiedOn,
--        os.ModifiedBy
--    FROM OSs os
--    INNER JOIN Tenants t ON t.Id = os.TenantId
--    INNER JOIN Companies com ON com.Id = os.CompanyId
--    WHERE os.TenantId = @TenantId
--      AND os.CompanyId = @ParentCompanyId
--      AND (
--            (@Id IS NOT NULL AND os.Id = @Id) 
--            OR NOT EXISTS (
--                SELECT 1 FROM ??? -- Replace ??? with a table referencing OS (similar to PIs for POs)
--                WHERE ???.OSId = os.Id
--            )
--          );
--END;