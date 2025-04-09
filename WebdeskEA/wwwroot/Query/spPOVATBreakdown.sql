
CREATE TABLE [dbo].[POVATBreakdown](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [POId] [int] NOT NULL,
    [TaxId] [int] NOT NULL,
    [TaxName] [nvarchar](100) NOT NULL,
    [TaxAmount] [decimal](18,2) NOT NULL,
)


GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOVATBreakdown_Insert')
    DROP PROCEDURE [dbo].[spPOVATBreakdown_Insert];
GO
CREATE PROCEDURE spPOVATBreakdown_Insert
    @POId INT,
    @TaxId INT,
    @TaxName NVARCHAR(100),
    @TaxAmount DECIMAL(18,2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO POVATBreakdown (POId, TaxId, TaxName, TaxAmount)
    VALUES (@POId, @TaxId, @TaxName, @TaxAmount);

    SET @Id = SCOPE_IDENTITY();
END;
GO






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOVATBreakdown_Update')
    DROP PROCEDURE [dbo].[spPOVATBreakdown_Update];
GO
CREATE PROCEDURE spPOVATBreakdown_Update
    @Id INT,
    @POId INT,
    @TaxId INT,
    @TaxName NVARCHAR(100),
    @TaxAmount DECIMAL(18,2)
AS
BEGIN
    UPDATE POVATBreakdown
    SET 
        POId = @POId,
        TaxId = @TaxId,
        TaxName = @TaxName,
        TaxAmount = @TaxAmount
    WHERE Id = @Id;
END;
GO






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOVATBreakdown_Delete')
    DROP PROCEDURE [dbo].[spPOVATBreakdown_Delete];
GO
CREATE PROCEDURE spPOVATBreakdown_Delete
    @Id INT
AS
BEGIN
    DELETE FROM POVATBreakdown
    WHERE Id = @Id;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOVATBreakdown_GetAll')
    DROP PROCEDURE [dbo].[spPOVATBreakdown_GetAll];
GO
CREATE PROCEDURE spPOVATBreakdown_GetAll
AS
BEGIN
    SELECT 
        Id,
        POId,
        TaxId,
        TaxName,
        TaxAmount
    FROM POVATBreakdown;
END;
GO









GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOVATBreakdown_GetById')
    DROP PROCEDURE [dbo].[spPOVATBreakdown_GetById];
GO
CREATE PROCEDURE spPOVATBreakdown_GetById
    @Id INT
AS
BEGIN
    SELECT 
        Id,
        POId,
        TaxId,
        TaxName,
        TaxAmount
    FROM POVATBreakdown
    WHERE Id = @Id;
END;
GO








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOVATBreakdown_GetAllByPOId')
    DROP PROCEDURE [dbo].[spPOVATBreakdown_GetAllByPOId];
GO
CREATE PROCEDURE spPOVATBreakdown_GetAllByPOId
    @POId INT
AS
BEGIN
    SELECT 
        Id,
        POId,
        TaxId,
        TaxName,
        TaxAmount
    FROM POVATBreakdown
    WHERE POId = @POId;
END;
GO







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOVATBreakdown_DeleteByPOId')
    DROP PROCEDURE [dbo].[spPOVATBreakdown_DeleteByPOId];
GO
CREATE PROCEDURE spPOVATBreakdown_DeleteByPOId
    @POId INT
AS
BEGIN
    DELETE FROM POVATBreakdown
    WHERE POId = @POId;
END;
GO
