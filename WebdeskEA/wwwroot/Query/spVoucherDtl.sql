
-- Create VoucherDtl table
--CREATE TABLE VoucherDtl (
--    Id INT IDENTITY(1,1) PRIMARY KEY,
--    VoucherId INT NOT NULL FOREIGN KEY REFERENCES Vouchers(Id),
--    COAId INT NOT NULL, -- Chart of Accounts ID
--    DbAmount DECIMAL(18,2) NOT NULL DEFAULT 0.0, -- Debit Amount
--    CrAmount DECIMAL(18,2) NOT NULL DEFAULT 0.0, -- Credit Amount
--    AccountNo NVARCHAR(50),
--    BankName NVARCHAR(100),     //
--    ChequeNo NVARCHAR(50),
--    PaymentType NVARCHAR(50),
--    TenantId INT NOT NULL,
--    TenantCompanyId INT NOT NULL
--);






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherDtl_Insert') 
    DROP PROCEDURE [dbo].[spVoucherDtl_Insert]
GO
CREATE PROCEDURE [dbo].[spVoucherDtl_Insert]
    @VoucherId INT,
    @COAId INT,
    @DbAmount DECIMAL(18,2),
    @CrAmount DECIMAL(18,2),
    @AccountNo NVARCHAR(50) = NULL,
    @BankName NVARCHAR(100) = NULL,
    @ChequeNo NVARCHAR(50) = NULL,
    @PaymentType NVARCHAR(50) = NULL,
	@PaidInvoiceNo NVARCHAR(50) = NULL,
    @PaidInvoiceId INT = 0,
	@PaidInvoiceType NVARCHAR(50) = NULL,
	@Remarks NVARCHAR(100) = NULL,
    @TenantId INT,
    @TenantCompanyId INT,
    @Id INT OUTPUT
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO VoucherDtl
        (
            VoucherId,
            COAId,
            DbAmount,
            CrAmount,
            AccountNo,
            BankName,
            ChequeNo,
            PaymentType,
			PaidInvoiceNo,
			PaidInvoiceId,
			PaidInvoiceType,
			Remarks,
            TenantId,
            TenantCompanyId
        )
        VALUES
        (
            @VoucherId,
            @COAId,
            @DbAmount,
            @CrAmount,
            @AccountNo,
            @BankName,
            @ChequeNo,
            @PaymentType,
			@PaidInvoiceNo,
			@PaidInvoiceId,
			@PaidInvoiceType,
			@Remarks,
            @TenantId,
            @TenantCompanyId
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






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherDtl_Update') 
    DROP PROCEDURE [dbo].[spVoucherDtl_Update]
GO
CREATE PROCEDURE [dbo].[spVoucherDtl_Update]
    @Id INT,
    @VoucherId INT,
    @COAId INT,
    @DbAmount DECIMAL(18,2),
    @CrAmount DECIMAL(18,2),
    @AccountNo NVARCHAR(50) = NULL,
    @BankName NVARCHAR(100) = NULL,
    @ChequeNo NVARCHAR(50) = NULL,
    @PaymentType NVARCHAR(50) = NULL,
	@PaidInvoiceNo NVARCHAR(50) = NULL,
    @PaidInvoiceId INT = 0,
	@PaidInvoiceType NVARCHAR(50) = NULL,
	@Remarks NVARCHAR(100) = NULL,
    @TenantId INT,
    @TenantCompanyId INT
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE VoucherDtl
        SET 
            VoucherId = @VoucherId,
            COAId = @COAId,
            DbAmount = @DbAmount,
            CrAmount = @CrAmount,
            AccountNo = @AccountNo,
            BankName = @BankName,
            ChequeNo = @ChequeNo,
            PaymentType = @PaymentType,
			PaidInvoiceNo = @PaidInvoiceNo,
			PaidInvoiceId = @PaidInvoiceId,
			PaidInvoiceType = @PaidInvoiceType,
			Remarks = @Remarks,
            TenantId = @TenantId,
            TenantCompanyId = @TenantCompanyId
        WHERE Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO









IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherDtl_Delete') 
    DROP PROCEDURE [dbo].[spVoucherDtl_Delete]
GO
CREATE PROCEDURE [dbo].[spVoucherDtl_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM VoucherDtl
    WHERE Id = @Id;
END;
GO







IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherDtl_GetAll') 
    DROP PROCEDURE [dbo].[spVoucherDtl_GetAll]
GO
CREATE PROCEDURE [dbo].[spVoucherDtl_GetAll]
AS
BEGIN
    SELECT 
        Id,
        VoucherId,
        COAId,
        DbAmount,
        CrAmount,
        AccountNo,
        BankName,
        ChequeNo,
        PaymentType,
		PaidInvoiceNo,
		PaidInvoiceId,
		PaidInvoiceType,
		Remarks,
        TenantId,
        TenantCompanyId
    FROM VoucherDtl;
END;
GO







IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherDtl_GetById') 
    DROP PROCEDURE [dbo].[spVoucherDtl_GetById]
GO
CREATE PROCEDURE [dbo].[spVoucherDtl_GetById]
    @Id INT
AS
BEGIN
    SELECT 
        Id,
        VoucherId,
        COAId,
        DbAmount,
        CrAmount,
        AccountNo,
        BankName,
        ChequeNo,
        PaymentType,
		PaidInvoiceNo,
		PaidInvoiceId,
		PaidInvoiceType,
		Remarks,
        TenantId,
        TenantCompanyId
    FROM VoucherDtl
    WHERE Id = @Id;
END;
GO





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherDtl_GetByVoucherIdCompanyAndTenantId') 
    DROP PROCEDURE [dbo].[spVoucherDtl_GetByVoucherIdCompanyAndTenantId]
GO
CREATE PROCEDURE [dbo].[spVoucherDtl_GetByVoucherIdCompanyAndTenantId]
    @VoucherId INT,
	@CompanyId int,
	@TenantId int
AS
BEGIN
    SELECT 
        vd.Id,
        vd.VoucherId,
        vd.COAId,
        vd.DbAmount,
        vd.CrAmount,
        vd.AccountNo,
        vd.BankName,
        vd.ChequeNo,
        vd.PaymentType,
		PaidInvoiceNo,
		PaidInvoiceId,
		PaidInvoiceType,
		Remarks,
        vd.TenantId,
        vd.TenantCompanyId,
		---- joined column ---
		coas.AccountName as AccountName
    FROM VoucherDtl vd
	left join coas on coas.id = vd.COAId
    WHERE VoucherId = @VoucherId and
	vd.TenantId = @TenantId and
	vd.TenantCompanyId = @CompanyId
END;
GO






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherDtl_GetByVoucherId') 
    DROP PROCEDURE [dbo].[spVoucherDtl_GetByVoucherId]
GO
CREATE PROCEDURE [dbo].[spVoucherDtl_GetByVoucherId]
    @VoucherId INT
AS
BEGIN
    SELECT 
        Id,
        VoucherId,
        COAId,
        DbAmount,
        CrAmount,
        AccountNo,
        BankName,
        ChequeNo,
        PaymentType,
		PaidInvoiceNo,
		PaidInvoiceId,
		PaidInvoiceType,
		Remarks,
        TenantId,
        TenantCompanyId
    FROM VoucherDtl
    WHERE VoucherId = @VoucherId;
END;
GO









IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherDtl_GetByTenantId') 
    DROP PROCEDURE [dbo].[spVoucherDtl_GetByTenantId]
GO
CREATE PROCEDURE [dbo].[spVoucherDtl_GetByTenantId]
    @TenantId INT
AS
BEGIN
    SELECT 
        Id,
        VoucherId,
        COAId,
        DbAmount,
        CrAmount,
        AccountNo,
        BankName,
        ChequeNo,
        PaymentType,
		PaidInvoiceNo,
		PaidInvoiceId,
		PaidInvoiceType,
		Remarks,
        TenantId,
        TenantCompanyId
    FROM VoucherDtl
    WHERE TenantId = @TenantId;
END;
GO





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherDtl_GetTotalByVoucherId') 
    DROP PROCEDURE [dbo].[spVoucherDtl_GetTotalByVoucherId]
GO
CREATE PROCEDURE [dbo].[spVoucherDtl_GetTotalByVoucherId]
    @VoucherId INT
AS
BEGIN
    SELECT 
        SUM(DbAmount) AS TotalDebit,
        SUM(CrAmount) AS TotalCredit
    FROM VoucherDtl
    WHERE VoucherId = @VoucherId;
END;
GO





