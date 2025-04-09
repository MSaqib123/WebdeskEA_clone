--CREATE TABLE [dbo].[GlobalSettings] (
--    [Id]           INT            IDENTITY (1, 1) NOT NULL,
--    [TenantId]     INT            NULL,
--    [CompanyId]    INT            NULL,
--    [UserId]       NVARCHAR(450)  NULL,
--    [SettingKey]   NVARCHAR(200)  NOT NULL,
--    [SettingValue] NVARCHAR(MAX)  NOT NULL,  -- can store large text or JSON
--    [ValueType]    NVARCHAR(50)   NULL,      -- e.g., 'string', 'json', 'int'
--    [CreatedDate]  DATETIME2(7)   NOT NULL CONSTRAINT DF_GlobalSettings_CreatedDate DEFAULT (GETUTCDATE()),
--    [UpdatedDate]  DATETIME2(7)   NOT NULL CONSTRAINT DF_GlobalSettings_UpdatedDate DEFAULT (GETUTCDATE()),
--    CONSTRAINT [PK_GlobalSettings] PRIMARY KEY CLUSTERED ([Id] ASC)
--);

---- Optional: Add indexes to speed up typical queries:
--CREATE NONCLUSTERED INDEX IX_GlobalSettings_TenantId_CompanyId_UserId_Key
--    ON [dbo].[GlobalSettings] ([TenantId], [CompanyId], [UserId], [SettingKey]);



IF OBJECT_ID('[dbo].[spGlobalSettings_GetAll]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[spGlobalSettings_GetAll];
GO
CREATE PROCEDURE [dbo].[spGlobalSettings_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        TenantId,
        CompanyId,
        UserId,
        SettingKey,
        SettingValue,
        ValueType,
        CreatedDate,
        UpdatedDate
    FROM GlobalSettings
    ORDER BY Id;
END;
go





IF OBJECT_ID('[dbo].[spGlobalSettings_GetById]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[spGlobalSettings_GetById];
GO
CREATE PROCEDURE [dbo].[spGlobalSettings_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        TenantId,
        CompanyId,
        UserId,
        SettingKey,
        SettingValue,
        ValueType,
        CreatedDate,
        UpdatedDate
    FROM GlobalSettings
    WHERE Id = @Id;
END;
go






IF OBJECT_ID('[dbo].[spGlobalSettings_GetByTenantCompanyUserKey]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[spGlobalSettings_GetByTenantCompanyUserKey];
GO
CREATE PROCEDURE [dbo].[spGlobalSettings_GetByTenantCompanyUserKey]
    @TenantId INT = NULL,
    @CompanyId INT = NULL,
    @UserId NVARCHAR(450) = NULL,
    @SettingKey NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        TenantId,
        CompanyId,
        UserId,
        SettingKey,
        SettingValue,
        ValueType,
        CreatedDate,
        UpdatedDate
    FROM GlobalSettings
    WHERE 
        (@TenantId IS NULL OR TenantId = @TenantId)
        AND (@CompanyId IS NULL OR CompanyId = @CompanyId)
        AND (@UserId IS NULL OR UserId = @UserId)
        AND SettingKey = @SettingKey;
END;
go






IF OBJECT_ID('[dbo].[spGlobalSettings_Insert]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[spGlobalSettings_Insert];
GO
CREATE PROCEDURE [dbo].[spGlobalSettings_Insert]
    @TenantId INT = NULL,
    @CompanyId INT = NULL,
    @UserId NVARCHAR(450) = NULL,
    @SettingKey NVARCHAR(200),
    @SettingValue NVARCHAR(MAX),
    @ValueType NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO GlobalSettings 
    (
        TenantId, 
        CompanyId, 
        UserId, 
        SettingKey, 
        SettingValue, 
        ValueType, 
        CreatedDate, 
        UpdatedDate
    )
    VALUES 
    (
        @TenantId, 
        @CompanyId, 
        @UserId, 
        @SettingKey, 
        @SettingValue, 
        @ValueType, 
        GETUTCDATE(), 
        GETUTCDATE()
    );

    SELECT SCOPE_IDENTITY() AS [Id]; -- Return the new record's ID
END;
go








IF OBJECT_ID('[dbo].[spGlobalSettings_Update]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[spGlobalSettings_Update];
GO

CREATE PROCEDURE [dbo].[spGlobalSettings_Update]
    @Id INT,
    @SettingValue NVARCHAR(MAX),
    @ValueType NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE GlobalSettings
    SET
        SettingValue = @SettingValue,
        ValueType = @ValueType,
        UpdatedDate = GETUTCDATE()
    WHERE Id = @Id;
END;
go






IF OBJECT_ID('[dbo].[spGlobalSettings_Delete]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[spGlobalSettings_Delete];
GO

CREATE PROCEDURE [dbo].[spGlobalSettings_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM GlobalSettings
    WHERE Id = @Id;
END;
GO