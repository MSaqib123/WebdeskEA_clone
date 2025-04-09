GO
--_____________________________________________ spCompanyUser_BulkInsert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyUser_BulkInsert') DROP PROCEDURE [dbo].[spCompanyUser_BulkInsert]
GO
CREATE PROCEDURE [dbo].[spCompanyUser_BulkInsert]
    @jsonInput NVARCHAR(MAX) 
AS 
BEGIN
    ---- Declare a table variable to hold the parsed JSON data
    --DECLARE @parsedJson AS TABLE (
    --    CompanyId INT,
    --    UserId NVARCHAR(400)
    --);

    ---- Parse the JSON input and insert it into the table variable
    --INSERT INTO @parsedJson (CompanyId, UserId)
    --SELECT 
    --    JSON_VALUE(value, '$.CompanyId') AS CompanyId,
    --    JSON_VALUE(value, '$.UserId') AS UserId
    --FROM OPENJSON(@jsonInput);

    ---- Insert the parsed data into the CompanyUsers table
    --INSERT INTO CompanyUsers (CompanyId, UserId)
    --SELECT CompanyId, UserId 
    --FROM @parsedJson;
	print 'SQL not Supported with json this hosted version'
END;
GO
--_____________________________________________ spCompanyUser_BulkUpdate _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyUser_BulkUpdate') DROP PROCEDURE [dbo].[spCompanyUser_BulkUpdate]
GO
CREATE PROCEDURE [dbo].[spCompanyUser_BulkUpdate]      
    @jsonInput NVARCHAR(MAX) 
AS 
BEGIN
    ---- Start a transaction
    --BEGIN TRANSACTION;

    --BEGIN TRY
    --    -- Create a table variable to hold parsed JSON data
    --    DECLARE @parsedJson AS TABLE (
    --        CompanyId INT,
    --        UserId NVARCHAR(400)
    --    );

    --    -- Parse the JSON input and insert it into the table variable
    --    INSERT INTO @parsedJson (CompanyId, UserId)
    --    SELECT 
    --        JSON_VALUE(value, '$.CompanyId') AS CompanyId,
    --        JSON_VALUE(value, '$.UserId') AS UserId
    --    FROM OPENJSON(@jsonInput);

    --    -- Delete existing records for the specified CompanyId from the table variable
    --    DELETE FROM CompanyUsers 
    --    WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM @parsedJson);

    --    -- Insert the new records into the CompanyUsers table
    --    INSERT INTO CompanyUsers (CompanyId, UserId)
    --    SELECT CompanyId, UserId FROM @parsedJson;

    --    -- Commit the transaction
    --    COMMIT TRANSACTION;
    --END TRY
    --BEGIN CATCH
    --    -- Rollback the transaction in case of error
    --    ROLLBACK TRANSACTION;
    --    -- Re-throw the error
    --    THROW;
    --END CATCH;
	print 'SQL not supported Json this verison'
END;
GO
GO
--_____________________________________________ spCompanyUser_Delete _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyUser_Delete') DROP PROCEDURE [dbo].[spCompanyUser_Delete]
GO
create PROCEDURE [dbo].[spCompanyUser_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM CompanyUsers
    WHERE CompanyId = @Id;
END;
GO
--_____________________________________________ spCompanyUser_GetAll _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyUser_GetAll') DROP PROCEDURE [dbo].[spCompanyUser_GetAll]
GO
CREATE PROCEDURE [dbo].[spCompanyUser_GetAll]
AS
BEGIN
    SELECT cu.CompanyId, cu.UserId, c.Name as CompanyName, u.UserName
    FROM CompanyUsers cu
    INNER JOIN Company c ON cu.CompanyId = c.Id
    INNER JOIN [AspNetUsers] u ON cu.UserId = u.Id
END;
GO
--_____________________________________________ spCompanyUser_GetByCompanyId _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyUser_GetByCompanyId') DROP PROCEDURE [dbo].[spCompanyUser_GetByCompanyId]
GO
CREATE PROCEDURE [dbo].[spCompanyUser_GetByCompanyId]   
    @CompanyId INT  
AS    
BEGIN    
    SELECT 
        c.Id AS CompanyId,
        cu.UserId,
        c.Name AS CompanyName, 
        u.UserName    
    FROM 
        Company c
    LEFT JOIN 
        CompanyUsers cu ON cu.CompanyId = c.Id    
    LEFT JOIN 
        [AspNetUsers] u ON cu.UserId = u.Id    
    WHERE 
        c.Id = @CompanyId  
END;
GO
--_____________________________________________ spCompanyUser_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyUser_GetById') DROP PROCEDURE [dbo].[spCompanyUser_GetById]
GO
CREATE PROCEDURE [dbo].[spCompanyUser_GetById]
    @Id INT
AS
BEGIN
    SELECT cu.CompanyId, cu.UserId, c.Name as CompanyName, u.UserName
    FROM CompanyUsers cu
    INNER JOIN Company c ON cu.CompanyId = c.Id
    INNER JOIN [AspNetUsers] u ON cu.UserId = u.Id
    WHERE cu.CompanyId = @Id;
END;
GO
--_____________________________________________ spCompanyUser_GetByName _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyUser_GetByName') DROP PROCEDURE [dbo].[spCompanyUser_GetByName]
GO
CREATE PROCEDURE [dbo].[spCompanyUser_GetByName]
    @Name NVARCHAR(256)
AS
BEGIN
    SELECT cu.CompanyId, cu.UserId, c.Name, u.UserName
    FROM CompanyUsers cu
    INNER JOIN Company c ON cu.CompanyId = c.Id
    INNER JOIN [AspNetUsers] u ON cu.UserId = u.Id
    WHERE c.Name LIKE '%' + @Name + '%' OR u.UserName LIKE '%' + @Name + '%';
END;
GO
--_____________________________________________ spCompanyUser_GetPaginated _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyUser_GetPaginated') DROP PROCEDURE [dbo].[spCompanyUser_GetPaginated]
GO
CREATE PROCEDURE [dbo].[spCompanyUser_GetPaginated]
    @PageIndex INT,
    @PageSize INT,
    @Filter NVARCHAR(256)
AS
BEGIN
    SELECT cu.CompanyId, cu.UserId, c.Name, u.UserName
    FROM CompanyUsers cu
    INNER JOIN Company c ON cu.CompanyId = c.Id
    INNER JOIN [ASpNetUser] u ON cu.UserId = u.Id
    WHERE c.Name LIKE '%' + @Filter + '%' OR u.UserName LIKE '%' + @Filter + '%'
    ORDER BY cu.CompanyId
    OFFSET (@PageIndex - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO
--_____________________________________________ spCompanyUser_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyUser_Insert') DROP PROCEDURE [dbo].[spCompanyUser_Insert]
GO
CREATE PROCEDURE [dbo].[spCompanyUser_Insert]
    @CompanyId INT,
    @UserId NVARCHAR(128)
AS
BEGIN
    INSERT INTO CompanyUsers (CompanyId, UserId)
    VALUES (@CompanyId, @UserId);
END;
GO
--_____________________________________________ spCompanyUser_Update _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyUser_Update') DROP PROCEDURE [dbo].[spCompanyUser_Update]
GO
CREATE PROCEDURE [dbo].[spCompanyUser_Update]
    @CompanyId INT,
    @UserId NVARCHAR(128)
AS
BEGIN
    UPDATE CompanyUsers
    SET UserId = @UserId
    WHERE CompanyId = @CompanyId;
END;
GO


