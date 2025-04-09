--CREATE TABLE [dbo].[PIDtls](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[PIId] [int] NOT NULL,
--	[ProductId] [int] NOT NULL,
--	[PIDtlQty] [int] NOT NULL,
--	[PIDtlPrice] [decimal](18, 2) NOT NULL,
--	[PIDtlTotal] [decimal](18, 2) NOT NULL,
--)
--GO




GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtl_Insert')
    DROP PROCEDURE [dbo].[spPIDtl_Insert];
GO
CREATE PROCEDURE spPIDtl_Insert
    @PIId INT,
    @ProductId INT,
    @PIDtlQty INT,
    @PIDtlPrice DECIMAL(18, 2),
    @PIDtlTotal DECIMAL(18, 2),
	@PIDtlTotalAfterVAT DECIMAL(18, 2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO PIDtls (PIId, ProductId, PIDtlQty, PIDtlPrice, PIDtlTotal,PIDtlTotalAfterVAT)
    VALUES (@PIId, @ProductId, @PIDtlQty, @PIDtlPrice, @PIDtlTotal,@PIDtlTotalAfterVAT);

    SET @Id = SCOPE_IDENTITY();
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtl_Update')
    DROP PROCEDURE [dbo].[spPIDtl_Update];
GO
CREATE PROCEDURE spPIDtl_Update
    @Id INT,
    @PIId INT,
    @ProductId INT,
    @PIDtlQty INT,
    @PIDtlPrice DECIMAL(18, 2),
    @PIDtlTotal DECIMAL(18, 2),
	@PIDtlTotalAfterVAT DECIMAL(18, 2)
AS
BEGIN
    UPDATE PIDtls
    SET
        PIId = @PIId,
        ProductId = @ProductId,
        PIDtlQty = @PIDtlQty,
        PIDtlPrice = @PIDtlPrice,
        PIDtlTotal = @PIDtlTotal,
		PIDtlTotalAfterVAT = @PIDtlTotalAfterVAT
    WHERE Id = @Id;
END;





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtl_Delete')
    DROP PROCEDURE [dbo].[spPIDtl_Delete];
GO
CREATE PROCEDURE spPIDtl_Delete
    @Id INT
AS
BEGIN
    DELETE FROM PIDtls
    WHERE Id = @Id;
END;






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtl_DeleteByPIId')
    DROP PROCEDURE [dbo].[spPIDtl_DeleteByPIId];
GO
CREATE PROCEDURE spPIDtl_DeleteByPIId
    @PIId INT
AS
BEGIN
    DELETE FROM PIDtls
    WHERE PIId = @PIId;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtl_GetAll')
    DROP PROCEDURE [dbo].[spPIDtl_GetAll];
GO
CREATE PROCEDURE spPIDtl_GetAll
AS
BEGIN
    SELECT 
        pid.Id,
        pid.PIId,
        pid.ProductId,
        pid.PIDtlQty,
        pid.PIDtlPrice,
        pid.PIDtlTotal,
		pid.PIDtlTotalAfterVAT,
		--______ JOin column _________
		p.ProductName
    FROM PIDtls pid
    LEFT JOIN Products p ON p.Id = pid.ProductId;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtl_GetById')
    DROP PROCEDURE [dbo].[spPIDtl_GetById];
GO
CREATE PROCEDURE spPIDtl_GetById
    @Id INT
AS
BEGIN
    SELECT 
        pid.Id,
        pid.PIId,
        pid.ProductId,
        pid.PIDtlQty,
        pid.PIDtlPrice,
        pid.PIDtlTotal,
		pid.PIDtlTotalAfterVAT,
		--______ JOin column _________
		p.ProductName
    FROM PIDtls pid
    LEFT JOIN Products p ON p.Id = pid.ProductId
    WHERE pid.Id = @Id;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPIDtl_GetAllByPIId')
    DROP PROCEDURE [dbo].[spPIDtl_GetAllByPIId];
GO
CREATE PROCEDURE spPIDtl_GetAllByPIId
    @PIId INT
AS
BEGIN
    SELECT 
        pid.Id,
        pid.PIId,
        pid.ProductId,
        pid.PIDtlQty,
        pid.PIDtlPrice,
        pid.PIDtlTotal,
		pid.PIDtlTotalAfterVAT,
		--______ JOin column _________
		p.ProductName
    FROM PIDtls pid
    LEFT JOIN Products p ON p.Id = pid.ProductId
    WHERE pid.PIId = @PIId;
END;




