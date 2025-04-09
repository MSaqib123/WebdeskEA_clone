
--===========================================================
--=====================  OSDtl Section  ======================
--===========================================================

--===========================================
-- OSDtl Insert
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOSDtl_Insert')
    DROP PROCEDURE [dbo].[spOSDtl_Insert];
GO
CREATE PROCEDURE spOSDtl_Insert
    @OSId INT,
    @ProductId INT,
    @OSDtlQty INT,
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO OSDtls (OSId, ProductId, OSDtlQty)
    VALUES (@OSId, @ProductId, @OSDtlQty);

    SET @Id = SCOPE_IDENTITY();
END;

--===========================================
-- OSDtl Update
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOSDtl_Update')
    DROP PROCEDURE [dbo].[spOSDtl_Update];
GO
CREATE PROCEDURE spOSDtl_Update
    @Id INT,
    @OSId INT,
    @ProductId INT,
    @OSDtlQty INT
AS
BEGIN
    UPDATE OSDtls
    SET 
        OSId = @OSId,
        ProductId = @ProductId,
        OSDtlQty = @OSDtlQty
    WHERE Id = @Id;
END;

--===========================================
-- OSDtl Delete
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOSDtl_Delete')
    DROP PROCEDURE [dbo].[spOSDtl_Delete];
GO
CREATE PROCEDURE spOSDtl_Delete
    @Id INT
AS
BEGIN
    DELETE FROM OSDtls
    WHERE Id = @Id;
END;

--===========================================
-- OSDtl GetAll
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOSDtl_GetAll')
    DROP PROCEDURE [dbo].[spOSDtl_GetAll];
GO
CREATE PROCEDURE spOSDtl_GetAll
AS
BEGIN
    SELECT *
    FROM OSDtls;
END;

--===========================================
-- OSDtl GetById
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOSDtl_GetById')
    DROP PROCEDURE [dbo].[spOSDtl_GetById];
GO
CREATE PROCEDURE spOSDtl_GetById
    @Id INT
AS
BEGIN
    SELECT *
    FROM OSDtls
    WHERE Id = @Id;
END;

--===========================================
-- OSDtl GetAllByOSId
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOSDtl_GetAllByOSId')
    DROP PROCEDURE [dbo].[spOSDtl_GetAllByOSId];
GO
CREATE PROCEDURE spOSDtl_GetAllByOSId
    @OSId INT
AS
BEGIN
    SELECT *
    FROM OSDtls
    WHERE OSId = @OSId;
END;

--===========================================
-- OSDtl DeleteByOSId
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOSDtl_DeleteByOSId')
    DROP PROCEDURE [dbo].[spOSDtl_DeleteByOSId];
GO
CREATE PROCEDURE spOSDtl_DeleteByOSId
    @OSId INT
AS
BEGIN
    DELETE FROM OSDtls
    WHERE OSId = @OSId;
END;

--===========================================================
--========================== END ============================
--===========================================================
