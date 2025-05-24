-- Create schemas
CREATE SCHEMA gstusr;
GO

CREATE SCHEMA usr;
GO

-- Create Views
CREATE OR ALTER VIEW gstusr.vwInfoDb AS
    SELECT 'Guest user database overview' as Title,
    (SELECT COUNT(*) FROM supusr.Attractions WHERE Seeded = 1) as NrSeededAttractions, 
    (SELECT COUNT(*) FROM supusr.Attractions WHERE Seeded = 0) as NrUnseededAttractions,
    (SELECT COUNT(*) FROM supusr.Comments WHERE Seeded = 1) as NrSeededComments, 
    (SELECT COUNT(*) FROM supusr.Comments WHERE Seeded = 0) as NrUnseededComments,
    (SELECT COUNT(*) FROM supusr.Addresses WHERE Seeded = 1) as NrSeededAdresses, 
    (SELECT COUNT(*) FROM supusr.Addresses WHERE Seeded = 0) as NrUnseededAdresses,
    (SELECT COUNT(*) FROM supusr.Categories WHERE Seeded = 1) as NrSeededCategories, 
    (SELECT COUNT(*) FROM supusr.Categories WHERE Seeded = 0) as NrUnseededCategories;
GO

CREATE OR ALTER VIEW gstusr.vwInfoZoos AS
    SELECT z.Country, z.City, COUNT(*) as NrZoos FROM supusr.Zoos z
    GROUP BY z.Country, z.City WITH ROLLUP;
GO

CREATE OR ALTER VIEW gstusr.vwInfoAnimals AS
    SELECT z.Country, z.City, z.Name as ZooName, COUNT(a.AnimalId) as NrAnimals FROM supusr.Zoos z
    INNER JOIN supusr.Animals a ON a.ZooDbMZooId = z.ZooId
    GROUP BY z.Country, z.City, z.Name WITH ROLLUP;
GO

CREATE OR ALTER VIEW gstusr.vwInfoEmployees AS
    SELECT z.Country, z.City, z.Name as ZooName, COUNT(e.EmployeeId) as NrEmployees FROM supusr.Zoos z
    INNER JOIN supusr.EmployeeDbMZooDbM ct ON ct.ZoosDbMZooId = z.ZooId
    INNER JOIN supusr.Employees e ON e.EmployeeId = ct.EmployeesDbMEmployeeId
    GROUP BY z.Country, z.City, z.Name WITH ROLLUP;
GO

-- Stored Procedure to Delete Data
CREATE OR ALTER PROC supusr.spDeleteAll
    @Seeded BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM supusr.Zoos WHERE Seeded = @Seeded;
    DELETE FROM supusr.Animals WHERE Seeded = @Seeded;
    DELETE FROM supusr.Employees WHERE Seeded = @Seeded;

    SELECT * FROM gstusr.vwInfoDb;

    RETURN 0;  -- indicating success
END;
GO

-- User Creation (Modified for Azure SQL)
DROP USER IF EXISTS gstusrUser;
DROP USER IF EXISTS usrUser;
DROP USER IF EXISTS supusrUser;

CREATE USER gstusrUser WITH PASSWORD = 'pa$$Word1';
CREATE USER usrUser WITH PASSWORD = 'pa$$Word1';
CREATE USER supusrUser WITH PASSWORD = 'pa$$Word1';
GO

-- Create Roles
CREATE ROLE zooefcGstUsr;
CREATE ROLE zooefcUsr;
CREATE ROLE zooefcSupUsr;

-- Assign Permissions
GRANT SELECT, EXECUTE ON SCHEMA::gstusr TO zooefcGstUsr;
GRANT SELECT ON SCHEMA::supusr TO zooefcUsr;
GRANT SELECT, UPDATE, INSERT, DELETE, EXECUTE ON SCHEMA::supusr TO zooefcSupUsr;

-- Assign Users to Roles
ALTER ROLE zooefcGstUsr ADD MEMBER gstusrUser;
ALTER ROLE zooefcUsr ADD MEMBER usrUser;
ALTER ROLE zooefcSupUsr ADD MEMBER supusrUser;
GO

-- Login Procedure
CREATE OR ALTER PROC gstusr.spLogin
    @UserNameOrEmail NVARCHAR(100),
    @Password NVARCHAR(200),
    @UserId UNIQUEIDENTIFIER OUTPUT,
    @UserName NVARCHAR(100) OUTPUT,
    @Role NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @UserId = NULL;
    SET @UserName = NULL;
    SET @Role = NULL;

    SELECT TOP 1 @UserId = UserId, @UserName = UserName, @Role = [Role] FROM dbo.Users 
    WHERE ((UserName = @UserNameOrEmail) OR
           (Email IS NOT NULL AND Email = @UserNameOrEmail)) 
          AND ([Password] = @Password);

    IF (@UserId IS NULL)
    BEGIN
        THROW 999999, 'Login error: wrong user or password', 1;
    END
END;
GO
