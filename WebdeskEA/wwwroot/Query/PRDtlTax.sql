CREATE TABLE PRDtlTax (
    Id INT IDENTITY(1,1) PRIMARY KEY, -- Primary key with auto-increment
    PRId INT NOT NULL, -- Foreign key to Sales Order
    PRDtlId INT NOT NULL, -- Foreign key to Sales Order Detail
    TaxId INT NOT NULL, -- Foreign key to Tax table
    TaxAmount DECIMAL(18, 2) NOT NULL, -- Tax amount with two decimal precision
    AfterTaxAmount DECIMAL(18, 2) NOT NULL -- Amount after tax with two decimal precision
);






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtlTax_Insert')
    DROP PROCEDURE [dbo].[spPRDtlTax_Insert];
GO
CREATE PROCEDURE spPRDtlTax_Insert
    @PRId INT,
    @PRDtlId INT,
    @TaxId INT,
    @TaxAmount DECIMAL(18, 2),
    @AfterTaxAmount DECIMAL(18, 2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO PRDtlTax (PRId, PRDtlId, TaxId, TaxAmount, AfterTaxAmount)
    VALUES (@PRId, @PRDtlId, @TaxId, @TaxAmount, @AfterTaxAmount);

    SET @Id = SCOPE_IDENTITY();
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtlTax_Update')
    DROP PROCEDURE [dbo].[spPRDtlTax_Update];
GO
CREATE PROCEDURE spPRDtlTax_Update
    @Id INT,
    @PRId INT,
    @PRDtlId INT,
    @TaxId INT,
    @TaxAmount DECIMAL(18, 2),
    @AfterTaxAmount DECIMAL(18, 2)
AS
BEGIN
    UPDATE PRDtlTax
    SET
        PRId = @PRId,
        PRDtlId = @PRDtlId,
        TaxId = @TaxId,
        TaxAmount = @TaxAmount,
        AfterTaxAmount = @AfterTaxAmount
    WHERE Id = @Id;
END;












GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtlTax_Delete')
    DROP PROCEDURE [dbo].[spPRDtlTax_Delete];
GO
CREATE PROCEDURE spPRDtlTax_Delete
    @Id INT
AS
BEGIN
    DELETE FROM PRDtlTax
    WHERE Id = @Id;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtlTax_GetAll')
    DROP PROCEDURE [dbo].[spPRDtlTax_GetAll];
GO
CREATE PROCEDURE spPRDtlTax_GetAll
AS
BEGIN
    SELECT 
        st.Id,
        st.PRId,
        st.PRDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM PRDtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.PRDtlId
END;










GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtlTax_GetById')
    DROP PROCEDURE [dbo].[spPRDtlTax_GetById];
GO
CREATE PROCEDURE spPRDtlTax_GetById
    @Id INT
AS
BEGIN
    SELECT 
        st.Id,
        st.PRId,
        st.PRDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM PRDtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.PRDtlId
    WHERE st.Id = @Id;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtlTax_GetAllByPRId')
    DROP PROCEDURE [dbo].[spPRDtlTax_GetAllByPRId];
GO
CREATE PROCEDURE spPRDtlTax_GetAllByPRId
    @PRId INT
AS
BEGIN
    SELECT 
        st.Id,
        st.PRId,
        st.PRDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM PRDtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.PRDtlId
    WHERE st.PRId = @PRId;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtlTax_GetAllByPRDtlId')
    DROP PROCEDURE [dbo].[spPRDtlTax_GetAllByPRDtlId];
GO
CREATE PROCEDURE spPRDtlTax_GetAllByPRDtlId
    @PRDtlId INT
AS
BEGIN
    SELECT 
        st.Id,
        st.PRId,
        st.PRDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM PRDtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.PRDtlId
    WHERE st.PRDtlId = @PRDtlId;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtlTax_DeleteByPRId')
    DROP PROCEDURE [dbo].[spPRDtlTax_DeleteByPRId];
GO
CREATE PROCEDURE spPRDtlTax_DeleteByPRId
    @PRId  INT
AS
BEGIN
    DELETE FROM PRDtlTax
    WHERE PRId = @PRId;
END;


