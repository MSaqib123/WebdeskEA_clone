---- Create Table for SODtl
--GO
--IF EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'SODtls')
--    DROP TABLE [dbo].[SODtls];
--GO
--CREATE TABLE [dbo].[SODtls](
--    [Id] INT IDENTITY(1,1) PRIMARY KEY,
--    [SOId] INT NOT NULL,
--    [ProductId] INT NOT NULL,
--    [SODtlQty] INT NOT NULL,
--    [SODtlPrice] DECIMAL(18,2) NOT NULL,
--    [SODtlTotal] DECIMAL(18,2) NOT NULL
--);
--GO


-- Insert Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtl_Insert')
    DROP PROCEDURE [dbo].[spSODtl_Insert];
GO
CREATE PROCEDURE spSODtl_Insert
    @SOId INT,
    @ProductId INT,
    @SODtlQty INT,
    @SODtlPrice DECIMAL(18,2),
    @SODtlTotal DECIMAL(18,2),
	@SODtlTotalAfterVAT DECIMAL(18,2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO SODtls (SOId, ProductId, SODtlQty, SODtlPrice, SODtlTotal, SODtlTotalAfterVAT)
    VALUES (@SOId, @ProductId, @SODtlQty, @SODtlPrice, @SODtlTotal,@SODtlTotalAfterVAT);

    SET @Id = SCOPE_IDENTITY();
END;







-- Update Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtl_Update')
    DROP PROCEDURE [dbo].[spSODtl_Update];
GO
CREATE PROCEDURE spSODtl_Update
    @Id INT,
    @SOId INT,
    @ProductId INT,
    @SODtlQty INT,
    @SODtlPrice DECIMAL(18,2),
    @SODtlTotal DECIMAL(18,2),
	@SODtlTotalAfterVAT DECIMAL(18,2)
AS
BEGIN
    UPDATE SODtls
    SET
        SOId = @SOId,
        ProductId = @ProductId,
        SODtlQty = @SODtlQty,
        SODtlPrice = @SODtlPrice,
        SODtlTotal = @SODtlTotal,
		SODtlTotalAfterVAT = @SODtlTotalAfterVAT
    WHERE Id = @Id;
END;

-- Delete Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtl_Delete')
    DROP PROCEDURE [dbo].[spSODtl_Delete];
GO
CREATE PROCEDURE spSODtl_Delete
    @Id INT
AS
BEGIN
    DELETE FROM SODtls
    WHERE Id = @Id;
END;

-- Get All Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtl_GetAll')
    DROP PROCEDURE [dbo].[spSODtl_GetAll];
GO
CREATE PROCEDURE spSODtl_GetAll
AS
BEGIN
    SELECT *
    FROM SODtls;
END;



-- Get By Id Procedure


GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtl_GetAllBySOId')
    DROP PROCEDURE [dbo].[spSODtl_GetAllBySOId];
GO
CREATE PROCEDURE spSODtl_GetAllBySOId
    @Id INT
AS
BEGIN
    SELECT *
    FROM SODtls
    WHERE SOId = @Id;
END;





-- Get By Id Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtl_GetById')
    DROP PROCEDURE [dbo].[spSODtl_GetById];
GO
CREATE PROCEDURE spSODtl_GetById
    @Id INT
AS
BEGIN
    SELECT *
    FROM SODtls
    WHERE Id = @Id;
END;




GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtl_GetAllBySOId')
    DROP PROCEDURE [dbo].[spSODtl_GetAllBySOId];
GO
CREATE PROCEDURE spSODtl_GetAllBySOId
    @Id INT
AS
BEGIN
    SELECT 
	*
    FROM SODtls
    WHERE SOId = @Id;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSODtl_DeleteBySOId')
    DROP PROCEDURE [dbo].[spSODtl_DeleteBySOId];
GO
CREATE PROCEDURE spSODtl_DeleteBySOId
    @SOId INT
AS
BEGIN
    delete from SODtls
    WHERE SOId = @SOId;
END;

