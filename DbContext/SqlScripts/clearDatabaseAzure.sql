--USE zooefc;
--GO

-- remove spLogin
DROP PROCEDURE IF EXISTS gstusr.spLogin
GO

-- remove roles
ALTER ROLE zoosefcGstUsr DROP MEMBER gstusrUser;
ALTER ROLE zoosefcGstUsr DROP MEMBER usrUser;
ALTER ROLE zoosefcGstUsr DROP MEMBER supusrUser;

ALTER ROLE zoosefcUsr DROP MEMBER usrUser;
ALTER ROLE zoosefcUsr DROP MEMBER supusrUser;

ALTER ROLE zoosefcSupUsr DROP MEMBER supusrUser;

DROP ROLE IF EXISTS zoosefcGstUsr;
DROP ROLE IF EXISTS zoosefcUsr;
DROP ROLE IF EXISTS zoosefcSupUsr;
GO

--drop users
DROP USER IF EXISTS  gstusrUser;
DROP USER IF EXISTS usrUser;
DROP USER IF EXISTS supusrUser;
GO

-- remove spDeleteAll
DROP PROCEDURE IF EXISTS supusr.spDeleteAll
GO

DROP VIEW IF EXISTS [gstusr].[vwInfoDb]
DROP VIEW IF EXISTS [gstusr].[vwInfoZoos]
DROP VIEW IF EXISTS [gstusr].[vwInfoAnimals]
DROP VIEW IF EXISTS [gstusr].[vwInfoEmployees]
GO

--drop tables in the right order not to get fk conflicts
DROP TABLE IF EXISTS supusr.TemplateModel;
DROP TABLE IF EXISTS dbo.Users;
DROP TABLE IF EXISTS supusr.EmployeeDbMZooDbM;
DROP TABLE IF EXISTS supusr.Animals;
DROP TABLE IF EXISTS supusr.Zoos;
DROP TABLE IF EXISTS supusr.CreditCards;
DROP TABLE IF EXISTS supusr.Employees;
DROP TABLE IF EXISTS dbo.__EFMigrationsHistory;
GO

-- drop schemas
DROP SCHEMA IF EXISTS gstusr;
DROP SCHEMA IF EXISTS usr;
DROP SCHEMA IF EXISTS supusr;
GO