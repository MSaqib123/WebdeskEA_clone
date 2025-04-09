--CREATE TABLE [dbo].[SIVATBreakdown](
--    [Id] [int] IDENTITY(1,1) NOT NULL,
--    [SIId] [int] NOT NULL,
--    [TaxId] [int] NOT NULL,
--    [TaxName] [nvarchar](100) NOT NULL,
--    [TaxAmount] [decimal](18,2) NOT NULL,
--)


GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIVATBreakdown_Insert')
    DROP PROCEDURE [dbo].[spSIVATBreakdown_Insert];
GO
CREATE PROCEDURE spSIVATBreakdown_Insert
    @SIId INT,
    @TaxId INT,
    @TaxName NVARCHAR(100),
    @TaxAmount DECIMAL(18,2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO SIVATBreakdown (SIId, TaxId, TaxName, TaxAmount)
    VALUES (@SIId, @TaxId, @TaxName, @TaxAmount);

    SET @Id = SCOPE_IDENTITY();
END;
GO






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIVATBreakdown_Update')
    DROP PROCEDURE [dbo].[spSIVATBreakdown_Update];
GO
CREATE PROCEDURE spSIVATBreakdown_Update
    @Id INT,
    @SIId INT,
    @TaxId INT,
    @TaxName NVARCHAR(100),
    @TaxAmount DECIMAL(18,2)
AS
BEGIN
    UPDATE SIVATBreakdown
    SET 
        SIId = @SIId,
        TaxId = @TaxId,
        TaxName = @TaxName,
        TaxAmount = @TaxAmount
    WHERE Id = @Id;
END;
GO






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIVATBreakdown_Delete')
    DROP PROCEDURE [dbo].[spSIVATBreakdown_Delete];
GO
CREATE PROCEDURE spSIVATBreakdown_Delete
    @Id INT
AS
BEGIN
    DELETE FROM SIVATBreakdown
    WHERE Id = @Id;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIVATBreakdown_GetAll')
    DROP PROCEDURE [dbo].[spSIVATBreakdown_GetAll];
GO
CREATE PROCEDURE spSIVATBreakdown_GetAll
AS
BEGIN
    SELECT 
        Id,
        SIId,
        TaxId,
        TaxName,
        TaxAmount
    FROM SIVATBreakdown;
END;
GO









GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIVATBreakdown_GetById')
    DROP PROCEDURE [dbo].[spSIVATBreakdown_GetById];
GO
CREATE PROCEDURE spSIVATBreakdown_GetById
    @Id INT
AS
BEGIN
    SELECT 
        Id,
        SIId,
        TaxId,
        TaxName,
        TaxAmount
    FROM SIVATBreakdown
    WHERE Id = @Id;
END;
GO








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIVATBreakdown_GetAllBySIId')
    DROP PROCEDURE [dbo].[spSIVATBreakdown_GetAllBySIId];
GO
CREATE PROCEDURE spSIVATBreakdown_GetAllBySIId
    @SIId INT
AS
BEGIN
    SELECT 
        Id,
        SIId,
        TaxId,
        TaxName,
        TaxAmount
    FROM SIVATBreakdown
    WHERE SIId = @SIId;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSIVATBreakdown_DeleteBySIId')
    DROP PROCEDURE [dbo].[spSIVATBreakdown_DeleteBySIId];
GO
CREATE PROCEDURE spSIVATBreakdown_DeleteBySIId
    @SIId INT
AS
BEGIN
    DELETE FROM SIVATBreakdown
    WHERE SIId = @SIId;
END;
GO
