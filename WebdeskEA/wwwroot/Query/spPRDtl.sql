--CREATE TABLE [dbo].[PRDtls](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[PRId] [int] NOT NULL,
--	[ProductId] [int] NOT NULL,
--	[PRDtlQty] [int] NOT NULL,
--	[PRDtlPrice] [decimal](18, 2) NOT NULL,
--	[PRDtlTotal] [decimal](18, 2) NOT NULL,
--)







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtl_Insert')
    DROP PROCEDURE [dbo].[spPRDtl_Insert];
GO
CREATE PROCEDURE spPRDtl_Insert
    @PRId INT,
    @ProductId INT,
    @PRDtlQty INT,
    @PRDtlPrice DECIMAL(18, 2),
    @PRDtlTotal DECIMAL(18, 2),
	@PRDtlTotalAfterVAT DECIMAL(18, 2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO PRDtls (PRId, ProductId, PRDtlQty, PRDtlPrice, PRDtlTotal,PRDtlTotalAfterVAT)
    VALUES (@PRId, @ProductId, @PRDtlQty, @PRDtlPrice, @PRDtlTotal,@PRDtlTotalAfterVAT);

    SET @Id = SCOPE_IDENTITY();
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtl_Update')
    DROP PROCEDURE [dbo].[spPRDtl_Update];
GO
CREATE PROCEDURE spPRDtl_Update
    @Id INT,
    @PRId INT,
    @ProductId INT,
    @PRDtlQty INT,
    @PRDtlPrice DECIMAL(18, 2),
    @PRDtlTotal DECIMAL(18, 2),
	@PRDtlTotalAfterVAT DECIMAL(18, 2)
AS
BEGIN
    UPDATE PRDtls
    SET
        PRId = @PRId,
        ProductId = @ProductId,
        PRDtlQty = @PRDtlQty,
        PRDtlPrice = @PRDtlPrice,
        PRDtlTotal = @PRDtlTotal,
		PRDtlTotalAfterVAT = @PRDtlTotalAfterVAT
    WHERE Id = @Id;
END;





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtl_Delete')
    DROP PROCEDURE [dbo].[spPRDtl_Delete];
GO
CREATE PROCEDURE spPRDtl_Delete
    @Id INT
AS
BEGIN
    DELETE FROM PRDtls
    WHERE Id = @Id;
END;






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtl_DeleteByPRId')
    DROP PROCEDURE [dbo].[spPRDtl_DeleteByPRId];
GO
CREATE PROCEDURE spPRDtl_DeleteByPRId
    @PRId INT
AS
BEGIN
    DELETE FROM PRDtls
    WHERE PRId = @PRId;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtl_GetAll')
    DROP PROCEDURE [dbo].[spPRDtl_GetAll];
GO
CREATE PROCEDURE spPRDtl_GetAll
AS
BEGIN
    SELECT 
        PRd.Id,
        PRd.PRId,
        PRd.ProductId,
        PRd.PRDtlQty,
        PRd.PRDtlPrice,
        PRd.PRDtlTotal,
		PRd.PRDtlTotalAfterVAT,
		--______ JOin column _________
		p.ProductName
    FROM PRDtls PRd
    LEFT JOIN Products p ON p.Id = PRd.ProductId;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtl_GetById')
    DROP PROCEDURE [dbo].[spPRDtl_GetById];
GO
CREATE PROCEDURE spPRDtl_GetById
    @Id INT
AS
BEGIN
    SELECT 
        PRd.Id,
        PRd.PRId,
        PRd.ProductId,
        PRd.PRDtlQty,
        PRd.PRDtlPrice,
        PRd.PRDtlTotal,
		PRd.PRDtlTotalAfterVAT,
		--______ JOin column _________
		p.ProductName
    FROM PRDtls PRd
    LEFT JOIN Products p ON p.Id = PRd.ProductId
    WHERE PRd.Id = @Id;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRDtl_GetAllByPRId')
    DROP PROCEDURE [dbo].[spPRDtl_GetAllByPRId];
GO
CREATE PROCEDURE spPRDtl_GetAllByPRId
    @PRId INT
AS
BEGIN
    SELECT 
        PRd.Id,
        PRd.PRId,
        PRd.ProductId,
        PRd.PRDtlQty,
        PRd.PRDtlPrice,
        PRd.PRDtlTotal,
		PRd.PRDtlTotalAfterVAT,
		--______ JOin column _________
		p.ProductName
    FROM PRDtls PRd
    LEFT JOIN Products p ON p.Id = PRd.ProductId
    WHERE PRd.PRId = @PRId;
END;




