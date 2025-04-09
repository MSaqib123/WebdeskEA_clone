--===========================================================================================
--==================================== VoucherCode Generate =================================
--===========================================================================================
IF OBJECT_ID(N'[dbo].[GetMaxVoucherCodeByParentIdTenantIdVoucherCode]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[GetMaxVoucherCodeByParentIdTenantIdVoucherCode];
GO

CREATE PROCEDURE [dbo].[GetMaxVoucherCodeByParentIdTenantIdVoucherCode]
(
    @TenantId   INT,
    @CompanyId  INT,
    @Prefix     VARCHAR(50),  -- e.g. 'CRV', 'BRV'
    @PadString  VARCHAR(20)   -- e.g. '0000' for 4-digit padding, '00000' for 5-digit
)
AS
BEGIN
    SET NOCOUNT ON;

    /*
        ============================================================================
         1) Table/column references:
              - [dbo].[YourVoucherTable] => your actual table, e.g. [dbo].[Voucher]
              - TenantId                 => your tenant ID column
              - TenantCompanyId          => your company ID column
              - VoucherType              => e.g. 'CRV', 'BRV'
              - VoucherCode              => e.g. 'CRV-0001'
         2) Adjust as necessary to fit your database schema
        ============================================================================
    */

    DECLARE @LastVoucherCode VARCHAR(50);

    -- A) Pull the maximum existing VoucherCode for the given Tenant, Company & Type
    SELECT
        @LastVoucherCode = MAX(VoucherCode)
    FROM
        [dbo].Voucher   -- <-- change to your real table name
    WHERE
        TenantId        = @TenantId
        AND TenantCompanyId = @CompanyId
        AND VoucherType  = @Prefix;  -- e.g. 'CRV'

    -- B) If there's no existing code, assume '0' so we can start from scratch
    IF @LastVoucherCode IS NULL
        SET @LastVoucherCode = '0';
    ELSE
    BEGIN
        -- C) If there's a dash (e.g. 'CRV-0001'), parse out the numeric portion
        DECLARE @DashPos INT = CHARINDEX('-', @LastVoucherCode);
        IF @DashPos > 0
        BEGIN
            SET @LastVoucherCode = SUBSTRING(
                                     @LastVoucherCode,
                                     @DashPos + 1,
                                     LEN(@LastVoucherCode) - @DashPos
                                  );
        END
    END

    -- D) Convert numeric portion to integer, increment by 1
    DECLARE @NumericPart INT = ISNULL(TRY_CAST(@LastVoucherCode AS INT), 0) + 1;

    /*
        E) Zero-pad the numeric part according to the length of @PadString.
           e.g. if @PadString = '0000', then pad length = 4
                if @PadString = '00000', then pad length = 5
    */
    DECLARE @PadLength   INT = LEN(@PadString);
    DECLARE @PaddedValue VARCHAR(50);
    SET @PaddedValue = RIGHT(
                          @PadString + CAST(@NumericPart AS VARCHAR(50)),
                          @PadLength
                       );

    -- F) Construct the final code: "CRV-0002", "BRV-0001", etc.
    DECLARE @NextVoucherCode VARCHAR(50) = @Prefix + '-' + @PaddedValue;

    -- G) Return the new code
    SELECT @NextVoucherCode AS MaxVoucherCode;
END
GO
