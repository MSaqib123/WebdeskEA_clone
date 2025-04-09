--CREATE TABLE PODtlTax (
--    Id INT IDENTITY(1,1) PRIMARY KEY, -- Primary key with auto-increment
--    POId INT NOT NULL, -- Foreign key to Sales Order
--    PODtlId INT NOT NULL, -- Foreign key to Sales Order Detail
--    TaxId INT NOT NULL, -- Foreign key to Tax table
--    TaxAmount DECIMAL(18, 2) NOT NULL, -- Tax amount with two decimal precision
--    AfterTaxAmount DECIMAL(18, 2) NOT NULL -- Amount after tax with two decimal precision
--);






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtlTax_Insert')
    DROP PROCEDURE [dbo].[spPODtlTax_Insert];
GO
CREATE PROCEDURE spPODtlTax_Insert
    @POId INT,
    @PODtlId INT,
    @TaxId INT,
    @TaxAmount DECIMAL(18, 2),
    @AfterTaxAmount DECIMAL(18, 2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO PODtlTax (POId, PODtlId, TaxId, TaxAmount, AfterTaxAmount)
    VALUES (@POId, @PODtlId, @TaxId, @TaxAmount, @AfterTaxAmount);

    SET @Id = SCOPE_IDENTITY();
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtlTax_Update')
    DROP PROCEDURE [dbo].[spPODtlTax_Update];
GO
CREATE PROCEDURE spPODtlTax_Update
    @Id INT,
    @POId INT,
    @PODtlId INT,
    @TaxId INT,
    @TaxAmount DECIMAL(18, 2),
    @AfterTaxAmount DECIMAL(18, 2)
AS
BEGIN
    UPDATE PODtlTax
    SET
        POId = @POId,
        PODtlId = @PODtlId,
        TaxId = @TaxId,
        TaxAmount = @TaxAmount,
        AfterTaxAmount = @AfterTaxAmount
    WHERE Id = @Id;
END;












GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtlTax_Delete')
    DROP PROCEDURE [dbo].[spPODtlTax_Delete];
GO
CREATE PROCEDURE spPODtlTax_Delete
    @Id INT
AS
BEGIN
    DELETE FROM PODtlTax
    WHERE Id = @Id;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtlTax_GetAll')
    DROP PROCEDURE [dbo].[spPODtlTax_GetAll];
GO
CREATE PROCEDURE spPODtlTax_GetAll
AS
BEGIN
    SELECT 
        st.Id,
        st.POId,
        st.PODtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM PODtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.PODtlId
END;










GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtlTax_GetById')
    DROP PROCEDURE [dbo].[spPODtlTax_GetById];
GO
CREATE PROCEDURE spPODtlTax_GetById
    @Id INT
AS
BEGIN
    SELECT 
        st.Id,
        st.POId,
        st.PODtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM PODtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.PODtlId
    WHERE st.Id = @Id;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtlTax_GetAllByPOId')
    DROP PROCEDURE [dbo].[spPODtlTax_GetAllByPOId];
GO
CREATE PROCEDURE spPODtlTax_GetAllByPOId
    @POId INT
AS
BEGIN
    SELECT 
        st.Id,
        st.POId,
        st.PODtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM PODtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.PODtlId
    WHERE st.POId = @POId;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtlTax_GetAllByPODtlId')
    DROP PROCEDURE [dbo].[spPODtlTax_GetAllByPODtlId];
GO
CREATE PROCEDURE spPODtlTax_GetAllByPODtlId
    @PODtlId INT
AS
BEGIN
    SELECT 
        st.Id,
        st.POId,
        st.PODtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM PODtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sod ON sod.Id = st.PODtlId
    WHERE st.PODtlId = @PODtlId;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtlTax_DeleteByPOId')
    DROP PROCEDURE [dbo].[spPODtlTax_DeleteByPOId];
GO
CREATE PROCEDURE spPODtlTax_DeleteByPOId
    @POId  INT
AS
BEGIN
    DELETE FROM PODtlTax
    WHERE POId = @POId;
END;


