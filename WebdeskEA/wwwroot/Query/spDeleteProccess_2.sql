IF OBJECT_ID('dbo.sp_DeleteEntityWithValidation', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DeleteEntityWithValidation;
GO

CREATE PROCEDURE dbo.sp_DeleteEntityWithValidation
    @TableName       NVARCHAR(100),
    @PrimaryKeyValue INT,
    @DeleteType      INT,
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelations BIT = 0;
    DECLARE @sql NVARCHAR(MAX);  -- Declare once, reuse in each branch

    /*
       Example relationship checks:
       IF EXISTS (SELECT 1 FROM SomeRelatedTable WHERE ForeignKeyId = @PrimaryKeyValue)
       BEGIN
           SET @HasRelations = 1;
       END
    */

    IF @DeleteType = 1 -- Soft Delete
    BEGIN
        SET @sql = '
            UPDATE ' + QUOTENAME(@TableName) + ' 
            SET Active = 0
            WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));

        EXEC(@sql);

        SET @OutputMessage = 'Record soft deleted successfully.';
    END
    ELSE IF @DeleteType = 2 -- Permanent Delete
    BEGIN
        SET @sql = '
            DELETE FROM ' + QUOTENAME(@TableName) + '
            WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));

        EXEC(@sql);

        SET @OutputMessage = 'Record permanently deleted successfully.';
    END
    ELSE IF @DeleteType = 3 -- Soft Delete If No Nested Relation
    BEGIN
        IF @HasRelations = 1
        BEGIN
            SET @OutputMessage = 'Cannot soft delete because related records exist.';
            RETURN;
        END

        SET @sql = '
            UPDATE ' + QUOTENAME(@TableName) + '
            SET Active = 0
            WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));

        EXEC(@sql);

        SET @OutputMessage = 'Record soft deleted successfully (no relations).';
    END
    ELSE IF @DeleteType = 4 -- Permanent Delete If No Nested Relation
    BEGIN
        IF @HasRelations = 1
        BEGIN
            SET @OutputMessage = 'Cannot permanently delete because related records exist.';
            RETURN;
        END

        SET @sql = '
            DELETE FROM ' + QUOTENAME(@TableName) + '
            WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));

        EXEC(@sql);

        SET @OutputMessage = 'Record permanently deleted successfully (no relations).';
    END
END
GO










IF OBJECT_ID('dbo.sp_DeleteWithRelations', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DeleteWithRelations;
GO

CREATE PROCEDURE dbo.sp_DeleteWithRelations
    @TableName        NVARCHAR(100),
    @PrimaryKeyValue  INT,
    @DeleteType       INT,
    @RefreshTables    NVARCHAR(MAX),
    @OutputMessage    NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelations  BIT = 0;
    DECLARE @RelatedTable  NVARCHAR(100);
    DECLARE @Pos INT;
    DECLARE @Len INT;
    DECLARE @TempStr NVARCHAR(MAX);
    DECLARE @sql NVARCHAR(MAX); -- Declare once here

    SET @TempStr = LTRIM(RTRIM(@RefreshTables));
    SET @Pos = CHARINDEX(',', @TempStr);
    SET @Len = LEN(@TempStr);

    WHILE @Pos > 0
    BEGIN
        SET @RelatedTable = LTRIM(RTRIM(SUBSTRING(@TempStr, 1, @Pos - 1)));
        SET @TempStr = SUBSTRING(@TempStr, @Pos + 1, @Len);
        SET @TempStr = LTRIM(RTRIM(@TempStr));
        SET @Len = LEN(@TempStr);
        SET @Pos = CHARINDEX(',', @TempStr);

        IF @RelatedTable <> ''
        BEGIN
            SET @sql = '
                IF EXISTS (SELECT 1 FROM ' + QUOTENAME(@RelatedTable) + '
                           WHERE ForeignKeyId = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + ')
                BEGIN
                    SET @HasRelations = 1
                END
            ';
            EXEC sp_executesql @sql, N'@HasRelations BIT OUTPUT', @HasRelations OUTPUT;

            IF @HasRelations = 1
            BEGIN
                SET @OutputMessage = 'Cannot delete because related records exist in ' + @RelatedTable + '.';
                RETURN;
            END
        END
    END

    -- Handle the last (or only) table
    SET @RelatedTable = LTRIM(RTRIM(@TempStr));
    IF @RelatedTable <> ''
    BEGIN
        SET @sql = '
            IF EXISTS (SELECT 1 FROM ' + QUOTENAME(@RelatedTable) + '
                       WHERE ForeignKeyId = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + ')
            BEGIN
                SET @HasRelations = 1
            END
        ';
        EXEC sp_executesql @sql, N'@HasRelations BIT OUTPUT', @HasRelations OUTPUT;

        IF @HasRelations = 1
        BEGIN
            SET @OutputMessage = 'Cannot delete because related records exist in ' + @RelatedTable + '.';
            RETURN;
        END
    END

    -- Now do the delete/soft-delete logic
    IF @DeleteType = 1 -- Soft Delete
    BEGIN
        SET @sql = '
            UPDATE ' + QUOTENAME(@TableName) + '
            SET Active = 0
            WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
        EXEC(@sql);

        SET @OutputMessage = 'Record soft deleted successfully.';
    END
    ELSE IF @DeleteType = 2 -- Permanent Delete
    BEGIN
        SET @sql = '
            DELETE FROM ' + QUOTENAME(@TableName) + '
            WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
        EXEC(@sql);

        SET @OutputMessage = 'Record permanently deleted successfully.';
    END
    ELSE IF @DeleteType = 3 -- Soft Delete (no nested relation)
    BEGIN
        SET @sql = '
            UPDATE ' + QUOTENAME(@TableName) + '
            SET Active = 0
            WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
        EXEC(@sql);

        SET @OutputMessage = 'Record soft deleted (refresh logic).';
    END
    ELSE IF @DeleteType = 4 -- Permanent Delete (no nested relation)
    BEGIN
        SET @sql = '
            DELETE FROM ' + QUOTENAME(@TableName) + '
            WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
        EXEC(@sql);

        SET @OutputMessage = 'Record permanently deleted (refresh logic).';
    END
END
GO










IF OBJECT_ID('dbo.sp_ForceSoftDeleteWithReferences', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ForceSoftDeleteWithReferences;
GO

CREATE PROCEDURE dbo.sp_ForceSoftDeleteWithReferences
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @ReferenceTables NVARCHAR(MAX),  -- e.g., "PODetail,PI"
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelation BIT = 0;
    DECLARE @Table NVARCHAR(100);
    DECLARE @Pos INT;
    DECLARE @Len INT;
    DECLARE @TempStr NVARCHAR(MAX);
    DECLARE @sql NVARCHAR(MAX);  -- declare once

    SET @TempStr = LTRIM(RTRIM(@ReferenceTables));
    SET @Pos = CHARINDEX(',', @TempStr);
    SET @Len = LEN(@TempStr);

    WHILE @Pos > 0
    BEGIN
        SET @Table = LTRIM(RTRIM(SUBSTRING(@TempStr, 1, @Pos - 1)));
        SET @TempStr = SUBSTRING(@TempStr, @Pos + 1, @Len);
        SET @TempStr = LTRIM(RTRIM(@TempStr));
        SET @Len = LEN(@TempStr);
        SET @Pos = CHARINDEX(',', @TempStr);

        IF @Table <> ''
        BEGIN
            SET @sql = '
                IF EXISTS (SELECT 1 FROM ' + QUOTENAME(@Table) + '
                           WHERE ForeignKeyId = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + ')
                BEGIN
                    SET @HasRelation = 1
                END
            ';
            EXEC sp_executesql @sql, N'@HasRelation BIT OUTPUT', @HasRelation OUTPUT;

            IF @HasRelation = 1
            BEGIN
                SET @OutputMessage = 'Cannot soft delete; found related records in ' + @Table;
                RETURN;
            END
        END
    END

    -- Handle last table
    SET @Table = LTRIM(RTRIM(@TempStr));
    IF @Table <> ''
    BEGIN
        SET @sql = '
            IF EXISTS (SELECT 1 FROM ' + QUOTENAME(@Table) + '
                       WHERE ForeignKeyId = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + ')
            BEGIN
                SET @HasRelation = 1
            END
        ';
        EXEC sp_executesql @sql, N'@HasRelation BIT OUTPUT', @HasRelation OUTPUT;

        IF @HasRelation = 1
        BEGIN
            SET @OutputMessage = 'Cannot soft delete; found related records in ' + @Table;
            RETURN;
        END
    END

    -- If no relations, do a soft delete
    SET @sql = '
        UPDATE ' + QUOTENAME(@BaseTableName) + '
        SET Active = 0
        WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
    EXEC(@sql);

    SET @OutputMessage = 'Record soft deleted successfully.';
END
GO










IF OBJECT_ID('dbo.sp_ForcePermanentDeleteWithReferences', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ForcePermanentDeleteWithReferences;
GO

CREATE PROCEDURE dbo.sp_ForcePermanentDeleteWithReferences
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @ReferenceTables NVARCHAR(MAX),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelation BIT = 0;
    DECLARE @Table NVARCHAR(100);
    DECLARE @Pos INT;
    DECLARE @Len INT;
    DECLARE @TempStr NVARCHAR(MAX);
    DECLARE @sql NVARCHAR(MAX);  -- one variable

    SET @TempStr = LTRIM(RTRIM(@ReferenceTables));
    SET @Pos = CHARINDEX(',', @TempStr);
    SET @Len = LEN(@TempStr);

    WHILE @Pos > 0
    BEGIN
        SET @Table = LTRIM(RTRIM(SUBSTRING(@TempStr, 1, @Pos - 1)));
        SET @TempStr = SUBSTRING(@TempStr, @Pos + 1, @Len);
        SET @TempStr = LTRIM(RTRIM(@TempStr));
        SET @Len = LEN(@TempStr);
        SET @Pos = CHARINDEX(',', @TempStr);

        IF @Table <> ''
        BEGIN
            SET @sql = '
                IF EXISTS (SELECT 1 FROM ' + QUOTENAME(@Table) + '
                           WHERE ForeignKeyId = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + ')
                BEGIN
                    SET @HasRelation = 1
                END
            ';
            EXEC sp_executesql @sql, N'@HasRelation BIT OUTPUT', @HasRelation OUTPUT;

            IF @HasRelation = 1
            BEGIN
                SET @OutputMessage = 'Cannot permanently delete; found related records in ' + @Table;
                RETURN;
            END
        END
    END

    -- Handle last table
    SET @Table = LTRIM(RTRIM(@TempStr));
    IF @Table <> ''
    BEGIN
        SET @sql = '
            IF EXISTS (SELECT 1 FROM ' + QUOTENAME(@Table) + '
                       WHERE ForeignKeyId = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + ')
            BEGIN
                SET @HasRelation = 1
            END
        ';
        EXEC sp_executesql @sql, N'@HasRelation BIT OUTPUT', @HasRelation OUTPUT;

        IF @HasRelation = 1
        BEGIN
            SET @OutputMessage = 'Cannot permanently delete; found related records in ' + @Table;
            RETURN;
        END
    END

    -- If no relations, do a permanent delete
    SET @sql = '
        DELETE FROM ' + QUOTENAME(@BaseTableName) + '
        WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
    EXEC(@sql);

    SET @OutputMessage = 'Record permanently deleted successfully.';
END
GO










DECLARE @sql NVARCHAR(MAX);

SET @sql = '...'; 
EXEC(@sql);
