CREATE PROCEDURE sp_DeleteEntityWithValidation
    @TableName       NVARCHAR(100),
    @PrimaryKeyValue INT,
    @DeleteType      INT,
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Example: If you want to detect if there's a known related table,
    -- set @HasRelations to 1 and return the appropriate message.
    DECLARE @HasRelations BIT = 0;

    -- (Customize your actual relationship checks here)
    -- e.g., if you have a specific table or foreign key. This is just a placeholder.
    -- IF EXISTS(SELECT 1 FROM SomeRelatedTable WHERE ForeignKeyId = @PrimaryKeyValue)
    -- BEGIN
    --     SET @HasRelations = 1;
    -- END

    IF @DeleteType = 1 -- Soft Delete
    BEGIN
        UPDATE [dbo].[@TableName]
        SET Active = 0
        WHERE Id = @PrimaryKeyValue;  -- or dynamic SQL if needed

        SET @OutputMessage = 'Record soft deleted successfully.';
    END
    ELSE IF @DeleteType = 2 -- Permanent Delete
    BEGIN
        DELETE FROM [dbo].[@TableName]
        WHERE Id = @PrimaryKeyValue;

        SET @OutputMessage = 'Record permanently deleted successfully.';
    END
    ELSE IF @DeleteType = 3 -- Soft Delete If No Nested Relation
    BEGIN
        IF @HasRelations = 1
        BEGIN
            SET @OutputMessage = 'Cannot soft delete because related records exist.';
            RETURN;
        END

        UPDATE [dbo].[@TableName]
        SET Active = 0
        WHERE Id = @PrimaryKeyValue;

        SET @OutputMessage = 'Record soft deleted successfully (no relations).';
    END
    ELSE IF @DeleteType = 4 -- Permanent Delete If No Nested Relation
    BEGIN
        IF @HasRelations = 1
        BEGIN
            SET @OutputMessage = 'Cannot permanently delete because related records exist.';
            RETURN;
        END

        DELETE FROM [dbo].[@TableName]
        WHERE Id = @PrimaryKeyValue;

        SET @OutputMessage = 'Record permanently deleted successfully (no relations).';
    END
END
GO









CREATE PROCEDURE sp_DeleteWithRelations
    @TableName        NVARCHAR(100),
    @PrimaryKeyValue  INT,
    @DeleteType       INT,
    @RefreshTables    NVARCHAR(MAX),
    @OutputMessage    NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- 1) Parse @RefreshTables into a temp table (comma-separated)
    CREATE TABLE #RefreshTables (TableName NVARCHAR(100));

    INSERT INTO #RefreshTables (TableName)
    SELECT TRIM([value]) 
    FROM STRING_SPLIT(@RefreshTables, ',');

    DECLARE @HasRelations BIT = 0;
    DECLARE @RelatedTable NVARCHAR(100);

    DECLARE cur CURSOR FOR
        SELECT TableName FROM #RefreshTables;

    OPEN cur;
    FETCH NEXT FROM cur INTO @RelatedTable;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Example check: "ForeignKeyId"
        DECLARE @sql NVARCHAR(MAX) = '
            IF EXISTS (SELECT 1 FROM ' + @RelatedTable + ' WHERE ForeignKeyId = ' 
                      + CAST(@PrimaryKeyValue AS NVARCHAR) + ')
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

        FETCH NEXT FROM cur INTO @RelatedTable;
    END

    CLOSE cur;
    DEALLOCATE cur;

    -- 2) Deletion logic based on DeleteType
    IF @DeleteType = 1 -- Soft Delete
    BEGIN
        EXEC('UPDATE ' + @TableName + ' SET Active = 0 WHERE Id = ' + CAST(@PrimaryKeyValue AS NVARCHAR));
        SET @OutputMessage = 'Record soft deleted successfully.';
    END
    ELSE IF @DeleteType = 2 -- Permanent Delete
    BEGIN
        EXEC('DELETE FROM ' + @TableName + ' WHERE Id = ' + CAST(@PrimaryKeyValue AS NVARCHAR));
        SET @OutputMessage = 'Record permanently deleted successfully.';
    END
    ELSE IF @DeleteType = 3 -- Soft Delete If No Nested Relation
    BEGIN
        EXEC('UPDATE ' + @TableName + ' SET Active = 0 WHERE Id = ' + CAST(@PrimaryKeyValue AS NVARCHAR));
        SET @OutputMessage = 'Record soft deleted (refresh logic).';
    END
    ELSE IF @DeleteType = 4 -- Permanent Delete If No Nested Relation
    BEGIN
        EXEC('DELETE FROM ' + @TableName + ' WHERE Id = ' + CAST(@PrimaryKeyValue AS NVARCHAR));
        SET @OutputMessage = 'Record permanently deleted (refresh logic).';
    END

    DROP TABLE #RefreshTables;
END
GO









CREATE PROCEDURE sp_ForceSoftDeleteWithReferences
    @BaseTableName NVARCHAR(100),
    @PrimaryKeyValue INT,
    @ReferenceTables NVARCHAR(MAX),  -- e.g., "PODetail,PI"
    @OutputMessage NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #ReferenceTables(TableName NVARCHAR(100));
    INSERT INTO #ReferenceTables(TableName)
    SELECT TRIM([value]) FROM STRING_SPLIT(@ReferenceTables, ',');

    DECLARE @HasRelation BIT = 0;
    DECLARE @Table NVARCHAR(100);

    DECLARE refCur CURSOR FOR SELECT TableName FROM #ReferenceTables;
    OPEN refCur;
    FETCH NEXT FROM refCur INTO @Table;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Example check: "ForeignKeyId"
        DECLARE @sql NVARCHAR(MAX) = '
            IF EXISTS (SELECT 1 FROM ' + @Table + ' WHERE ForeignKeyId = '
            + CAST(@PrimaryKeyValue AS NVARCHAR) + ')
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

        FETCH NEXT FROM refCur INTO @Table;
    END

    CLOSE refCur;
    DEALLOCATE refCur;

    -- If no relations, do a soft delete
    SET @sql = 'UPDATE ' + @BaseTableName + ' SET Active = 0 WHERE Id = '
               + CAST(@PrimaryKeyValue AS NVARCHAR);
    EXEC(@sql);

    SET @OutputMessage = 'Record soft deleted successfully.';
END
GO













CREATE PROCEDURE sp_ForcePermanentDeleteWithReferences
    @BaseTableName NVARCHAR(100),
    @PrimaryKeyValue INT,
    @ReferenceTables NVARCHAR(MAX),
    @OutputMessage NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #ReferenceTables(TableName NVARCHAR(100));
    INSERT INTO #ReferenceTables(TableName)
    SELECT TRIM([value]) FROM STRING_SPLIT(@ReferenceTables, ',');

    DECLARE @HasRelation BIT = 0;
    DECLARE @Table NVARCHAR(100);

    DECLARE refCur CURSOR FOR SELECT TableName FROM #ReferenceTables;
    OPEN refCur;
    FETCH NEXT FROM refCur INTO @Table;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        DECLARE @sql NVARCHAR(MAX) = '
            IF EXISTS (SELECT 1 FROM ' + @Table + ' WHERE ForeignKeyId = '
            + CAST(@PrimaryKeyValue AS NVARCHAR) + ')
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

        FETCH NEXT FROM refCur INTO @Table;
    END

    CLOSE refCur;
    DEALLOCATE refCur;

    -- If no relations, do a permanent delete
    SET @sql = 'DELETE FROM ' + @BaseTableName + ' WHERE Id = '
               + CAST(@PrimaryKeyValue AS NVARCHAR);
    EXEC(@sql);

    SET @OutputMessage = 'Record permanently deleted successfully.';
END
GO











CREATE PROCEDURE sp_ForcePermanentDeleteIfNoOtherRefKeys
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @RefTableKeys    NVARCHAR(MAX),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Similar steps: parse @RefTableKeys, gather #AllUserTables except those, check references...
    -- If references found, return an error message.
    -- Otherwise do a permanent delete.

    DECLARE @DeleteSql NVARCHAR(MAX) = '
        DELETE FROM ' + @BaseTableName + '
        WHERE Id = ' + CAST(@PrimaryKeyValue AS NVARCHAR);

    EXEC(@DeleteSql);

    SET @OutputMessage = 'Record permanently deleted successfully, and no references found in other tables.';
END

GO














CREATE PROCEDURE sp_ForceSoftDeleteWithRefTableAndKey
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @ReferenceTable  NVARCHAR(100),
    @ReferenceKey    NVARCHAR(100),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelation BIT = 0;

    DECLARE @sql NVARCHAR(MAX) = '
        IF EXISTS (
            SELECT 1 FROM ' + @ReferenceTable + '
            WHERE ' + @ReferenceKey + ' = ' + CAST(@PrimaryKeyValue AS NVARCHAR) + '
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

    SET @sql = 'UPDATE ' + @BaseTableName + ' SET Active = 0 WHERE Id = '
               + CAST(@PrimaryKeyValue AS NVARCHAR);
    EXEC(@sql);

    SET @OutputMessage = 'Record soft deleted successfully.';
END
GO















CREATE PROCEDURE sp_ForcePermanentDeleteWithRefTableAndKey
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @ReferenceTable  NVARCHAR(100),
    @ReferenceKey    NVARCHAR(100),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelation BIT = 0;

    DECLARE @sql NVARCHAR(MAX) = '
        IF EXISTS (
            SELECT 1 FROM ' + @ReferenceTable + '
            WHERE ' + @ReferenceKey + ' = ' + CAST(@PrimaryKeyValue AS NVARCHAR) + '
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

    SET @sql = 'DELETE FROM ' + @BaseTableName + ' WHERE Id = '
               + CAST(@PrimaryKeyValue AS NVARCHAR);
    EXEC(@sql);

    SET @OutputMessage = 'Record permanently deleted successfully.';
END
GO











CREATE PROCEDURE sp_ForceSoftDeleteIfNoRelationWithRefTableAndKey
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @ReferenceTable  NVARCHAR(100),
    @ReferenceKey    NVARCHAR(100),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelation BIT = 0;

    DECLARE @sql NVARCHAR(MAX) = '
        IF EXISTS (
            SELECT 1 FROM ' + @ReferenceTable + '
            WHERE ' + @ReferenceKey + ' = ' + CAST(@PrimaryKeyValue AS NVARCHAR) + '
        )
        BEGIN
            SET @HasRelation = 1
        END
    ';

    EXEC sp_executesql @sql, N'@HasRelation BIT OUTPUT', @HasRelation OUTPUT;

    IF @HasRelation = 1
    BEGIN
        SET @OutputMessage = 'Cannot soft delete (NoRelation mode) because ' + @ReferenceTable
                             + ' references ' + @ReferenceKey + '.';
        RETURN;
    END

    -- Additional "no-other-relations" checks can go here if needed

    SET @sql = 'UPDATE ' + @BaseTableName + ' SET Active = 0 WHERE Id = '
               + CAST(@PrimaryKeyValue AS NVARCHAR);
    EXEC(@sql);

    SET @OutputMessage = 'Record soft deleted successfully (NoRelation mode).';
END
GO












CREATE PROCEDURE sp_ForcePermanentDeleteIfNoRelationWithRefTableAndKey
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @ReferenceTable  NVARCHAR(100),
    @ReferenceKey    NVARCHAR(100),
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasRelation BIT = 0;

    DECLARE @sql NVARCHAR(MAX) = '
        IF EXISTS (
            SELECT 1 FROM ' + @ReferenceTable + '
            WHERE ' + @ReferenceKey + ' = ' + CAST(@PrimaryKeyValue AS NVARCHAR) + '
        )
        BEGIN
            SET @HasRelation = 1
        END
    ';

    EXEC sp_executesql @sql, N'@HasRelation BIT OUTPUT', @HasRelation OUTPUT;

    IF @HasRelation = 1
    BEGIN
        SET @OutputMessage = 'Cannot permanently delete (NoRelation mode) because ' 
                             + @ReferenceTable + ' references ' + @ReferenceKey + '.';
        RETURN;
    END

    -- Additional "no-other-relations" checks can go here if needed

    SET @sql = 'DELETE FROM ' + @BaseTableName + ' WHERE Id = '
               + CAST(@PrimaryKeyValue AS NVARCHAR);
    EXEC(@sql);

    SET @OutputMessage = 'Record permanently deleted successfully (NoRelation mode).';
END
GO
