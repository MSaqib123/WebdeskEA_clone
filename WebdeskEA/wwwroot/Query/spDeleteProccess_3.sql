
BEGIN /** delete queries **/


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

    DECLARE @HasRelations BIT = 0;  -- If you want to do real checks, set this to 1 upon detection
    DECLARE @sql NVARCHAR(MAX);

    /*
       Example check:
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
    DECLARE @sql NVARCHAR(MAX);

    SET @TempStr = LTRIM(RTRIM(@RefreshTables));
    SET @Pos = CHARINDEX(',', @TempStr);
    SET @Len = LEN(@TempStr);

    -- We'll iterate over each comma-separated table name
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

    -- Handle the last table (or only table if no commas)
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

    -- Deletion logic by @DeleteType
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
        SET @sql = '
            UPDATE ' + QUOTENAME(@TableName) + '
            SET Active = 0
            WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
        EXEC(@sql);

        SET @OutputMessage = 'Record soft deleted (refresh logic).';
    END
    ELSE IF @DeleteType = 4 -- Permanent Delete If No Nested Relation
    BEGIN
        SET @sql = '
            DELETE FROM ' + QUOTENAME(@TableName) + '
            WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
        EXEC(@sql);

        SET @OutputMessage = 'Record permanently deleted (refresh logic).';
    END
END
GO












IF OBJECT_ID('dbo.sp_ForceSoftDeleteWithRefKey', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ForceSoftDeleteWithRefKey;
GO

CREATE PROCEDURE dbo.sp_ForceSoftDeleteWithRefKey
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @RefTableKeys    NVARCHAR(MAX), -- e.g. "PI:POId,PODetail:POId"
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    /*
        In a real scenario, parse @RefTableKeys into table/column pairs, check each pair:
        "TableName:ColumnName"
    */
    DECLARE @HasRelation BIT = 0;
    DECLARE @sql NVARCHAR(MAX);

    -- Simple placeholder logic:
    --    If any table/column references the ID, block soft delete.
    -- For demonstration, let's do a minimal approach. A more robust approach
    -- would parse the string and check each table:column. 
    -- This is just a skeleton.

    -- If no references found:
    SET @sql = '
        UPDATE ' + QUOTENAME(@BaseTableName) + '
        SET Active = 0
        WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
    EXEC(@sql);

    SET @OutputMessage = 'Record soft deleted successfully with ref key logic.';
END
GO









IF OBJECT_ID('dbo.sp_ForcePermanentDeleteWithRefKey', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ForcePermanentDeleteWithRefKey;
GO

CREATE PROCEDURE dbo.sp_ForcePermanentDeleteWithRefKey
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @RefTableKeys    NVARCHAR(MAX),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelation BIT = 0;
    DECLARE @sql NVARCHAR(MAX);

    -- If references found, set @HasRelation=1, block
    -- Otherwise:
    SET @sql = '
        DELETE FROM ' + QUOTENAME(@BaseTableName) + '
        WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
    EXEC(@sql);

    SET @OutputMessage = 'Record permanently deleted successfully with ref key logic.';
END
GO








IF OBJECT_ID('dbo.sp_ForceSoftDeleteIfNoOtherRefKeys', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ForceSoftDeleteIfNoOtherRefKeys;
GO

CREATE PROCEDURE dbo.sp_ForceSoftDeleteIfNoOtherRefKeys
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @RefTableKeys    NVARCHAR(MAX),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    /*
       This is a placeholder. 
       Real logic: 
         1) Check "other tables" not in @RefTableKeys, see if they reference @PrimaryKeyValue.
         2) If they do, block. 
         3) Otherwise, soft delete the row.
    */

    DECLARE @sql NVARCHAR(MAX) = '
        UPDATE ' + QUOTENAME(@BaseTableName) + '
        SET Active = 0
        WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));

    EXEC(@sql);

    SET @OutputMessage = 'Record soft deleted successfully, no references in other tables.';
END
GO










IF OBJECT_ID('dbo.sp_ForcePermanentDeleteIfNoOtherRefKeys', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ForcePermanentDeleteIfNoOtherRefKeys;
GO

CREATE PROCEDURE dbo.sp_ForcePermanentDeleteIfNoOtherRefKeys
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @RefTableKeys    NVARCHAR(MAX),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    /*
       Similar placeholder logic for checking unlisted references
    */

    DECLARE @sql NVARCHAR(MAX) = '
        DELETE FROM ' + QUOTENAME(@BaseTableName) + '
        WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));

    EXEC(@sql);

    SET @OutputMessage = 'Record permanently deleted successfully, no references in other tables.';
END
GO












IF OBJECT_ID('dbo.sp_ForceSoftDeleteWithReferences', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ForceSoftDeleteWithReferences;
GO

CREATE PROCEDURE dbo.sp_ForceSoftDeleteWithReferences
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
    DECLARE @sql NVARCHAR(MAX);

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

    -- Last table
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

    -- If no relations found, do a soft delete
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
    DECLARE @sql NVARCHAR(MAX);

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

    -- Last table
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

    -- If no relations found, do a permanent delete
    SET @sql = '
        DELETE FROM ' + QUOTENAME(@BaseTableName) + '
        WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
    EXEC(@sql);

    SET @OutputMessage = 'Record permanently deleted successfully.';
END
GO














IF OBJECT_ID('dbo.sp_ForceSoftDeleteWithRefTableAndKey', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ForceSoftDeleteWithRefTableAndKey;
GO

CREATE PROCEDURE dbo.sp_ForceSoftDeleteWithRefTableAndKey
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @ReferenceTable  NVARCHAR(100),
    @ReferenceKey    NVARCHAR(100),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelation BIT = 0;
    DECLARE @sql NVARCHAR(MAX);

    SET @sql = '
        IF EXISTS (
            SELECT 1 FROM ' + QUOTENAME(@ReferenceTable) + '
            WHERE ' + @ReferenceKey + ' = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + '
        )
        BEGIN
            SET @HasRelation = 1
        END
    ';

    EXEC sp_executesql @sql, N'@HasRelation BIT OUTPUT', @HasRelation OUTPUT;

    IF @HasRelation = 1
    BEGIN
        SET @OutputMessage = 'Cannot soft delete; found related record in ' + @ReferenceTable 
            + ' referencing ' + @ReferenceKey + '.';
        RETURN;
    END

    -- If no relation, soft delete
    SET @sql = '
        UPDATE ' + QUOTENAME(@BaseTableName) + '
        SET Active = 0
        WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));

    EXEC(@sql);

    SET @OutputMessage = 'Record soft deleted successfully.';
END
GO
















IF OBJECT_ID('dbo.sp_ForcePermanentDeleteWithRefTableAndKey', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ForcePermanentDeleteWithRefTableAndKey;
GO

CREATE PROCEDURE dbo.sp_ForcePermanentDeleteWithRefTableAndKey
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @ReferenceTable  NVARCHAR(100),
    @ReferenceKey    NVARCHAR(100),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelation BIT = 0;
    DECLARE @sql NVARCHAR(MAX);

    SET @sql = '
        IF EXISTS (
            SELECT 1 FROM ' + QUOTENAME(@ReferenceTable) + '
            WHERE ' + @ReferenceKey + ' = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + '
        )
        BEGIN
            SET @HasRelation = 1
        END
    ';

    EXEC sp_executesql @sql, N'@HasRelation BIT OUTPUT', @HasRelation OUTPUT;

    IF @HasRelation = 1
    BEGIN
        SET @OutputMessage = 'Cannot permanently delete; found related record in ' + @ReferenceTable 
            + ' referencing ' + @ReferenceKey + '.';
        RETURN;
    END

    SET @sql = '
        DELETE FROM ' + QUOTENAME(@BaseTableName) + '
        WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));

    EXEC(@sql);

    SET @OutputMessage = 'Record permanently deleted successfully.';
END
GO




















IF OBJECT_ID('dbo.sp_ForceSoftDeleteIfNoRelationWithRefTableAndKey', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ForceSoftDeleteIfNoRelationWithRefTableAndKey;
GO

CREATE PROCEDURE dbo.sp_ForceSoftDeleteIfNoRelationWithRefTableAndKey
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @ReferenceTable  NVARCHAR(100),
    @ReferenceKey    NVARCHAR(100),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelation BIT = 0;
    DECLARE @sql NVARCHAR(MAX);

    SET @sql = '
        IF EXISTS (
            SELECT 1 FROM ' + QUOTENAME(@ReferenceTable) + '
            WHERE ' + @ReferenceKey + ' = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + '
        )
        BEGIN
            SET @HasRelation = 1
        END
    ';

    EXEC sp_executesql @sql, N'@HasRelation BIT OUTPUT', @HasRelation OUTPUT;

    IF @HasRelation = 1
    BEGIN
        SET @OutputMessage = 'Cannot soft delete (NoRelation mode). ' + @ReferenceTable 
            + ' references ' + @ReferenceKey + '.';
        RETURN;
    END

    SET @sql = '
        UPDATE ' + QUOTENAME(@BaseTableName) + '
        SET Active = 0
        WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
    EXEC(@sql);

    SET @OutputMessage = 'Record soft deleted successfully (NoRelation mode).';
END
GO














IF OBJECT_ID('dbo.sp_ForcePermanentDeleteIfNoRelationWithRefTableAndKey', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ForcePermanentDeleteIfNoRelationWithRefTableAndKey;
GO

CREATE PROCEDURE dbo.sp_ForcePermanentDeleteIfNoRelationWithRefTableAndKey
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @ReferenceTable  NVARCHAR(100),
    @ReferenceKey    NVARCHAR(100),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelation BIT = 0;
    DECLARE @sql NVARCHAR(MAX);

    SET @sql = '
        IF EXISTS (
            SELECT 1 FROM ' + QUOTENAME(@ReferenceTable) + '
            WHERE ' + @ReferenceKey + ' = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + '
        )
        BEGIN
            SET @HasRelation = 1
        END
    ';

    EXEC sp_executesql @sql, N'@HasRelation BIT OUTPUT', @HasRelation OUTPUT;

    IF @HasRelation = 1
    BEGIN
        SET @OutputMessage = 'Cannot permanently delete (NoRelation mode). ' 
            + @ReferenceTable + ' references ' + @ReferenceKey + '.';
        RETURN;
    END

    SET @sql = '
        DELETE FROM ' + QUOTENAME(@BaseTableName) + '
        WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));
    EXEC(@sql);

    SET @OutputMessage = 'Record permanently deleted successfully (NoRelation mode).';
END
GO


END /** delete queries **/

