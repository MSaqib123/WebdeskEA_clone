GO
--_____________________________________________ spCompanyBuesinessCategory_GetAll _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyBuesinessCategory_GetAll') DROP PROCEDURE [dbo].[spCompanyBuesinessCategory_GetAll]
GO
create PROCEDURE [dbo].[spCompanyBuesinessCategory_GetAll]
AS  
BEGIN  
	SELECT       
        BC.Id,      
        BC.[Name],      
		BC.CompanyId,    
        c.Name AS CompanyName    
    FROM       
        BusinessCategories BC    
    LEFT JOIN       
        Companies c ON BC.CompanyId = c.Id    
END;
GO
--_____________________________________________ spCompanyBusinessCategory_Delete _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyBusinessCategory_Delete') DROP PROCEDURE [dbo].[spCompanyBusinessCategory_Delete]
GO
create PROCEDURE [dbo].[spCompanyBusinessCategory_Delete]
    @Id INT  
AS  
BEGIN  
    DELETE FROM BusinessCategories 
    WHERE Id = @Id;  
END;
GO
--_____________________________________________ spCompanyBusinessCategory_GetByCompanyId _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyBusinessCategory_GetByCompanyId') DROP PROCEDURE [dbo].[spCompanyBusinessCategory_GetByCompanyId]
GO
CREATE PROCEDURE [dbo].[spCompanyBusinessCategory_GetByCompanyId]
    @CompanyId INT    
AS      
BEGIN      
    SELECT   
        BC.Id,  
        BC.[Name],  
		BC.CompanyId,
        c.Name AS CompanyName
    FROM   
        BusinessCategories BC
    LEFT JOIN   
        Companies c ON BC.CompanyId = c.Id
    WHERE   
        c.Id = @CompanyId    
END;  
GO
--_____________________________________________ spCompanyBusinessCategory_GetById _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyBusinessCategory_GetById') DROP PROCEDURE [dbo].[spCompanyBusinessCategory_GetById]
GO
CREATE PROCEDURE [dbo].[spCompanyBusinessCategory_GetById]
    @Id INT    
AS    
BEGIN    
    SELECT       
        BC.Id,      
        BC.[Name],      
		BC.CompanyId,    
        c.Name AS CompanyName    
    FROM       
        BusinessCategories BC    
    LEFT JOIN       
        Companies c ON BC.CompanyId = c.Id    
    WHERE       
        BC.Id = @Id         
END;
GO
--_____________________________________________ spCompanyBusinessCategory_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyBusinessCategory_Insert') DROP PROCEDURE [dbo].[spCompanyBusinessCategory_Insert]
GO
CREATE PROCEDURE [dbo].[spCompanyBusinessCategory_Insert]  
 @Id INT OUTPUT,  
 @Name NVARCHAR(128),   
 @CompanyId INT
AS    
BEGIN    
	 Insert into BusinessCategories (  
	  Name,  
	  CompanyId  
	 )  
	 values (  
	  @Name,  
	  @CompanyId  
	 )  
  -- Get the last inserted ID within the same scope
    SET @Id = SCOPE_IDENTITY();
END;
GO
--_____________________________________________ spCompanyBusinessCategory_Update _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spCompanyBusinessCategory_Update') DROP PROCEDURE [dbo].[spCompanyBusinessCategory_Update]
GO
CREATE PROCEDURE [dbo].[spCompanyBusinessCategory_Update]  
	@Id int,
	@Name NVARCHAR(50),
    @CompanyId INT
AS  
BEGIN  
    UPDATE BusinessCategories
    SET 
	Name = @Name,
	CompanyId = @CompanyId
    WHERE Id = @Id;  
END;
GO

