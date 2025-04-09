CREATE TABLE PIDtlTax (
    Id INT IDENTITY(1,1) PRIMARY KEY, -- Primary key with auto-increment
    PIId INT NOT NULL, -- Foreign key to Sales Order
    PIDtlId INT NOT NULL, -- Foreign key to Sales Order Detail
    TaxId INT NOT NULL, -- Foreign key to Tax table
    TaxAmount DECIMAL(18, 2) NOT NULL, -- Tax amount with two decimal precision
    AfterTaxAmount DECIMAL(18, 2) NOT NULL -- Amount after tax with two decimal precision
);






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtlTax_Insert')
    DROP PROCEDURE [dbo].[spPIDtlTax_Insert];
GO
CREATE PROCEDURE spPIDtlTax_Insert
    @PIId INT,
    @PIDtlId INT,
    @TaxId INT,
    @TaxAmount DECIMAL(18, 2),
    @AfterTaxAmount DECIMAL(18, 2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO PIDtlTax (PIId, PIDtlId, TaxId, TaxAmount, AfterTaxAmount)
    VALUES (@PIId, @PIDtlId, @TaxId, @TaxAmount, @AfterTaxAmount);

    SET @Id = SCOPE_IDENTITY();
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtlTax_Update')
    DROP PROCEDURE [dbo].[spPIDtlTax_Update];
GO
CREATE PROCEDURE spPIDtlTax_Update
    @Id INT,
    @PIId INT,
    @PIDtlId INT,
    @TaxId INT,
    @TaxAmount DECIMAL(18, 2),
    @AfterTaxAmount DECIMAL(18, 2)
AS
BEGIN
    UPDATE PIDtlTax
    SET
        PIId = @PIId,
        PIDtlId = @PIDtlId,
        TaxId = @TaxId,
        TaxAmount = @TaxAmount,
        AfterTaxAmount = @AfterTaxAmount
    WHERE Id = @Id;
END;












GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtlTax_Delete')
    DROP PROCEDURE [dbo].[spPIDtlTax_Delete];
GO
CREATE PROCEDURE spPIDtlTax_Delete
    @Id INT
AS
BEGIN
    DELETE FROM PIDtlTax
    WHERE Id = @Id;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtlTax_GetAll')
    DROP PROCEDURE [dbo].[spPIDtlTax_GetAll];
GO
CREATE PROCEDURE spPIDtlTax_GetAll
AS
BEGIN
    SELECT 
        st.Id,
        st.PIId,
        st.PIDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName,
        sod.SODtlDescription
    FROM PIDtlTax st
    LEFT JOIN Taxes t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.PIDtlId;
END;










GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtlTax_GetById')
    DROP PROCEDURE [dbo].[spPIDtlTax_GetById];
GO
CREATE PROCEDURE spPIDtlTax_GetById
    @Id INT
AS
BEGIN
    SELECT 
        st.Id,
        st.PIId,
        st.PIDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName,
        sod.SODtlDescription
    FROM PIDtlTax st
    LEFT JOIN Taxes t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.PIDtlId
    WHERE st.Id = @Id;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtlTax_GetAllByPIId')
    DROP PROCEDURE [dbo].[spPIDtlTax_GetAllByPIId];
GO
CREATE PROCEDURE spPIDtlTax_GetAllByPIId
    @PIId INT
AS
BEGIN
    SELECT 
        st.Id,
        st.PIId,
        st.PIDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName,
        sod.SODtlDescription
    FROM PIDtlTax st
    LEFT JOIN Taxes t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.PIDtlId
    WHERE st.PIId = @PIId;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtlTax_GetAllByPIDtlId')
    DROP PROCEDURE [dbo].[spPIDtlTax_GetAllByPIDtlId];
GO
CREATE PROCEDURE spPIDtlTax_GetAllByPIDtlId
    @PIDtlId INT
AS
BEGIN
    SELECT 
        st.Id,
        st.PIId,
        st.PIDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName,
        sod.SODtlDescription
    FROM PIDtlTax st
    LEFT JOIN Taxes t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.PIDtlId
    WHERE st.PIDtlId = @PIDtlId;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtlTax_DeleteByPIId')
    DROP PROCEDURE [dbo].[spPIDtlTax_DeleteByPIId];
GO
CREATE PROCEDURE spPIDtlTax_DeleteByPIId
    @PIId  INT
AS
BEGIN
    DELETE FROM PIDtlTax
    WHERE PIId = @PIId;
END;


