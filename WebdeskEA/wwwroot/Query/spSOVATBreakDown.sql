--CREATE TABLE [dbo].[SOVATBreakdown](
--    [Id] [int] IDENTITY(1,1) NOT NULL,
--    [SOId] [int] NOT NULL,
--    [TaxId] [int] NOT NULL,
--    [TaxName] [nvarchar](100) NOT NULL,
--    [TaxAmount] [decimal](18,2) NOT NULL,
--)


GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSOVATBreakdown_Insert')
    DROP PROCEDURE [dbo].[spSOVATBreakdown_Insert];
GO
CREATE PROCEDURE spSOVATBreakdown_Insert
    @SOId INT,
    @TaxId INT,
    @TaxName NVARCHAR(100),
    @TaxAmount DECIMAL(18,2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO SOVATBreakdown (SOId, TaxId, TaxName, TaxAmount)
    VALUES (@SOId, @TaxId, @TaxName, @TaxAmount);

    SET @Id = SCOPE_IDENTITY();
END;
GO






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSOVATBreakdown_Update')
    DROP PROCEDURE [dbo].[spSOVATBreakdown_Update];
GO
CREATE PROCEDURE spSOVATBreakdown_Update
    @Id INT,
    @SOId INT,
    @TaxId INT,
    @TaxName NVARCHAR(100),
    @TaxAmount DECIMAL(18,2)
AS
BEGIN
    UPDATE SOVATBreakdown
    SET 
        SOId = @SOId,
        TaxId = @TaxId,
        TaxName = @TaxName,
        TaxAmount = @TaxAmount
    WHERE Id = @Id;
END;
GO






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSOVATBreakdown_Delete')
    DROP PROCEDURE [dbo].[spSOVATBreakdown_Delete];
GO
CREATE PROCEDURE spSOVATBreakdown_Delete
    @Id INT
AS
BEGIN
    DELETE FROM SOVATBreakdown
    WHERE Id = @Id;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSOVATBreakdown_GetAll')
    DROP PROCEDURE [dbo].[spSOVATBreakdown_GetAll];
GO
CREATE PROCEDURE spSOVATBreakdown_GetAll
AS
BEGIN
    SELECT 
        Id,
        SOId,
        TaxId,
        TaxName,
        TaxAmount
    FROM SOVATBreakdown;
END;
GO









GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSOVATBreakdown_GetById')
    DROP PROCEDURE [dbo].[spSOVATBreakdown_GetById];
GO
CREATE PROCEDURE spSOVATBreakdown_GetById
    @Id INT
AS
BEGIN
    SELECT 
        Id,
        SOId,
        TaxId,
        TaxName,
        TaxAmount
    FROM SOVATBreakdown
    WHERE Id = @Id;
END;
GO








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSOVATBreakdown_GetAllBySOId')
    DROP PROCEDURE [dbo].[spSOVATBreakdown_GetAllBySOId];
GO
CREATE PROCEDURE spSOVATBreakdown_GetAllBySOId
    @SOId INT
AS
BEGIN
    SELECT 
        Id,
        SOId,
        TaxId,
        TaxName,
        TaxAmount
    FROM SOVATBreakdown
    WHERE SOId = @SOId;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSOVATBreakdown_DeleteBySOId')
    DROP PROCEDURE [dbo].[spSOVATBreakdown_DeleteBySOId];
GO
CREATE PROCEDURE spSOVATBreakdown_DeleteBySOId
    @SOId INT
AS
BEGIN
    DELETE FROM SOVATBreakdown
    WHERE SOId = @SOId;
END;
GO
