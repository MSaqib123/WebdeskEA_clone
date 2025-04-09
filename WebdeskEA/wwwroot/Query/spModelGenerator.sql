-- ============================================================================================================
--=================================== Model Generation ========================================================
-- ============================================================================================================
-- Author:      Your Name
-- Create date: 2025-01-11
-- Description: Universal Model Generator (C#, Java, JS, Flutter)
-- exec AdvancedModelGenerator  'banks',2

IF OBJECT_ID('dbo.AdvancedModelGenerator', 'P') IS NOT NULL
    DROP PROCEDURE dbo.AdvancedModelGenerator;
GO

CREATE PROCEDURE dbo.AdvancedModelGenerator
(
    @TableName     NVARCHAR(128),  -- The table name to generate
    @GenerateDto   BIT = 0,        -- 0 = Only Model, 1 = Also generate "Dto"
    @LanguageParam NVARCHAR(10) = N'C#' 
    -- Supported values:
    --   'C#'       => Full C# code with data annotations
    --   'Java'     => Basic Java class
    --   'Js'       => Basic ES6 JavaScript class
    --   'Flutter'  => Basic Dart class for Flutter
)
AS
BEGIN
    SET NOCOUNT ON;

    ------------------------------------------------------------------------------
    -- 0. Validate table name
    ------------------------------------------------------------------------------
    DECLARE @ActualTableName NVARCHAR(128);

    SELECT @ActualTableName = t.TABLE_NAME
    FROM INFORMATION_SCHEMA.TABLES t
    WHERE t.TABLE_TYPE = 'BASE TABLE'
      AND t.TABLE_NAME COLLATE SQL_Latin1_General_CP1_CI_AS = @TableName;

    IF @ActualTableName IS NULL
    BEGIN
        RAISERROR('====== Table not found: %s ======', 16, 1, @TableName);
        RETURN;
    END;

    ------------------------------------------------------------------------------
    -- 1. Determine if table has auditing columns => Inherit from BaseEntity in C#
    ------------------------------------------------------------------------------
    DECLARE @InheritBaseEntity BIT = 0;
    DECLARE @HasActive BIT = 0;

    IF EXISTS 
    (
        SELECT 1 
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = @ActualTableName
          AND COLUMN_NAME IN ('Active','CreatedBy','CreatedOn','ModifiedBy','ModifiedOn')
    )
    BEGIN
        SET @InheritBaseEntity = 1;
    END;

    ------------------------------------------------------------------------------
    -- 2. Gather column info
    ------------------------------------------------------------------------------
    IF OBJECT_ID('tempdb..#Tmp_Columns') IS NOT NULL
        DROP TABLE #Tmp_Columns;

    SELECT 
        c.COLUMN_NAME,
        c.DATA_TYPE,
        CAST(c.CHARACTER_MAXIMUM_LENGTH AS INT) AS [MaxLength],
        CASE WHEN c.IS_NULLABLE = 'YES' THEN 1 ELSE 0 END AS IsNullable,
        c.ORDINAL_POSITION
    INTO #Tmp_Columns
    FROM INFORMATION_SCHEMA.COLUMNS c
    WHERE c.TABLE_NAME = @ActualTableName
    ORDER BY c.ORDINAL_POSITION;

    ------------------------------------------------------------------------------
    -- 3. Map SQL data types to "universal" types (we'll branch per language)
    ------------------------------------------------------------------------------
    IF OBJECT_ID('tempdb..#TypeMapping') IS NOT NULL
        DROP TABLE #TypeMapping;

    CREATE TABLE #TypeMapping
    (
        SqlType      VARCHAR(50),
        CSharpType   VARCHAR(50),
        JavaType     VARCHAR(50),
        JsType       VARCHAR(50),
        DartType     VARCHAR(50)
    );

    INSERT INTO #TypeMapping
    VALUES
    --  SqlType         CSharpType   JavaType     JsType      DartType
    ('bigint',         'long',       'long',      'number',   'int'),
    ('binary',         'byte[]',     'byte[]',    'Array',    'List<int>'),
    ('bit',            'bool',       'boolean',   'boolean',  'bool'),
    ('char',           'string',     'String',    'string',   'String'),
    ('date',           'DateTime',   'Date',      'Date',     'DateTime'),
    ('datetime',       'DateTime',   'Date',      'Date',     'DateTime'),
    ('datetime2',      'DateTime',   'Date',      'Date',     'DateTime'),
    ('datetimeoffset', 'DateTimeOffset','Date',   'Date',     'DateTime'),
    ('decimal',        'decimal',    'BigDecimal','number',   'double'),
    ('float',          'double',     'double',    'number',   'double'),
    ('image',          'byte[]',     'byte[]',    'Array',    'List<int>'),
    ('int',            'int',        'int',       'number',   'int'),
    ('money',          'decimal',    'BigDecimal','number',   'double'),
    ('nchar',          'string',     'String',    'string',   'String'),
    ('ntext',          'string',     'String',    'string',   'String'),
    ('numeric',        'decimal',    'BigDecimal','number',   'double'),
    ('nvarchar',       'string',     'String',    'string',   'String'),
    ('real',           'float',      'float',     'number',   'double'),
    ('smalldatetime',  'DateTime',   'Date',      'Date',     'DateTime'),
    ('smallint',       'short',      'short',     'number',   'int'),
    ('smallmoney',     'decimal',    'BigDecimal','number',   'double'),
    ('text',           'string',     'String',    'string',   'String'),
    ('time',           'TimeSpan',   'String',    'string',   'String'),
    ('timestamp',      'byte[]',     'byte[]',    'Array',    'List<int>'),
    ('tinyint',        'byte',       'byte',      'number',   'int'),
    ('uniqueidentifier','Guid',      'String',    'string',   'String'),
    ('varbinary',      'byte[]',     'byte[]',    'Array',    'List<int>'),
    ('varchar',        'string',     'String',    'string',   'String');

    ------------------------------------------------------------------------------
    -- 4. Identify PK columns (for annotation logic)
    ------------------------------------------------------------------------------
    IF OBJECT_ID('tempdb..#CS_Columns') IS NOT NULL
        DROP TABLE #CS_Columns;

    CREATE TABLE #CS_Columns
    (
        ColumnName    NVARCHAR(128),
        SqlType       VARCHAR(50),
        CSharpType    VARCHAR(50),
        JavaType      VARCHAR(50),
        JsType        VARCHAR(50),
        DartType      VARCHAR(50),
        IsNullable    BIT,
        MaxLength     INT,
        IsPrimaryKey  BIT DEFAULT(0),
        IsIdentity    BIT DEFAULT(0)
    );

    INSERT INTO #CS_Columns (ColumnName, SqlType, CSharpType, JavaType, JsType, DartType, IsNullable, MaxLength)
    SELECT 
        t.COLUMN_NAME,
        t.DATA_TYPE,
        ISNULL(m.CSharpType, 'object') AS CSharpType,
        ISNULL(m.JavaType,   'String') AS JavaType,
        ISNULL(m.JsType,     'any')    AS JsType,
        ISNULL(m.DartType,   'var')    AS DartType,
        t.IsNullable,
        t.MaxLength
    FROM #Tmp_Columns t
    LEFT JOIN #TypeMapping m ON t.DATA_TYPE = m.SqlType;

    -- Mark PK columns
    ;WITH PKCols AS
    (
        SELECT kcu.COLUMN_NAME
        FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
        INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
            ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
        WHERE tc.TABLE_NAME = @ActualTableName
          AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
    )
    UPDATE #CS_Columns
    SET IsPrimaryKey = 1
    WHERE ColumnName IN (SELECT COLUMN_NAME FROM PKCols);

    -- Mark identity columns
    ;WITH IdentityCols AS
    (
        SELECT name
        FROM sys.identity_columns
        WHERE object_id = OBJECT_ID(@ActualTableName)
    )
    UPDATE #CS_Columns
    SET IsIdentity = 1
    WHERE ColumnName IN (SELECT name FROM IdentityCols);

    ------------------------------------------------------------------------------
    -- 5. (Optional) Identify foreign-key relationships for C# only
    ------------------------------------------------------------------------------
    IF OBJECT_ID('tempdb..#FK_Relationships') IS NOT NULL
        DROP TABLE #FK_Relationships;

    CREATE TABLE #FK_Relationships
    (
        PKTable        SYSNAME,
        PKColumn       SYSNAME,
        FKTable        SYSNAME,
        FKColumn       SYSNAME
    );

    INSERT INTO #FK_Relationships
    SELECT
        pk_tab.name AS PKTable,
        pk_col.name AS PKColumn,
        fk_tab.name AS FKTable,
        fk_col.name AS FKColumn
    FROM sys.foreign_key_columns fkc
    INNER JOIN sys.tables pk_tab ON pk_tab.object_id = fkc.referenced_object_id
    INNER JOIN sys.columns pk_col ON pk_col.object_id = pk_tab.object_id
                                 AND pk_col.column_id = fkc.referenced_column_id
    INNER JOIN sys.tables fk_tab ON fk_tab.object_id = fkc.parent_object_id
    INNER JOIN sys.columns fk_col ON fk_col.object_id = fk_tab.object_id
                                 AND fk_col.column_id = fkc.parent_column_id;

    -- For "joined columns" in C#
    IF OBJECT_ID('tempdb..#RefBy') IS NOT NULL
        DROP TABLE #RefBy;

    SELECT
        FKTable,
        FKColumn,
        PKTable,
        PKColumn
    INTO #RefBy
    FROM #FK_Relationships
    WHERE PKTable = @ActualTableName;

    IF OBJECT_ID('tempdb..#RefTo') IS NOT NULL
        DROP TABLE #RefTo;

    SELECT
        FKTable,
        FKColumn,
        PKTable,
        PKColumn
    INTO #RefTo
    FROM #FK_Relationships
    WHERE FKTable = @ActualTableName;

    ------------------------------------------------------------------------------
    -- 6. Decide Class Names
    ------------------------------------------------------------------------------
    DECLARE @ClassName  SYSNAME = @ActualTableName;
    DECLARE @DtoName    SYSNAME = @ActualTableName + 'Dto';

    ------------------------------------------------------------------------------
    -- 7. We'll build final code in an NVARCHAR(MAX) variable, then print
    ------------------------------------------------------------------------------
    DECLARE @CodeOutput NVARCHAR(MAX) = N'';

    ------------------------------------------------------------------------------
    -- 8. Generate the Model code for the chosen language
    ------------------------------------------------------------------------------
    IF @LanguageParam = 'C#'
    BEGIN
        --------------------------------------------------------------------------
        -- 8a. Generate C# Model
        --     If we detect those audit columns (Active, CreatedBy, etc.) => Inherit
        --     from BaseEntity => exclude them from property generation
        --------------------------------------------------------------------------
        DECLARE @CSharpModel NVARCHAR(MAX) = N'';

        SET @CSharpModel = 
            N'public class ' + @ClassName
            + CASE WHEN @InheritBaseEntity = 1 THEN ' : BaseEntity' ELSE '' END
            + CHAR(13) + CHAR(10) + N'{' + CHAR(13) + CHAR(10);

        SET @CSharpModel += CHAR(9) + '//__________ Main Columns __________' + CHAR(13) + CHAR(10);
        SET @CSharpModel += CHAR(9) + '#region Main_Columns' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

        -- Exclude auditing columns from property generation if inheriting
        -- i.e. do NOT generate if ColumnName in ('Active','CreatedBy','CreatedOn','ModifiedBy','ModifiedOn')
        SELECT @CSharpModel += 
            CASE 
                WHEN IsPrimaryKey=0 
                     AND IsNullable=0 
                     AND CSharpType='string' 
                     AND MaxLength>0 
                     AND MaxLength<8000
                     AND ColumnName NOT IN ('Active','CreatedBy','CreatedOn','ModifiedBy','ModifiedOn')
                    THEN CHAR(9) + '[StringLength(' + CAST(MaxLength AS VARCHAR(10)) + ')]' 
                         + CHAR(13)+CHAR(10)
                ELSE ''
            END
            + CASE 
                WHEN IsPrimaryKey=0 
                     AND IsNullable=0 
                     AND CSharpType NOT IN ('string','byte[]','object') 
                     AND ColumnName NOT IN ('Active','CreatedBy','CreatedOn','ModifiedBy','ModifiedOn')
                    THEN CHAR(9) + '[Required]' + CHAR(13)+CHAR(10)
                ELSE ''
            END
            + CASE 
                WHEN ColumnName NOT IN ('Active','CreatedBy','CreatedOn','ModifiedBy','ModifiedOn')
                THEN CHAR(9) + 'public '
                     + CASE WHEN IsNullable=1 
                                AND CSharpType NOT IN ('string','byte[]','object') 
                            THEN CSharpType + '?' 
                            ELSE CSharpType END
                     + ' ' + ColumnName + ' { get; set; }' + CHAR(13) + CHAR(10)
                ELSE ''
            END
        FROM #CS_Columns
        ORDER BY ColumnName;

        SET @CSharpModel += CHAR(13) + CHAR(10) + CHAR(9) + '#endregion' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

        --------------------------------------------------------------------------
        -- (Optional) Joined Columns if referencing other tables => Single
        --------------------------------------------------------------------------
        IF EXISTS(SELECT 1 FROM #RefTo)
        BEGIN
            SET @CSharpModel += CHAR(9) + '//------ Joined Columns -----' + CHAR(13) + CHAR(10);
            SET @CSharpModel += CHAR(9) + '#region Joined_Columns' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

            DECLARE @FKTable SYSNAME, @FKColumn SYSNAME, @PKTable SYSNAME, @PKColumn SYSNAME;
            DECLARE cRefTo CURSOR FOR
                SELECT FKTable, FKColumn, PKTable, PKColumn FROM #RefTo;
            OPEN cRefTo;
            FETCH NEXT FROM cRefTo INTO @FKTable, @FKColumn, @PKTable, @PKColumn;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                SET @CSharpModel += 
                    CHAR(9) + '[NotMapped]' + CHAR(13) + CHAR(10)
                    + CHAR(9) + '[ValidateNever]' + CHAR(13) + CHAR(10)
                    + CHAR(9) + 'public ' + @PKTable + ' ' + @PKTable + ' { get; set; }'
                    + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

                FETCH NEXT FROM cRefTo INTO @FKTable, @FKColumn, @PKTable, @PKColumn;
            END;

            CLOSE cRefTo;
            DEALLOCATE cRefTo;

            SET @CSharpModel += CHAR(9) + '#endregion' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);
        END;

        --------------------------------------------------------------------------
        -- (Optional) Not Mapped region => referencing tables => List<T>
        --------------------------------------------------------------------------
        SET @CSharpModel += CHAR(9) + '//__________ List and Single Object__________' + CHAR(13)+CHAR(10);
        SET @CSharpModel += CHAR(9) + '#region Note_Mapped' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

        IF EXISTS(SELECT 1 FROM #RefBy)
        BEGIN
            DECLARE @RB_FKTable SYSNAME, @RB_FKColumn SYSNAME, @RB_PKTable SYSNAME, @RB_PKColumn SYSNAME;
            DECLARE cRefBy CURSOR FOR
                SELECT FKTable, FKColumn, PKTable, PKColumn FROM #RefBy;
            OPEN cRefBy;
            FETCH NEXT FROM cRefBy INTO @RB_FKTable, @RB_FKColumn, @RB_PKTable, @RB_PKColumn;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                SET @CSharpModel += 
                    CHAR(9) + '[ValidateNever]' + CHAR(13) + CHAR(10)
                    + CHAR(9) + '[NotMapped]' + CHAR(13) + CHAR(10)
                    + CHAR(9) + 'public List<' + @RB_FKTable + '> ' + @RB_FKTable + 'List { get; set; }'
                    + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

                FETCH NEXT FROM cRefBy INTO @RB_FKTable, @RB_FKColumn, @RB_PKTable, @RB_PKColumn;
            END;

            CLOSE cRefBy;
            DEALLOCATE cRefBy;
        END;

        SET @CSharpModel += CHAR(9) + '#endregion' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);
        SET @CSharpModel += N'}' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

        ------------------------------------------------------------------------------
        -- 8b. Generate C# DTO if requested
        --     Same logic: If we are inheriting from BaseEntity => skip the audit columns
        ------------------------------------------------------------------------------
        DECLARE @CSharpDto NVARCHAR(MAX) = N'';
        IF @GenerateDto = 1
        BEGIN
            SET @CSharpDto = 
                N'public class ' + @DtoName
                + CASE WHEN @InheritBaseEntity = 1 THEN ' : BaseEntity' ELSE '' END
                + CHAR(13) + CHAR(10) + N'{' + CHAR(13) + CHAR(10);

            -- Constructor
            SET @CSharpDto += CHAR(9) + 'public ' + @DtoName + '()' + CHAR(13) + CHAR(10);
            SET @CSharpDto += CHAR(9) + '{' + CHAR(13) + CHAR(10);

            IF EXISTS(SELECT 1 FROM #RefBy)
            BEGIN
                DECLARE @RefBy2_FKTable SYSNAME, @RefBy2_FKColumn SYSNAME, @RefBy2_PKTable SYSNAME, @RefBy2_PKColumn SYSNAME;
                DECLARE cRefBy2 CURSOR FOR
                    SELECT FKTable, FKColumn, PKTable, PKColumn FROM #RefBy;
                OPEN cRefBy2;
                FETCH NEXT FROM cRefBy2 INTO @RefBy2_FKTable, @RefBy2_FKColumn, @RefBy2_PKTable, @RefBy2_PKColumn;

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    SET @CSharpDto += CHAR(9) + CHAR(9)
                        + @RefBy2_FKTable + 'Dtos = new List<' + @RefBy2_FKTable + 'Dto>();'
                        + CHAR(13) + CHAR(10);

                    FETCH NEXT FROM cRefBy2 INTO @RefBy2_FKTable, @RefBy2_FKColumn, @RefBy2_PKTable, @RefBy2_PKColumn;
                END;

                CLOSE cRefBy2;
                DEALLOCATE cRefBy2;
            END;

            SET @CSharpDto += CHAR(9) + '}' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

            -- Main Columns
            SET @CSharpDto += CHAR(9) + '//__________ Main Columns __________' + CHAR(13) + CHAR(10);
            SET @CSharpDto += CHAR(9) + '#region Main_Columns' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

            SELECT @CSharpDto += 
                CASE 
                    WHEN IsPrimaryKey=0 
                         AND IsNullable=0 
                         AND CSharpType='string' 
                         AND MaxLength>0 
                         AND MaxLength<8000
                         AND ColumnName NOT IN ('Active','CreatedBy','CreatedOn','ModifiedBy','ModifiedOn')
                        THEN CHAR(9) + '[StringLength(' + CAST(MaxLength AS VARCHAR(10)) + ')]' 
                             + CHAR(13)+CHAR(10)
                    ELSE ''
                END
                + CASE 
                    WHEN IsPrimaryKey=0 
                         AND IsNullable=0 
                         AND CSharpType NOT IN ('string','byte[]','object')
                         AND ColumnName NOT IN ('Active','CreatedBy','CreatedOn','ModifiedBy','ModifiedOn')
                        THEN CHAR(9) + '[Required]' + CHAR(13)+CHAR(10)
                    ELSE ''
                END
                + CASE 
                    WHEN ColumnName NOT IN ('Active','CreatedBy','CreatedOn','ModifiedBy','ModifiedOn')
                    THEN CHAR(9) + 'public '
                         + CASE WHEN IsNullable=1 
                                    AND CSharpType NOT IN ('string','byte[]','object') 
                                THEN CSharpType + '?' 
                                ELSE CSharpType END
                         + ' ' + ColumnName + ' { get; set; }' + CHAR(13) + CHAR(10)
                    ELSE ''
                END
            FROM #CS_Columns
            ORDER BY ColumnName;

            SET @CSharpDto += CHAR(13) + CHAR(10) + CHAR(9) + '#endregion' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

            -- Joined Columns
            IF EXISTS(SELECT 1 FROM #RefTo)
            BEGIN
                SET @CSharpDto += CHAR(9) + '//------ Joined Columns -----' + CHAR(13) + CHAR(10);
                SET @CSharpDto += CHAR(9) + '#region Joined_Columns' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

                DECLARE @FKTable2 SYSNAME, @FKColumn2 SYSNAME, @PKTable2 SYSNAME, @PKColumn2 SYSNAME;
                DECLARE cRefTo2 CURSOR FOR
                    SELECT FKTable, FKColumn, PKTable, PKColumn FROM #RefTo;
                OPEN cRefTo2;
                FETCH NEXT FROM cRefTo2 INTO @FKTable2, @FKColumn2, @PKTable2, @PKColumn2;

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    SET @CSharpDto += 
                        CHAR(9) + '[NotMapped]' + CHAR(13) + CHAR(10)
                        + CHAR(9) + '[ValidateNever]' + CHAR(13) + CHAR(10)
                        + CHAR(9) + 'public ' + @PKTable2 + 'Dto ' + @PKTable2 + 'Dto { get; set; }'
                        + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

                    FETCH NEXT FROM cRefTo2 INTO @FKTable2, @FKColumn2, @PKTable2, @PKColumn2;
                END;

                CLOSE cRefTo2;
                DEALLOCATE cRefTo2;

                SET @CSharpDto += CHAR(9) + '#endregion' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);
            END;

            -- Not Mapped => Lists
            SET @CSharpDto += CHAR(9) + '//__________ List and Single Object__________' + CHAR(13)+CHAR(10);
            SET @CSharpDto += CHAR(9) + '#region Note_Mapped' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

            IF EXISTS(SELECT 1 FROM #RefBy)
            BEGIN
                DECLARE @RB2_FKTable SYSNAME, @RB2_FKColumn SYSNAME, @RB2_PKTable SYSNAME, @RB2_PKColumn SYSNAME;
                DECLARE cRefBy3 CURSOR FOR
                    SELECT FKTable, FKColumn, PKTable, PKColumn FROM #RefBy;
                OPEN cRefBy3;
                FETCH NEXT FROM cRefBy3 INTO @RB2_FKTable, @RB2_FKColumn, @RB2_PKTable, @RB2_PKColumn;

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    SET @CSharpDto += 
                        CHAR(9) + '[ValidateNever]' + CHAR(13) + CHAR(10)
                        + CHAR(9) + '[NotMapped]' + CHAR(13) + CHAR(10)
                        + CHAR(9) + 'public IEnumerable<' + @RB2_FKTable + 'Dto> ' + @RB2_FKTable + 'Dtos { get; set; }'
                        + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

                    FETCH NEXT FROM cRefBy3 INTO @RB2_FKTable, @RB2_FKColumn, @RB2_PKTable, @RB2_PKColumn;
                END;

                CLOSE cRefBy3;
                DEALLOCATE cRefBy3;
            END;

            SET @CSharpDto += CHAR(9) + '#endregion' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

            SET @CSharpDto += N'}' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);
        END;

        -- Combine model + dto
        SET @CodeOutput = @CSharpModel + @CSharpDto;
    END
    ELSE IF @LanguageParam = 'Java'
    BEGIN
        --------------------------------------------------------------------------
        -- Basic Java Class
        --------------------------------------------------------------------------
        DECLARE @JavaModel NVARCHAR(MAX) = N'public class ' + @ClassName + N' {' + CHAR(13)+CHAR(10);

        -- Add private fields
        -- We do not skip the audit columns for Java, 
        -- but you can do so if you want (just replicate the filter).
        SELECT @JavaModel += CHAR(9)
            + 'private '
            + CASE 
                WHEN IsNullable=1 AND JavaType NOT IN ('String','byte[]') 
                    THEN JavaType 
                ELSE JavaType 
              END
            + ' ' + ColumnName + ';'
            + CHAR(13) + CHAR(10)
        FROM #CS_Columns;

        -- Add default constructor
        SET @JavaModel += CHAR(13)+CHAR(10) + CHAR(9) + 'public ' + @ClassName + '() {}' + CHAR(13)+CHAR(10);

        -- Add getters/setters
        SELECT @JavaModel += 
            CHAR(13) + CHAR(10)
            + CHAR(9) + 'public ' 
            + CASE 
                WHEN IsNullable=1 AND JavaType NOT IN ('String','byte[]')
                    THEN JavaType
                ELSE JavaType
              END
            + ' get' + ColumnName + '() {' + CHAR(13)+CHAR(10)
            + CHAR(9) + CHAR(9) + 'return ' + ColumnName + ';' + CHAR(13)+CHAR(10)
            + CHAR(9) + '}' + CHAR(13)+CHAR(10)
            + CHAR(9) + 'public void set' + ColumnName + '('
            + CASE 
                WHEN IsNullable=1 AND JavaType NOT IN ('String','byte[]')
                    THEN JavaType
                ELSE JavaType
              END
            + ' ' + ColumnName + ') {' + CHAR(13)+CHAR(10)
            + CHAR(9) + CHAR(9) + 'this.' + ColumnName + ' = ' + ColumnName + ';' + CHAR(13)+CHAR(10)
            + CHAR(9) + '}' + CHAR(13)+CHAR(10)
        FROM #CS_Columns;

        SET @JavaModel += N'}' + CHAR(13)+CHAR(10);

        SET @CodeOutput = @JavaModel;
    END
    ELSE IF @LanguageParam = 'Js'
    BEGIN
        --------------------------------------------------------------------------
        -- Basic JavaScript (ES6) class
        --------------------------------------------------------------------------
        DECLARE @JsModel NVARCHAR(MAX) = N'export class ' + @ClassName + N' {' + CHAR(13)+CHAR(10);
        SET @JsModel += CHAR(9) + 'constructor() {' + CHAR(13)+CHAR(10);

        SELECT @JsModel += 
            CHAR(9) + CHAR(9) + 'this.' + ColumnName + ' = null;' + CHAR(13)+CHAR(10)
        FROM #CS_Columns;

        SET @JsModel += CHAR(9) + '}' + CHAR(13)+CHAR(10);
        SET @JsModel += N'}' + CHAR(13)+CHAR(10);

        SET @CodeOutput = @JsModel;
    END
    ELSE IF @LanguageParam = 'Flutter'
    BEGIN
        --------------------------------------------------------------------------
        -- Basic Dart class for Flutter
        --------------------------------------------------------------------------
        DECLARE @DartModel NVARCHAR(MAX) = N'class ' + @ClassName + N' {' + CHAR(13)+CHAR(10);

        -- Constructor signature
        SET @DartModel += CHAR(9) + @ClassName + '({' + CHAR(13)+CHAR(10);

        SELECT @DartModel += 
            CHAR(9) + CHAR(9) + 'this.' + ColumnName + ',' + CHAR(13)+CHAR(10)
        FROM #CS_Columns;

        SET @DartModel += CHAR(9) + '});' + CHAR(13)+CHAR(10) + CHAR(13)+CHAR(10);

        -- Fields
        SELECT @DartModel += 
            CHAR(9) + 'final '
            + CASE 
                WHEN IsNullable=1 AND DartType NOT IN ('String','var') 
                    THEN DartType + '?' 
                ELSE DartType
              END
            + ' ' + ColumnName + ';' + CHAR(13) + CHAR(10)
        FROM #CS_Columns;

        SET @DartModel += N'}' + CHAR(13)+CHAR(10);

        SET @CodeOutput = @DartModel;
    END
    ELSE
    BEGIN
        SET @CodeOutput = N'-- Unsupported language parameter: ' + @LanguageParam;
    END;

    ------------------------------------------------------------------------------
    -- 9. Print or SELECT the final code
    ------------------------------------------------------------------------------
    -- Because code can exceed 8k, we do chunked printing:
    DECLARE @ChunkSize INT = 4000;
    DECLARE @TotalLen INT = LEN(@CodeOutput);
    DECLARE @Pos INT = 1;
    WHILE @Pos <= @TotalLen
    BEGIN
        PRINT SUBSTRING(@CodeOutput, @Pos, @ChunkSize);
        SET @Pos += @ChunkSize;
    END;

    ------------------------------------------------------------------------------
    -- 10. Cleanup
    ------------------------------------------------------------------------------
    IF OBJECT_ID('tempdb..#Tmp_Columns') IS NOT NULL
        DROP TABLE #Tmp_Columns;
    IF OBJECT_ID('tempdb..#TypeMapping') IS NOT NULL
        DROP TABLE #TypeMapping;
    IF OBJECT_ID('tempdb..#CS_Columns') IS NOT NULL
        DROP TABLE #CS_Columns;
    IF OBJECT_ID('tempdb..#FK_Relationships') IS NOT NULL
        DROP TABLE #FK_Relationships;
    IF OBJECT_ID('tempdb..#RefBy') IS NOT NULL
        DROP TABLE #RefBy;
    IF OBJECT_ID('tempdb..#RefTo') IS NOT NULL
        DROP TABLE #RefTo;
END;
GO




-- ============================================================================================================
--=================================== Reset Settings ========================================================
-- ============================================================================================================

GO
IF OBJECT_ID('dbo.ResetPurchaseHierarchy', 'P') IS NOT NULL
    DROP PROCEDURE dbo.ResetPurchaseHierarchy;
GO
create proc ResetPurchaseHierarchy
as
begin
	delete from PODtls
	delete from POs

	delete from PIDtls
	delete from PIs

	delete from PRDtls
	delete from PRs

end

go


IF OBJECT_ID('dbo.ResetSalesHierarchy', 'P') IS NOT NULL
    DROP PROCEDURE dbo.ResetSalesHierarchy;
go
Create proc ResetSalesHierarchy
as
begin
	delete from SODtls
	delete from SOs

	delete from SODtls
	delete from SOs

	delete from SRDtls
	delete from SRs

end

