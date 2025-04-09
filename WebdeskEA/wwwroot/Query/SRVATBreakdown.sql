CREATE TABLE [dbo].[SRVATBreakdown](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SRId] [int] NOT NULL,
    [TaxId] [int] NOT NULL,
    [TaxName] [nvarchar](100) NOT NULL,
    [TaxAmount] [decimal](18,2) NOT NULL,
)


GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRVATBreakdown_Insert')
    DROP PROCEDURE [dbo].[spSRVATBreakdown_Insert];
GO
CREATE PROCEDURE spSRVATBreakdown_Insert
    @SRId INT,
    @TaxId INT,
    @TaxName NVARCHAR(100),
    @TaxAmount DECIMAL(18,2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO SRVATBreakdown (SRId, TaxId, TaxName, TaxAmount)
    VALUES (@SRId, @TaxId, @TaxName, @TaxAmount);

    SET @Id = SCOPE_IDENTITY();
END;
GO






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRVATBreakdown_Update')
    DROP PROCEDURE [dbo].[spSRVATBreakdown_Update];
GO
CREATE PROCEDURE spSRVATBreakdown_Update
    @Id INT,
    @SRId INT,
    @TaxId INT,
    @TaxName NVARCHAR(100),
    @TaxAmount DECIMAL(18,2)
AS
BEGIN
    UPDATE SRVATBreakdown
    SET 
        SRId = @SRId,
        TaxId = @TaxId,
        TaxName = @TaxName,
        TaxAmount = @TaxAmount
    WHERE Id = @Id;
END;
GO






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRVATBreakdown_Delete')
    DROP PROCEDURE [dbo].[spSRVATBreakdown_Delete];
GO
CREATE PROCEDURE spSRVATBreakdown_Delete
    @Id INT
AS
BEGIN
    DELETE FROM SRVATBreakdown
    WHERE Id = @Id;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRVATBreakdown_GetAll')
    DROP PROCEDURE [dbo].[spSRVATBreakdown_GetAll];
GO
CREATE PROCEDURE spSRVATBreakdown_GetAll
AS
BEGIN
    SELECT 
        Id,
        SRId,
        TaxId,
        TaxName,
        TaxAmount
    FROM SRVATBreakdown;
END;
GO









GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRVATBreakdown_GetById')
    DROP PROCEDURE [dbo].[spSRVATBreakdown_GetById];
GO
CREATE PROCEDURE spSRVATBreakdown_GetById
    @Id INT
AS
BEGIN
    SELECT 
        Id,
        SRId,
        TaxId,
        TaxName,
        TaxAmount
    FROM SRVATBreakdown
    WHERE Id = @Id;
END;
GO








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRVATBreakdown_GetAllBySRId')
    DROP PROCEDURE [dbo].[spSRVATBreakdown_GetAllBySRId];
GO
CREATE PROCEDURE spSRVATBreakdown_GetAllBySRId
    @SRId INT
AS
BEGIN
    SELECT 
        Id,
        SRId,
        TaxId,
        TaxName,
        TaxAmount
    FROM SRVATBreakdown
    WHERE SRId = @SRId;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSRVATBreakdown_DeleteBySRId')
    DROP PROCEDURE [dbo].[spSRVATBreakdown_DeleteBySRId];
GO
CREATE PROCEDURE spSRVATBreakdown_DeleteBySRId
    @SRId INT
AS
BEGIN
    DELETE FROM SRVATBreakdown
    WHERE SRId = @SRId;
END;
GO
