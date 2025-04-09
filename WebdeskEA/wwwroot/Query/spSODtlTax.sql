--CREATE TABLE SODtlTax (
--    Id INT IDENTITY(1,1) PRIMARY KEY, -- Primary key with auto-increment
--    SOId INT NOT NULL, -- Foreign key to Sales Order
--    SODtlId INT NOT NULL, -- Foreign key to Sales Order Detail
--    TaxId INT NOT NULL, -- Foreign key to Tax table
--    TaxAmount DECIMAL(18, 2) NOT NULL, -- Tax amount with two decimal precision
--    AfterTaxAmount DECIMAL(18, 2) NOT NULL -- Amount after tax with two decimal precision
--);






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtlTax_Insert')
    DROP PROCEDURE [dbo].[spSODtlTax_Insert];
GO
CREATE PROCEDURE spSODtlTax_Insert
    @SOId INT,
    @SODtlId INT,
    @TaxId INT,
    @TaxAmount DECIMAL(18, 2),
    @AfterTaxAmount DECIMAL(18, 2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO SODtlTax (SOId, SODtlId, TaxId, TaxAmount, AfterTaxAmount)
    VALUES (@SOId, @SODtlId, @TaxId, @TaxAmount, @AfterTaxAmount);

    SET @Id = SCOPE_IDENTITY();
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtlTax_Update')
    DROP PROCEDURE [dbo].[spSODtlTax_Update];
GO
CREATE PROCEDURE spSODtlTax_Update
    @Id INT,
    @SOId INT,
    @SODtlId INT,
    @TaxId INT,
    @TaxAmount DECIMAL(18, 2),
    @AfterTaxAmount DECIMAL(18, 2)
AS
BEGIN
    UPDATE SODtlTax
    SET
        SOId = @SOId,
        SODtlId = @SODtlId,
        TaxId = @TaxId,
        TaxAmount = @TaxAmount,
        AfterTaxAmount = @AfterTaxAmount
    WHERE Id = @Id;
END;












GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtlTax_Delete')
    DROP PROCEDURE [dbo].[spSODtlTax_Delete];
GO
CREATE PROCEDURE spSODtlTax_Delete
    @Id INT
AS
BEGIN
    DELETE FROM SODtlTax
    WHERE Id = @Id;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtlTax_GetAll')
    DROP PROCEDURE [dbo].[spSODtlTax_GetAll];
GO
CREATE PROCEDURE spSODtlTax_GetAll
AS
BEGIN
    SELECT 
        st.Id,
        st.SOId,
        st.SODtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName,
        sod.SODtlDescription
    FROM SODtlTax st
    LEFT JOIN Taxes t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.SODtlId;
END;










GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtlTax_GetById')
    DROP PROCEDURE [dbo].[spSODtlTax_GetById];
GO
CREATE PROCEDURE spSODtlTax_GetById
    @Id INT
AS
BEGIN
    SELECT 
        st.Id,
        st.SOId,
        st.SODtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName,
        sod.SODtlDescription
    FROM SODtlTax st
    LEFT JOIN Taxes t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.SODtlId
    WHERE st.Id = @Id;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtlTax_GetAllBySOId')
    DROP PROCEDURE [dbo].[spSODtlTax_GetAllBySOId];
GO
CREATE PROCEDURE spSODtlTax_GetAllBySOId
    @SOId INT
AS
BEGIN
    SELECT 
        st.Id,
        st.SOId,
        st.SODtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName,
        sod.SODtlDescription
    FROM SODtlTax st
    LEFT JOIN Taxes t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.SODtlId
    WHERE st.SOId = @SOId;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtlTax_GetAllBySODtlId')
    DROP PROCEDURE [dbo].[spSODtlTax_GetAllBySODtlId];
GO
CREATE PROCEDURE spSODtlTax_GetAllBySODtlId
    @SODtlId INT
AS
BEGIN
    SELECT 
        st.Id,
        st.SOId,
        st.SODtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName,
        sod.SODtlDescription
    FROM SODtlTax st
    LEFT JOIN Taxes t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.SODtlId
    WHERE st.SODtlId = @SODtlId;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtlTax_DeleteBySOId')
    DROP PROCEDURE [dbo].[spSODtlTax_DeleteBySOId];
GO
CREATE PROCEDURE spSODtlTax_DeleteBySOId
    @SOId  INT
AS
BEGIN
    DELETE FROM SODtlTax
    WHERE SOId = @SOId;
END;


