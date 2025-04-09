--_____________________________________________ spCOAType_Delete _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOAType_Delete') DROP PROCEDURE [dbo].[spCOAType_Delete]
GO
CREATE PROCEDURE [dbo].[spCOAType_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM Coatypes
    WHERE Id = @Id;
END
GO
--_____________________________________________ spCOAType_GetAll _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOAType_GetAll') DROP PROCEDURE [dbo].[spCOAType_GetAll]
GO
CREATE PROCEDURE [dbo].[spCOAType_GetAll]
AS
BEGIN
    SELECT Id, COATypeName
    FROM Coatypes
END
GO
--_____________________________________________ spCOAType_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOAType_GetById') DROP PROCEDURE [dbo].[spCOAType_GetById]
GO
CREATE PROCEDURE [dbo].[spCOAType_GetById]
    @Id INT
AS
BEGIN
    SELECT Id, COATypeName
    FROM Coatypes
    WHERE Id = @Id;
END
GO
--_____________________________________________ spCOAType_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOAType_Insert') DROP PROCEDURE [dbo].[spCOAType_Insert]
GO
CREATE PROCEDURE [dbo].[spCOAType_Insert]
    @COATypeName NVARCHAR(100),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO Coatypes (COATypeName)
    VALUES (@COATypeName);

    SET @Id = SCOPE_IDENTITY();
END
GO
--_____________________________________________ spCOAType_Update _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCOAType_Update') DROP PROCEDURE [dbo].[spCOAType_Update]
GO
CREATE PROCEDURE [dbo].[spCOAType_Update]
    @Id INT,
    @COATypeName NVARCHAR(100)
AS
BEGIN
    UPDATE Coatypes
    SET COATypeName = @COATypeName
    WHERE Id = @Id;
END
GO