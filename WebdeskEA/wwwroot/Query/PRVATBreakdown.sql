CREATE TABLE [dbo].[PRVATBreakdown](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [PRId] [int] NOT NULL,
    [TaxId] [int] NOT NULL,
    [TaxName] [nvarchar](100) NOT NULL,
    [TaxAmount] [decimal](18,2) NOT NULL,
)


GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRVATBreakdown_Insert')
    DROP PROCEDURE [dbo].[spPRVATBreakdown_Insert];
GO
CREATE PROCEDURE spPRVATBreakdown_Insert
    @PRId INT,
    @TaxId INT,
    @TaxName NVARCHAR(100),
    @TaxAmount DECIMAL(18,2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO PRVATBreakdown (PRId, TaxId, TaxName, TaxAmount)
    VALUES (@PRId, @TaxId, @TaxName, @TaxAmount);

    SET @Id = SCOPE_IDENTITY();
END;
GO






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRVATBreakdown_Update')
    DROP PROCEDURE [dbo].[spPRVATBreakdown_Update];
GO
CREATE PROCEDURE spPRVATBreakdown_Update
    @Id INT,
    @PRId INT,
    @TaxId INT,
    @TaxName NVARCHAR(100),
    @TaxAmount DECIMAL(18,2)
AS
BEGIN
    UPDATE PRVATBreakdown
    SET 
        PRId = @PRId,
        TaxId = @TaxId,
        TaxName = @TaxName,
        TaxAmount = @TaxAmount
    WHERE Id = @Id;
END;
GO






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRVATBreakdown_Delete')
    DROP PROCEDURE [dbo].[spPRVATBreakdown_Delete];
GO
CREATE PROCEDURE spPRVATBreakdown_Delete
    @Id INT
AS
BEGIN
    DELETE FROM PRVATBreakdown
    WHERE Id = @Id;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRVATBreakdown_GetAll')
    DROP PROCEDURE [dbo].[spPRVATBreakdown_GetAll];
GO
CREATE PROCEDURE spPRVATBreakdown_GetAll
AS
BEGIN
    SELECT 
        Id,
        PRId,
        TaxId,
        TaxName,
        TaxAmount
    FROM PRVATBreakdown;
END;
GO









GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRVATBreakdown_GetById')
    DROP PROCEDURE [dbo].[spPRVATBreakdown_GetById];
GO
CREATE PROCEDURE spPRVATBreakdown_GetById
    @Id INT
AS
BEGIN
    SELECT 
        Id,
        PRId,
        TaxId,
        TaxName,
        TaxAmount
    FROM PRVATBreakdown
    WHERE Id = @Id;
END;
GO








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRVATBreakdown_GetAllByPRId')
    DROP PROCEDURE [dbo].[spPRVATBreakdown_GetAllByPRId];
GO
CREATE PROCEDURE spPRVATBreakdown_GetAllByPRId
    @PRId INT
AS
BEGIN
    SELECT 
        Id,
        PRId,
        TaxId,
        TaxName,
        TaxAmount
    FROM PRVATBreakdown
    WHERE PRId = @PRId;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPRVATBreakdown_DeleteByPRId')
    DROP PROCEDURE [dbo].[spPRVATBreakdown_DeleteByPRId];
GO
CREATE PROCEDURE spPRVATBreakdown_DeleteByPRId
    @PRId INT
AS
BEGIN
    DELETE FROM PRVATBreakdown
    WHERE PRId = @PRId;
END;
GO
