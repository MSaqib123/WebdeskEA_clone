
-- Create Vouchers table
--CREATE TABLE Vouchers (
--    Id INT IDENTITY(1,1) PRIMARY KEY,
--    VoucherTypeId INT NOT NULL FOREIGN KEY REFERENCES VoucherTypes(Id),
--    VoucherCode NVARCHAR(50) NOT NULL,
--    VoucherNarration NVARCHAR(255),
--    VoucherInvoiceNo NVARCHAR(50),
--    TenantId INT NOT NULL,
--    TenantCompanyId INT NOT NULL,
--    Active BIT NOT NULL DEFAULT 1,
--    CreatedBy NVARCHAR(100),
--    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),
--    ModifiedBy NVARCHAR(100),
--    ModifiedOn DATETIME NULL
--);






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucher_Delete') 
    DROP PROCEDURE [dbo].[spVoucher_Delete]
GO
CREATE PROCEDURE [dbo].[spVoucher_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM Voucher
    WHERE Id = @Id;
END
GO




IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucher_GetAll') 
    DROP PROCEDURE [dbo].[spVoucher_GetAll]
GO
CREATE PROCEDURE [dbo].[spVoucher_GetAll]
AS
BEGIN
    SELECT 
        v.Id
        ,v.VoucherTypeId
		,v.VoucherType
        ,v.VoucherCode
        ,v.VoucherNarration
        ,v.VoucherInvoiceNo
        ,v.TenantId
        ,v.TenantCompanyId
        ,v.Active
        ,v.CreatedBy
        ,v.CreatedOn
        ,v.ModifiedBy
        ,v.ModifiedOn
		------------- Join Column ----------------
		,coalesce(vt.VoucherTypeName,'-') as VoucherTypeName
    FROM Voucher v
	left join VoucherType vt on vt.VoucherTypeName = v.VoucherType
END
GO







IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucher_GetById') 
    DROP PROCEDURE [dbo].[spVoucher_GetById]
GO
CREATE PROCEDURE [dbo].[spVoucher_GetById]
    @Id INT
AS
BEGIN
    SELECT 
        v.Id
        ,v.VoucherTypeId
		,v.VoucherType
        ,v.VoucherCode
        ,v.VoucherNarration
        ,v.VoucherInvoiceNo
        ,v.TenantId
        ,v.TenantCompanyId
        ,v.Active
        ,v.CreatedBy
        ,v.CreatedOn
        ,v.ModifiedBy
        ,v.ModifiedOn
		------------- Join Column ----------------
		,coalesce(vt.VoucherTypeName,'-') as VoucherTypeName
    FROM Voucher v
	left join VoucherType vt on vt.VoucherTypeName = v.VoucherType
    WHERE v.Id = @Id;
END
GO





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucher_GetByTenantIdCompanyId') 
    DROP PROCEDURE [dbo].[spVoucher_GetByTenantIdCompanyId]
GO
CREATE PROCEDURE [dbo].[spVoucher_GetByTenantIdCompanyId]
    @Id INT,
	@TenantId INT,
	@CompanyId INT
AS
BEGIN
    SELECT 
        v.Id
        ,v.VoucherTypeId
		,v.VoucherType
        ,v.VoucherCode
        ,v.VoucherNarration
        ,v.VoucherInvoiceNo
        ,v.TenantId
        ,v.TenantCompanyId
        ,v.Active
        ,v.CreatedBy
        ,v.CreatedOn
        ,v.ModifiedBy
        ,v.ModifiedOn
		------------- Join Column ----------------
		,coalesce(vt.VoucherTypeName,'-') as VoucherTypeName
    FROM Voucher v
	left join VoucherType vt on vt.VoucherTypeName = v.VoucherType
    WHERE 
	v.Id = @Id and
	v.TenantId = @TenantId and
	v.TenantCompanyId = @CompanyId
END
GO



IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucher_Insert') 
    DROP PROCEDURE [dbo].[spVoucher_Insert]
GO
alter PROCEDURE [dbo].[spVoucher_Insert]
    @VoucherTypeId INT,
	@VoucherType varchar(50),
    @VoucherCode NVARCHAR(50),
    @VoucherNarration NVARCHAR(255) = NULL,
    @VoucherInvoiceNo NVARCHAR(50) = NULL,
    @TenantId INT,
    @TenantCompanyId INT,
    @Active BIT = 1,
    @CreatedBy NVARCHAR(100),
    @CreatedOn DATETIME = NULL,
	@ModifiedBy NVARCHAR(100),
    @ModifiedOn DATETIME = NULL,
	@FinancialYearId int,
    @Id INT OUTPUT
AS
BEGIN
    SET @CreatedOn = COALESCE(@CreatedOn, GETDATE());
    
    BEGIN TRANSACTION;

    BEGIN TRY
        INSERT INTO Voucher
        (
            VoucherTypeId,
			VoucherType,
            VoucherCode,
            VoucherNarration,
            VoucherInvoiceNo,
            TenantId,
            TenantCompanyId,
            Active,
            CreatedBy,
            CreatedOn,
			FinancialYearId
        )
        VALUES
        (
            @VoucherTypeId,
			@VoucherType,
            @VoucherCode,
            @VoucherNarration,
            @VoucherInvoiceNo,
            @TenantId,
            @TenantCompanyId,
            @Active,
            @CreatedBy,
            @CreatedOn,
			@FinancialYearId
        );

        SET @Id = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucher_Update') 
    DROP PROCEDURE [dbo].[spVoucher_Update]
GO
CREATE PROCEDURE [dbo].[spVoucher_Update]
    @Id INT,
    @VoucherTypeId INT,
	@VoucherType varchar(50),
    @VoucherCode NVARCHAR(50),
    @VoucherNarration NVARCHAR(255) = NULL,
    @VoucherInvoiceNo NVARCHAR(50) = NULL,
    @TenantId INT,
    @TenantCompanyId INT,
    @Active BIT,
    @CreatedBy NVARCHAR(100),
    @CreatedOn DATETIME = NULL,
	@ModifiedBy NVARCHAR(100),
    @ModifiedOn DATETIME = NULL
AS
BEGIN
    SET @ModifiedOn = COALESCE(@ModifiedOn, GETDATE());

    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE Voucher
        SET 
            VoucherTypeId = @VoucherTypeId,
			VoucherType = @VoucherType,
            VoucherCode = @VoucherCode,
            VoucherNarration = @VoucherNarration,
            VoucherInvoiceNo = @VoucherInvoiceNo,
            TenantId = @TenantId,
            TenantCompanyId = @TenantCompanyId,
            Active = @Active,
            ModifiedBy = @ModifiedBy,
            ModifiedOn = @ModifiedOn
        WHERE 
            Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO







IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucher_GetByTenant') 
    DROP PROCEDURE [dbo].[spVoucher_GetByTenant]
GO
CREATE PROCEDURE [dbo].[spVoucher_GetByTenant]
    @TenantId INT
AS
BEGIN
    SELECT 
        v.Id
        ,v.VoucherTypeId
		,v.VoucherType
        ,v.VoucherCode
        ,v.VoucherNarration
        ,v.VoucherInvoiceNo
        ,v.TenantId
        ,v.TenantCompanyId
        ,v.Active
        ,v.CreatedBy
        ,v.CreatedOn
        ,v.ModifiedBy
        ,v.ModifiedOn
		------------- Join Column ----------------
		,coalesce(vt.VoucherTypeName,'-') as VoucherTypeName
    FROM Voucher v
	left join VoucherType vt on vt.VoucherTypeName = v.VoucherType
    WHERE v.TenantId = @TenantId;
END
GO






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucher_GetAllByCompanyId') 
    DROP PROCEDURE [dbo].[spVoucher_GetAllByCompanyId]
GO
CREATE PROCEDURE spVoucher_GetAllByCompanyId
    @CompanyId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        v.Id
        ,v.VoucherTypeId
		,v.VoucherType
        ,v.VoucherCode
        ,v.VoucherNarration
        ,v.VoucherInvoiceNo
        ,v.TenantId
        ,v.TenantCompanyId
        ,v.Active
        ,v.CreatedBy
        ,v.CreatedOn
        ,v.ModifiedBy
        ,v.ModifiedOn
		------------- Join Column ----------------
		,coalesce(vt.VoucherTypeName,'-') as VoucherTypeName
    FROM Voucher v
	left join VoucherType vt on vt.VoucherTypeName = v.VoucherType
    WHERE 
        (@CompanyId IS NULL OR v.TenantCompanyId = @CompanyId)
END;
GO








IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucher_GetMaxVoucherCodeByTenantId') 
    DROP PROCEDURE [dbo].[spVoucher_GetMaxVoucherCodeByTenantId]
GO
CREATE PROCEDURE spVoucher_GetMaxVoucherCodeByTenantId
    @TenantId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        MAX(VoucherCode) AS MaxVoucherCode
    FROM 
        Voucher
    WHERE 
        TenantId = @TenantId;
END;
GO




IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucher_GetAllByParentCompanyAndTenantId') 
    DROP PROCEDURE [dbo].[spVoucher_GetAllByParentCompanyAndTenantId]
GO
CREATE PROCEDURE spVoucher_GetAllByParentCompanyAndTenantId
    @ParentCompanyId INT,
	@TenantId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        v.Id
        ,v.VoucherTypeId
		,v.VoucherType
        ,v.VoucherCode
        ,v.VoucherNarration
        ,v.VoucherInvoiceNo
        ,v.TenantId
        ,v.TenantCompanyId
        ,v.Active
        ,v.CreatedBy
        ,v.CreatedOn
        ,v.ModifiedBy
        ,v.ModifiedOn
		------------- Join Column ----------------
		,coalesce(vt.VoucherTypeName,'-') as VoucherTypeName
    FROM Voucher v
	left join VoucherType vt on vt.VoucherTypeName = v.VoucherType
    WHERE 
		v.TenantId = @TenantId And
		v.TenantCompanyId =  @ParentCompanyId
END;
GO






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucher_GetAllByTenantCompanyIdByVoucherType') 
    DROP PROCEDURE [dbo].[spVoucher_GetAllByTenantCompanyIdByVoucherType]
GO
CREATE PROCEDURE spVoucher_GetAllByTenantCompanyIdByVoucherType
    @ParentCompanyId INT,
	@TenantId INT,
	@VoucherType varchar(40) = null
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        v.Id
        ,v.VoucherTypeId
		,v.VoucherType
        ,v.VoucherCode
        ,v.VoucherNarration
        ,v.VoucherInvoiceNo
        ,v.TenantId
        ,v.TenantCompanyId
        ,v.Active
        ,v.CreatedBy
        ,v.CreatedOn
        ,v.ModifiedBy
        ,v.ModifiedOn
		------------- Join Column ----------------
		,coalesce(vt.VoucherTypeName,'-') as VoucherTypeName
    FROM Voucher v
	left join VoucherType vt on vt.VoucherTypeName = v.VoucherType
    WHERE 
		v.TenantId = @TenantId And
		v.TenantCompanyId =  @ParentCompanyId And
		v.VoucherType = @VoucherType
END;
GO

