--CREATE TABLE SIDtlTax (
--    Id INT IDENTITY(1,1) PRIMARY KEY, -- Primary key with auto-increment
--    SIId INT NOT NULL, -- Foreign key to Sales Order
--    SIDtlId INT NOT NULL, -- Foreign key to Sales Order Detail
--    TaxId INT NOT NULL, -- Foreign key to Tax table
--    TaxAmount DECIMAL(18, 2) NOT NULL, -- Tax amount with two decimal precision
--    AfterTaxAmount DECIMAL(18, 2) NOT NULL -- Amount after tax with two decimal precision
--);






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIDtlTax_Insert')
    DROP PROCEDURE [dbo].[spSIDtlTax_Insert];
GO
CREATE PROCEDURE spSIDtlTax_Insert
    @SIId INT,
    @SIDtlId INT,
    @TaxId INT,
    @TaxAmount DECIMAL(18, 2),
    @AfterTaxAmount DECIMAL(18, 2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO SIDtlTax (SIId, SIDtlId, TaxId, TaxAmount, AfterTaxAmount)
    VALUES (@SIId, @SIDtlId, @TaxId, @TaxAmount, @AfterTaxAmount);

    SET @Id = SCOPE_IDENTITY();
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIDtlTax_Update')
    DROP PROCEDURE [dbo].[spSIDtlTax_Update];
GO
CREATE PROCEDURE spSIDtlTax_Update
    @Id INT,
    @SIId INT,
    @SIDtlId INT,
    @TaxId INT,
    @TaxAmount DECIMAL(18, 2),
    @AfterTaxAmount DECIMAL(18, 2)
AS
BEGIN
    UPDATE SIDtlTax
    SET
        SIId = @SIId,
        SIDtlId = @SIDtlId,
        TaxId = @TaxId,
        TaxAmount = @TaxAmount,
        AfterTaxAmount = @AfterTaxAmount
    WHERE Id = @Id;
END;












GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIDtlTax_Delete')
    DROP PROCEDURE [dbo].[spSIDtlTax_Delete];
GO
CREATE PROCEDURE spSIDtlTax_Delete
    @Id INT
AS
BEGIN
    DELETE FROM SIDtlTax
    WHERE Id = @Id;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIDtlTax_GetAll')
    DROP PROCEDURE [dbo].[spSIDtlTax_GetAll];
GO
CREATE PROCEDURE spSIDtlTax_GetAll
AS
BEGIN
    SELECT 
        st.Id,
        st.SIId,
        st.SIDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM SIDtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SIDtls sid ON sid.Id = st.SIDtlId;
END;










GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIDtlTax_GetById')
    DROP PROCEDURE [dbo].[spSIDtlTax_GetById];
GO
CREATE PROCEDURE spSIDtlTax_GetById
    @Id INT
AS
BEGIN
    SELECT 
        st.Id,
        st.SIId,
        st.SIDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM SIDtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SIDtls sid ON sid.Id = st.SIDtlId
    WHERE st.Id = @Id;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIDtlTax_GetAllBySIId')
    DROP PROCEDURE [dbo].[spSIDtlTax_GetAllBySIId];
GO
CREATE PROCEDURE spSIDtlTax_GetAllBySIId
    @SIId INT
AS
BEGIN
    SELECT 
        st.Id,
        st.SIId,
        st.SIDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM SIDtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SiDtls sid ON sid.Id = st.SIDtlId
    WHERE st.SIId = @SIId;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIDtlTax_GetAllBySODtlId')
    DROP PROCEDURE [dbo].[spSIDtlTax_GetAllBySODtlId];
GO
CREATE PROCEDURE spSIDtlTax_GetAllBySODtlId
    @SIDtlId INT
AS
BEGIN
    SELECT 
        st.Id,
        st.SIId,
        st.SIDtlId,
        st.TaxId,
        st.TaxAmount,
        st.AfterTaxAmount,
        -- Example join columns
        t.TaxName
    FROM SIDtlTax st
    LEFT JOIN TaxMasters t ON t.Id = st.TaxId
    LEFT JOIN SODtls sid ON sid.Id = st.SIDtlId
    WHERE st.SIDtlId = @SIDtlId;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIDtlTax_DeleteBySIId')
    DROP PROCEDURE [dbo].[spSIDtlTax_DeleteBySIId];
GO
CREATE PROCEDURE spSIDtlTax_DeleteBySIId
    @SIId  INT
AS
BEGIN
    DELETE FROM SIDtlTax
    WHERE SIId = @SIId;
END;


