CREATE TABLE SRDtlTax (
    Id INT IDENTITY(1,1) PRIMARY KEY, -- Primary key with auto-increment
    SRId INT NOT NULL, -- Foreign key to Sales Order
    SRDtlId INT NOT NULL, -- Foreign key to Sales Order Detail
    TaxId INT NOT NULL, -- Foreign key to Tax table
    TaxAmount DECIMAL(18, 2) NOT NULL, -- Tax amount with two decimal precision
    AfterTaxAmount DECIMAL(18, 2) NOT NULL -- Amount after tax with two decimal precision
);






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtlTax_Insert')
    DROP PROCEDURE [dbo].[spSRDtlTax_Insert];
GO
CREATE PROCEDURE spSRDtlTax_Insert
    @SRId INT,
    @SRDtlId INT,
    @TaxId INT,
    @TaxAmount DECIMAL(18, 2),
    @AfterTaxAmount DECIMAL(18, 2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO SRDtlTax (SRId, SRDtlId, TaxId, TaxAmount, AfterTaxAmount)
    VALUES (@SRId, @SRDtlId, @TaxId, @TaxAmount, @AfterTaxAmount);

    SET @Id = SCOPE_IDENTITY();
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtlTax_Update')
    DROP PROCEDURE [dbo].[spSRDtlTax_Update];
GO
CREATE PROCEDURE spSRDtlTax_Update
    @Id INT,
    @SRId INT,
    @SRDtlId INT,
    @TaxId INT,
    @TaxAmount DECIMAL(18, 2),
    @AfterTaxAmount DECIMAL(18, 2)
AS
BEGIN
    UPDATE SRDtlTax
    SET
        SRId = @SRId,
        SRDtlId = @SRDtlId,
        TaxId = @TaxId,
        TaxAmount = @TaxAmount,
        AfterTaxAmount = @AfterTaxAmount
    WHERE Id = @Id;
END;












GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtlTax_Delete')
    DROP PROCEDURE [dbo].[spSRDtlTax_Delete];
GO
CREATE PROCEDURE spSRDtlTax_Delete
    @Id INT
AS
BEGIN
    DELETE FROM SRDtlTax
    WHERE Id = @Id;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtlTax_GetAll')
    DROP PROCEDURE [dbo].[spSRDtlTax_GetAll];
GO
CREATE PROCEDURE spSRDtlTax_GetAll
AS
BEGIN
    SELECT 
        st.Id,
        st.SRId,
        st.SRDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM SRDtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.SRDtlId
END;










GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtlTax_GetById')
    DROP PROCEDURE [dbo].[spSRDtlTax_GetById];
GO
CREATE PROCEDURE spSRDtlTax_GetById
    @Id INT
AS
BEGIN
    SELECT 
        st.Id,
        st.SRId,
        st.SRDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM SRDtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.SRDtlId
    WHERE st.Id = @Id;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtlTax_GetAllBySRId')
    DROP PROCEDURE [dbo].[spSRDtlTax_GetAllBySRId];
GO
CREATE PROCEDURE spSRDtlTax_GetAllBySRId
    @SRId INT
AS
BEGIN
    SELECT 
        st.Id,
        st.SRId,
        st.SRDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM SRDtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.SRDtlId
    WHERE st.SRId = @SRId;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtlTax_GetAllBySRDtlId')
    DROP PROCEDURE [dbo].[spSRDtlTax_GetAllBySRDtlId];
GO
CREATE PROCEDURE spSRDtlTax_GetAllBySRDtlId
    @SRDtlId INT
AS
BEGIN
    SELECT 
        st.Id,
        st.SRId,
        st.SRDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM SRDtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.SRDtlId
    WHERE st.SRDtlId = @SRDtlId;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtlTax_DeleteBySRId')
    DROP PROCEDURE [dbo].[spSRDtlTax_DeleteBySRId];
GO
CREATE PROCEDURE spSRDtlTax_DeleteBySRId
    @SRId  INT
AS
BEGIN
    DELETE FROM SRDtlTax
    WHERE SRId = @SRId;
END;


