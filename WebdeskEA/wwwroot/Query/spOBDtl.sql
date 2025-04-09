--===========================================================
--======================  OBDtl Section  =====================
--===========================================================

--===========================================
-- OBDtl Insert
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOBDtl_Insert')
    DROP PROCEDURE [dbo].[spOBDtl_Insert];
GO
CREATE PROCEDURE spOBDtl_Insert
    @OBId INT,
    @COAId INT,
    @OBDtlTranType VARCHAR(50),
    @OBDtlOpenBlnc DECIMAL(18,2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO OBDtls (OBId, COAId, OBDtlTranType, OBDtlOpenBlnc)
    VALUES (@OBId, @COAId, @OBDtlTranType, @OBDtlOpenBlnc);

    SET @Id = SCOPE_IDENTITY();
END;

--===========================================
-- OBDtl Update
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOBDtl_Update')
    DROP PROCEDURE [dbo].[spOBDtl_Update];
GO
CREATE PROCEDURE spOBDtl_Update
    @Id INT,
    @OBId INT,
    @COAId INT,
    @OBDtlTranType VARCHAR(50),
    @OBDtlOpenBlnc DECIMAL(18,2)
AS
BEGIN
    UPDATE OBDtls
    SET 
        OBId = @OBId,
        COAId = @COAId,
        OBDtlTranType = @OBDtlTranType,
        OBDtlOpenBlnc = @OBDtlOpenBlnc
    WHERE Id = @Id;
END;

--===========================================
-- OBDtl Delete
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOBDtl_Delete')
    DROP PROCEDURE [dbo].[spOBDtl_Delete];
GO
CREATE PROCEDURE spOBDtl_Delete
    @Id INT
AS
BEGIN
    DELETE FROM OBDtls
    WHERE Id = @Id;
END;

--===========================================
-- OBDtl GetAll
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOBDtl_GetAll')
    DROP PROCEDURE [dbo].[spOBDtl_GetAll];
GO
CREATE PROCEDURE spOBDtl_GetAll
AS
BEGIN
    SELECT *
    FROM OBDtls;
END;

--===========================================
-- OBDtl GetById
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOBDtl_GetById')
    DROP PROCEDURE [dbo].[spOBDtl_GetById];
GO
CREATE PROCEDURE spOBDtl_GetById
    @Id INT
AS
BEGIN
    SELECT *
    FROM OBDtls
    WHERE Id = @Id;
END;

--===========================================
-- OBDtl GetAllByOBId
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOBDtl_GetAllByOBId')
    DROP PROCEDURE [dbo].[spOBDtl_GetAllByOBId];
GO
CREATE PROCEDURE spOBDtl_GetAllByOBId
    @OBId INT
AS
BEGIN
    SELECT *
    FROM OBDtls
    WHERE OBId = @OBId;
END;

--===========================================
-- OBDtl DeleteByOBId
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spOBDtl_DeleteByOBId')
    DROP PROCEDURE [dbo].[spOBDtl_DeleteByOBId];
GO
CREATE PROCEDURE spOBDtl_DeleteByOBId
    @OBId INT
AS
BEGIN
    DELETE FROM OBDtls
    WHERE OBId = @OBId;
END;
