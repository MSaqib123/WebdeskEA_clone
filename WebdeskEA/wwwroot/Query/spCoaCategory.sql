--===========================================
-- CoaCategory Insert
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCoaCategory_Insert')
    DROP PROCEDURE [dbo].[spCoaCategory_Insert];
GO
CREATE PROCEDURE spCoaCategory_Insert
    @CoaTypeId INT = NULL,
    @CoaCategoryName VARCHAR(50) = NULL,
    @CoaCategoryDescription VARCHAR(2000) = NULL,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.CoaCategory 
        (CoaTypeId, CoaCategoryName, CoaCategoryDescription)
    VALUES 
        (@CoaTypeId, @CoaCategoryName, @CoaCategoryDescription);

    SET @Id = SCOPE_IDENTITY();
END;
GO






--===========================================
-- CoaCategory Update
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCoaCategory_Update')
    DROP PROCEDURE [dbo].[spCoaCategory_Update];
GO
CREATE PROCEDURE spCoaCategory_Update
    @Id INT,
    @CoaTypeId INT = NULL,
    @CoaCategoryName VARCHAR(50) = NULL,
    @CoaCategoryDescription VARCHAR(2000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.CoaCategory
    SET 
        CoaTypeId = @CoaTypeId,
        CoaCategoryName = @CoaCategoryName,
        CoaCategoryDescription = @CoaCategoryDescription
    WHERE 
        Id = @Id;
END;
GO




--===========================================
-- CoaCategory Delete
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCoaCategory_Delete')
    DROP PROCEDURE [dbo].[spCoaCategory_Delete];
GO
CREATE PROCEDURE spCoaCategory_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.CoaCategory
    WHERE Id = @Id;
END;
GO




--===========================================
-- CoaCategory GetAll
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCoaCategory_GetAll')
    DROP PROCEDURE [dbo].[spCoaCategory_GetAll];
GO
CREATE PROCEDURE spCoaCategory_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        cc.Id,
        cc.CoaTypeId,
        ct.CoaTypeName, -- Assuming CoaType has a 'CoaTypeName' column
        cc.CoaCategoryName,
        cc.CoaCategoryDescription
    FROM dbo.CoaCategory cc
    LEFT JOIN dbo.CoaTypes ct ON ct.Id = cc.CoaTypeId;
END;
GO






--===========================================
-- CoaCategory GetById
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCoaCategory_GetById')
    DROP PROCEDURE [dbo].[spCoaCategory_GetById];
GO
CREATE PROCEDURE spCoaCategory_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        cc.Id,
        cc.CoaTypeId,
        ct.CoaTypeName, -- Assuming CoaType has a 'CoaTypeName' column
        cc.CoaCategoryName,
        cc.CoaCategoryDescription,
        cc.CreatedOn
    FROM dbo.CoaCategory cc
    LEFT JOIN dbo.CoaTypes ct ON ct.Id = cc.CoaTypeId
    WHERE cc.Id = @Id;
END;
GO





--===========================================
-- CoaCategory GetByCoaType
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCoaCategory_GetByCoaType')
    DROP PROCEDURE [dbo].[spCoaCategory_GetByCoaType];
GO
CREATE PROCEDURE spCoaCategory_GetByCoaType
    @CoaTypeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        cc.Id,
        cc.CoaTypeId,
        ct.CoaTypeName, -- Assuming CoaType has a 'CoaTypeName' column
        cc.CoaCategoryName,
        cc.CoaCategoryDescription
    FROM dbo.CoaCategory cc
    LEFT JOIN dbo.CoaTypes ct ON ct.Id = cc.CoaTypeId
    WHERE cc.CoaTypeId = @CoaTypeId;
END;
GO






--===========================================
-- CoaCategory Search
--===========================================
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCoaCategory_Search')
    DROP PROCEDURE [dbo].[spCoaCategory_Search];
GO
CREATE PROCEDURE spCoaCategory_Search
    @CoaCategoryName VARCHAR(50) = NULL,
    @Active BIT = NULL -- Assuming you have an 'Active' status; adjust as necessary
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        cc.Id,
        cc.CoaTypeId,
        ct.CoaTypeName, -- Assuming CoaType has a 'CoaTypeName' column
        cc.CoaCategoryName,
        cc.CoaCategoryDescription
    FROM dbo.CoaCategory cc
    LEFT JOIN dbo.CoaTypes ct ON ct.Id = cc.CoaTypeId
    WHERE 
        (@CoaCategoryName IS NULL OR cc.CoaCategoryName LIKE '%' + @CoaCategoryName + '%')
END;
GO
