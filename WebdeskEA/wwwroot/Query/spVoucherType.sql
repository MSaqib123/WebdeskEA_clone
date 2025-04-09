-- Create VoucherTypes table
CREATE TABLE VoucherTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VoucherTypeName NVARCHAR(100) NOT NULL
);





IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherType_Delete')
    DROP PROCEDURE [dbo].[spVoucherType_Delete]
GO
CREATE PROCEDURE [dbo].[spVoucherType_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM VoucherTypes
    WHERE Id = @Id;
END
GO








IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherType_GetAll')
    DROP PROCEDURE [dbo].[spVoucherType_GetAll]
GO
CREATE PROCEDURE [dbo].[spVoucherType_GetAll]
AS
BEGIN
    SELECT 
        Id,
        VoucherTypeName
    FROM VoucherTypes;
END
GO







IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherType_GetById')
    DROP PROCEDURE [dbo].[spVoucherType_GetById]
GO
CREATE PROCEDURE [dbo].[spVoucherType_GetById]
    @Id INT
AS
BEGIN
    SELECT 
        Id,
        VoucherTypeName
    FROM VoucherTypes
    WHERE Id = @Id;
END
GO






IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherType_Insert')
    DROP PROCEDURE [dbo].[spVoucherType_Insert]
GO
CREATE PROCEDURE [dbo].[spVoucherType_Insert]
    @VoucherTypeName NVARCHAR(100),
    @Id INT OUTPUT
AS
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY
        INSERT INTO VoucherTypes (VoucherTypeName)
        VALUES (@VoucherTypeName);

        SET @Id = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO







IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherType_Update')
    DROP PROCEDURE [dbo].[spVoucherType_Update]
GO
CREATE PROCEDURE [dbo].[spVoucherType_Update]
    @Id INT,
    @VoucherTypeName NVARCHAR(100)
AS
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE VoucherTypes
        SET VoucherTypeName = @VoucherTypeName
        WHERE Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO







IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherType_GetByName')
    DROP PROCEDURE [dbo].[spVoucherType_GetByName]
GO
CREATE PROCEDURE [dbo].[spVoucherType_GetByName]
    @VoucherTypeName NVARCHAR(100)
AS
BEGIN
    SELECT 
        Id,
        VoucherTypeName
    FROM VoucherTypes
    WHERE VoucherTypeName LIKE '%' + @VoucherTypeName + '%';
END
GO








IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spVoucherType_GetMaxId')
    DROP PROCEDURE [dbo].[spVoucherType_GetMaxId]
GO
CREATE PROCEDURE [dbo].[spVoucherType_GetMaxId]
AS
BEGIN
    SELECT MAX(Id) AS MaxId
    FROM VoucherTypes;
END
GO
