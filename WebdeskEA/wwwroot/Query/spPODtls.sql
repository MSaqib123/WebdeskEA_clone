---- Create Table for PODtl
--GO
--IF EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'PODtls')
--    DROP TABLE [dbo].[PODtls];
--GO
--CREATE TABLE [dbo].[PODtls](
--    [Id] INT IDENTITY(1,1) PRIMARY KEY,
--    [POId] INT NOT NULL,
--    [ProductId] INT NOT NULL,
--    [PODtlQty] INT NOT NULL,
--    [PODtlPrice] DECIMAL(18,2) NOT NULL,
--    [PODtlTotal] DECIMAL(18,2) NOT NULL
--);
--GO


-- Insert Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtl_Insert')
    DROP PROCEDURE [dbo].[spPODtl_Insert];
GO
CREATE PROCEDURE spPODtl_Insert
    @POId INT,
    @ProductId INT,
    @PODtlQty INT,
    @PODtlPrice DECIMAL(18,2),
    @PODtlTotal DECIMAL(18,2),
	@PODtlTotalAfterVAT DECIMAL(18,2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO PODtls (POId, ProductId, PODtlQty, PODtlPrice, PODtlTotal,PODtlTotalAfterVAT)
    VALUES (@POId, @ProductId, @PODtlQty, @PODtlPrice, @PODtlTotal,@PODtlTotalAfterVAT);

    SET @Id = SCOPE_IDENTITY();
END;

-- Update Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtl_Update')
    DROP PROCEDURE [dbo].[spPODtl_Update];
GO
CREATE PROCEDURE spPODtl_Update
    @Id INT,
    @POId INT,
    @ProductId INT,
    @PODtlQty INT,
    @PODtlPrice DECIMAL(18,2),
    @PODtlTotal DECIMAL(18,2),
	@PODtlTotalAfterVAT DECIMAL(18,2)
AS
BEGIN
    UPDATE PODtls
    SET
        POId = @POId,
        ProductId = @ProductId,
        PODtlQty = @PODtlQty,
        PODtlPrice = @PODtlPrice,
        PODtlTotal = @PODtlTotal,
		PODtlTotalAfterVAT = @PODtlTotalAfterVAT
    WHERE Id = @Id;
END;

-- Delete Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtl_Delete')
    DROP PROCEDURE [dbo].[spPODtl_Delete];
GO
CREATE PROCEDURE spPODtl_Delete
    @Id INT
AS
BEGIN
    DELETE FROM PODtls
    WHERE Id = @Id;
END;

-- Get All Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtl_GetAll')
    DROP PROCEDURE [dbo].[spPODtl_GetAll];
GO
CREATE PROCEDURE spPODtl_GetAll
AS
BEGIN
    SELECT *
    FROM PODtls;
END;



-- Get By Id Procedure


GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtl_GetAllByPOId')
    DROP PROCEDURE [dbo].[spPODtl_GetAllByPOId];
GO
CREATE PROCEDURE spPODtl_GetAllByPOId
    @Id INT
AS
BEGIN
    SELECT *
    FROM PODtls
    WHERE POId = @Id;
END;





-- Get By Id Procedure
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtl_GetById')
    DROP PROCEDURE [dbo].[spPODtl_GetById];
GO
CREATE PROCEDURE spPODtl_GetById
    @Id INT
AS
BEGIN
    SELECT *
    FROM PODtls
    WHERE Id = @Id;
END;






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPODtl_DeleteByPOId')
    DROP PROCEDURE [dbo].[spPODtl_DeleteByPOId];
GO
CREATE PROCEDURE spPODtl_DeleteByPOId  
    @POId INT  
AS  
BEGIN  
    delete
    FROM PODtls  
    WHERE POId = @POId;  
END;  
  