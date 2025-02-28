--USE zooefc;
--GO

--01-create-schema.sql
--create a schema for guest users, i.e. not logged in
CREATE SCHEMA gstusr;
GO

--create a schema for logged in user
CREATE SCHEMA usr;
GO

--02-create-gstusr-view.sql
--create a view that gives overview of the database content
CREATE OR ALTER VIEW gstusr.vwInfoDb AS
    SELECT 'Guest user database overview' as Title,
    (SELECT COUNT(*) FROM supusr.Zoos WHERE Seeded = 1) as nrSeededZoos, 
    (SELECT COUNT(*) FROM supusr.Zoos WHERE Seeded = 0) as nrUnseededZoos,
    (SELECT COUNT(*) FROM supusr.Animals WHERE Seeded = 1) as nrSeededAnimals, 
    (SELECT COUNT(*) FROM supusr.Animals WHERE Seeded = 0) as nrUnseededAnimals,
    (SELECT COUNT(*) FROM supusr.Employees WHERE Seeded = 1) as nrSeededEmployees, 
    (SELECT COUNT(*) FROM supusr.Employees WHERE Seeded = 0) as nrUnseededEmployees,
    (SELECT COUNT(*) FROM supusr.CreditCards WHERE Seeded = 1) as nrSeededCreditCards, 
    (SELECT COUNT(*) FROM supusr.CreditCards WHERE Seeded = 0) as nrUnseededCreditCards

GO

CREATE OR ALTER VIEW gstusr.vwInfoZoos AS
    SELECT z.Country, z.City, COUNT(*) as NrZoos  FROM supusr.Zoos z
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



--03-create-supusr-sp.sql
CREATE OR ALTER PROC supusr.spDeleteAll
    @Seeded BIT = 1

    AS

    SET NOCOUNT ON;

    DELETE FROM supusr.Zoos WHERE Seeded = @Seeded;
    DELETE FROM supusr.Animals WHERE Seeded = @Seeded;
    DELETE FROM supusr.Employees WHERE Seeded = @Seeded;

    SELECT * FROM gstusr.vwInfoDb;

    --throw our own error
    --;THROW 999999, 'my own supusr.spDeleteAll Error directly from SQL Server', 1

    --show return code usage
    RETURN 0;  --indicating success
    --RETURN 1;  --indicating your own error code, in this case 1
GO

--04-create-users-azure.sql
--create 3 users we will late set credentials for these
DROP USER IF EXISTS  gstusrUser;
DROP USER IF EXISTS usrUser;
DROP USER IF EXISTS supusrUser;

CREATE USER gstusrUser WITH PASSWORD = N'pa$$Word1'; 
CREATE USER usrUser WITH PASSWORD = N'pa$$Word1'; 
CREATE USER supusrUser WITH PASSWORD = N'pa$$Word1'; 

ALTER ROLE db_datareader ADD MEMBER gstusrUser; 
ALTER ROLE db_datareader ADD MEMBER usrUser; 
ALTER ROLE db_datareader ADD MEMBER supusrUser; 
GO

--05-create-roles-credentials.sql
--create roles
CREATE ROLE zoosefcGstUsr;
CREATE ROLE zoosefcUsr;
CREATE ROLE zoosefcSupUsr;

--assign securables creadentials to the roles
GRANT SELECT, EXECUTE ON SCHEMA::gstusr to zoosefcGstUsr;
GRANT SELECT ON SCHEMA::supusr to zoosefcUsr;
GRANT SELECT, UPDATE, INSERT, DELETE, EXECUTE ON SCHEMA::supusr to zoosefcSupUsr;

--finally, add the users to the roles
ALTER ROLE zoosefcGstUsr ADD MEMBER gstusrUser;

ALTER ROLE zoosefcGstUsr ADD MEMBER usrUser;
ALTER ROLE zoosefcUsr ADD MEMBER usrUser;

ALTER ROLE zoosefcGstUsr ADD MEMBER supusrUser;
ALTER ROLE zoosefcUsr ADD MEMBER supusrUser;
ALTER ROLE zoosefcSupUsr ADD MEMBER supusrUser;
GO

--07-create-gstusr-login.sql
CREATE OR ALTER PROC gstusr.spLogin
    @UserNameOrEmail NVARCHAR(100),
    @Password NVARCHAR(200),

    @UserId UNIQUEIDENTIFIER OUTPUT,
    @UserName NVARCHAR(100) OUTPUT,
    @Role NVARCHAR(100) OUTPUT
    
    AS

    SET NOCOUNT ON;
    
    SET @UserId = NULL;
    SET @UserName = NULL;
    SET @Role = NULL;
    
    SELECT Top 1 @UserId = UserId, @UserName = UserName, @Role = [Role] FROM dbo.Users 
    WHERE ((UserName = @UserNameOrEmail) OR
           (Email IS NOT NULL AND (Email = @UserNameOrEmail))) AND ([Password] = @Password);
    
    IF (@UserId IS NULL)
    BEGIN
        ;THROW 999999, 'Login error: wrong user or password', 1
    END

GO

