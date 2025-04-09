--CREATE TABLE [dbo].[SRDtls]
--(
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[SRId] [int] NOT NULL,
--	[ProductId] [int] NOT NULL,
--	[SRDtlQty] [int] NOT NULL,
--	[SRDtlPrice] [decimal](18, 2) NOT NULL,
--	[SRDtlTotal] [decimal](18, 2) NOT NULL,
--) 







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtl_Insert')
    DROP PROCEDURE [dbo].[spSRDtl_Insert];
GO
CREATE PROCEDURE spSRDtl_Insert
    @SRId INT,
    @ProductId INT,
    @SRDtlQty INT,
    @SRDtlPrice DECIMAL(18, 2),
    @SRDtlTotal DECIMAL(18, 2),
	@SRDtlTotalAfterVAT DECIMAL(18, 2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO SRDtls (SRId, ProductId, SRDtlQty, SRDtlPrice, SRDtlTotal,SRDtlTotalAfterVAT)
    VALUES (@SRId, @ProductId, @SRDtlQty, @SRDtlPrice, @SRDtlTotal,@SRDtlTotalAfterVAT);

    SET @Id = SCOPE_IDENTITY();
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtl_Update')
    DROP PROCEDURE [dbo].[spSRDtl_Update];
GO
CREATE PROCEDURE spSRDtl_Update
    @Id INT,
    @SRId INT,
    @ProductId INT,
    @SRDtlQty INT,
    @SRDtlPrice DECIMAL(18, 2),
    @SRDtlTotal DECIMAL(18, 2),
	@SRDtlTotalAfterVAT DECIMAL(18, 2)
AS
BEGIN
    UPDATE SRDtls
    SET
        SRId = @SRId,
        ProductId = @ProductId,
        SRDtlQty = @SRDtlQty,
        SRDtlPrice = @SRDtlPrice,
        SRDtlTotal = @SRDtlTotal,
		SRDtlTotalAfterVAT = @SRDtlTotalAfterVAT
    WHERE Id = @Id;
END;





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtl_Delete')
    DROP PROCEDURE [dbo].[spSRDtl_Delete];
GO
CREATE PROCEDURE spSRDtl_Delete
    @Id INT
AS
BEGIN
    DELETE FROM SRDtls
    WHERE Id = @Id;
END;

GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtl_DeleteBySRId')
    DROP PROCEDURE [dbo].[spSRDtl_DeleteBySRId];
GO
CREATE PROCEDURE spSRDtl_DeleteBySRId
    @SRId INT
AS
BEGIN
    DELETE FROM SRDtls
    WHERE SRId = @SRId;
END;




GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtl_GetAll')
    DROP PROCEDURE [dbo].[spSRDtl_GetAll];
GO
CREATE PROCEDURE spSRDtl_GetAll
AS
BEGIN
    SELECT 
        SRd.Id,
        SRd.SRId,
        SRd.ProductId,
        SRd.SRDtlQty,
        SRd.SRDtlPrice,
        SRd.SRDtlTotal,
		SRd.SRDtlTotalAfterVAT,
		--___ JOin colum ___
		p.ProductName
    FROM SRDtls SRd
    LEFT JOIN Products p ON p.Id = SRd.ProductId;
END;






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtl_GetById')
    DROP PROCEDURE [dbo].[spSRDtl_GetById];
GO
CREATE PROCEDURE spSRDtl_GetById
    @Id INT
AS
BEGIN
    SELECT 
        SRd.Id,
        SRd.SRId,
        SRd.ProductId,
        SRd.SRDtlQty,
        SRd.SRDtlPrice,
        SRd.SRDtlTotal,
		SRd.SRDtlTotalAfterVAT,
		--___ JOin colum ___
		p.ProductName
    FROM SRDtls SRd
    LEFT JOIN Products p ON p.Id = SRd.ProductId
    WHERE SRd.Id = @Id;
END;






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRDtl_GetAllBySRId')
    DROP PROCEDURE [dbo].[spSRDtl_GetAllBySRId];
GO
CREATE PROCEDURE spSRDtl_GetAllBySRId
    @SRId INT
AS
BEGIN
    SELECT 
        SRd.Id,
        SRd.SRId,
        SRd.ProductId,
        SRd.SRDtlQty,
        SRd.SRDtlPrice,
        SRd.SRDtlTotal,
		SRd.SRDtlTotalAfterVAT,
		--___ JOin colum ___
		p.ProductName
    FROM SRDtls SRd
    LEFT JOIN Products p ON p.Id = SRd.ProductId
    WHERE SRd.SRId = @SRId;
END;
