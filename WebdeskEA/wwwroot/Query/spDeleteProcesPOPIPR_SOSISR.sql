--===========================================
--============ Best Delete Solution =========
--===========================================
IF OBJECT_ID('dbo.sp_ForcePermanentDeleteWithRefKey_TwoList', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ForcePermanentDeleteWithRefKey_TwoList;
GO

CREATE PROCEDURE dbo.sp_ForcePermanentDeleteWithRefKey_TwoList
    @BaseTableName   NVARCHAR(100),
    @PrimaryKeyValue INT,
    @BlockRefKeys    NVARCHAR(MAX),  -- e.g. "PIs:POId,SomethingElse:POId"
    @ForceRefKeys    NVARCHAR(MAX),  -- e.g. "PODtls:POId,SomeOtherDtl:POId"
    @OutputMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasBlock BIT = 0;
    DECLARE @Pair NVARCHAR(200);        -- e.g. "PIs:POId"
    DECLARE @Pos INT;
    DECLARE @Len INT;
    DECLARE @TempStr NVARCHAR(MAX);
    DECLARE @tbl NVARCHAR(100);
    DECLARE @col NVARCHAR(100);
    DECLARE @sql NVARCHAR(MAX);

    --------------------------------------------------------------------------------
    -- 1) BLOCK References: If any exist, we abort the delete
    --------------------------------------------------------------------------------
    SET @TempStr = LTRIM(RTRIM(@BlockRefKeys));
    SET @Pos = CHARINDEX(',', @TempStr);
    SET @Len = LEN(@TempStr);

    WHILE @Pos > 0
    BEGIN
        SET @Pair = LTRIM(RTRIM(SUBSTRING(@TempStr, 1, @Pos - 1)));
        SET @TempStr = SUBSTRING(@TempStr, @Pos + 1, @Len);
        SET @TempStr = LTRIM(RTRIM(@TempStr));
        SET @Len = LEN(@TempStr);
        SET @Pos = CHARINDEX(',', @TempStr);

        IF @Pair <> ''
        BEGIN
            -- Parse "TableName:ColumnName"
            EXEC dbo.ParseTableKeyPair @Pair, @tbl OUTPUT, @col OUTPUT;

            IF @tbl <> '' AND @col <> ''
            BEGIN
                SET @sql = '
                    IF EXISTS (SELECT 1 FROM ' + QUOTENAME(@tbl) + '
                               WHERE ' + QUOTENAME(@col) + ' = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + ')
                    BEGIN
                        SET @HasBlock = 1
                    END
                ';
                EXEC sp_executesql @sql, N'@HasBlock BIT OUTPUT', @HasBlock OUTPUT;

                IF @HasBlock = 1
                BEGIN
                    SET @OutputMessage = 'Cannot delete; found referencing rows in ' + @tbl 
                        + ' (' + @col + ') which blocks this delete.';
                    RETURN;
                END
            END
        END
    END

    -- Handle last pair in block list
    SET @Pair = LTRIM(RTRIM(@TempStr));
    IF @Pair <> ''
    BEGIN
        EXEC dbo.ParseTableKeyPair @Pair, @tbl OUTPUT, @col OUTPUT;

        IF @tbl <> '' AND @col <> ''
        BEGIN
            SET @sql = '
                IF EXISTS (SELECT 1 FROM ' + QUOTENAME(@tbl) + '
                           WHERE ' + QUOTENAME(@col) + ' = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + ')
                BEGIN
                    SET @HasBlock = 1
                END
            ';
            EXEC sp_executesql @sql, N'@HasBlock BIT OUTPUT', @HasBlock OUTPUT;

            IF @HasBlock = 1
            BEGIN
                SET @OutputMessage = 'Cannot delete; found referencing rows in ' + @tbl 
                    + ' (' + @col + ') which blocks this delete.';
                RETURN;
            END
        END
    END

    --------------------------------------------------------------------------------
    -- 2) FORCE References: If records exist, we delete them FIRST
    --------------------------------------------------------------------------------
    SET @TempStr = LTRIM(RTRIM(@ForceRefKeys));
    SET @Pos = CHARINDEX(',', @TempStr);
    SET @Len = LEN(@TempStr);

    WHILE @Pos > 0
    BEGIN
        SET @Pair = LTRIM(RTRIM(SUBSTRING(@TempStr, 1, @Pos - 1)));
        SET @TempStr = SUBSTRING(@TempStr, @Pos + 1, @Len);
        SET @TempStr = LTRIM(RTRIM(@TempStr));
        SET @Len = LEN(@TempStr);
        SET @Pos = CHARINDEX(',', @TempStr);

        IF @Pair <> ''
        BEGIN
            EXEC dbo.ParseTableKeyPair @Pair, @tbl OUTPUT, @col OUTPUT;

            IF @tbl <> '' AND @col <> ''
            BEGIN
                -- Force-delete from the child table
                SET @sql = '
                    DELETE FROM ' + QUOTENAME(@tbl) + '
                    WHERE ' + QUOTENAME(@col) + ' = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + ';
                ';
                EXEC (@sql);
            END
        END
    END

    -- Handle last pair in force list
    SET @Pair = LTRIM(RTRIM(@TempStr));
    IF @Pair <> ''
    BEGIN
        EXEC dbo.ParseTableKeyPair @Pair, @tbl OUTPUT, @col OUTPUT;

        IF @tbl <> '' AND @col <> ''
        BEGIN
            SET @sql = '
                DELETE FROM ' + QUOTENAME(@tbl) + '
                WHERE ' + QUOTENAME(@col) + ' = ' + CAST(@PrimaryKeyValue AS VARCHAR(20)) + ';
            ';
            EXEC (@sql);
        END
    END

    --------------------------------------------------------------------------------
    -- 3) Now safe to delete the base row
    --------------------------------------------------------------------------------
    SET @sql = '
        DELETE FROM ' + QUOTENAME(@BaseTableName) + '
        WHERE Id = ' + CAST(@PrimaryKeyValue AS VARCHAR(20));

    EXEC(@sql);

    SET @OutputMessage = 'Record permanently deleted successfully (block keys not found, force keys deleted).';
END
GO


IF OBJECT_ID('dbo.ParseTableKeyPair', 'P') IS NOT NULL
    DROP PROCEDURE dbo.ParseTableKeyPair;
GO

CREATE PROCEDURE dbo.ParseTableKeyPair
    @Pair NVARCHAR(200),      -- e.g. "PODtls:POId"
    @OutTable NVARCHAR(100) OUTPUT,
    @OutColumn NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- We find the ":" position
    DECLARE @pos INT = CHARINDEX(':', @Pair);
    IF @pos > 0
    BEGIN
        SET @OutTable = LTRIM(RTRIM(SUBSTRING(@Pair, 1, @pos - 1)));
        SET @OutColumn = LTRIM(RTRIM(SUBSTRING(@Pair, @pos + 1, LEN(@Pair))));
    END
    ELSE
    BEGIN
        -- If no colon, fallback or set empty
        SET @OutTable = LTRIM(RTRIM(@Pair));
        SET @OutColumn = '';
    END
END
GO
