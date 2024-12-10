USE [master]
GO

IF DB_ID('lawoffice') IS NOT NULL
  set noexec on 

-- IMPORTANT: looplex database (LATIN1_GENERAL_100_CI_AI_SC_UTF8) and lawoffice schema are REQUIRED as prerequisites.
CREATE DATABASE [lawoffice]
COLLATE LATIN1_GENERAL_100_CI_AI_SC_UTF8;
GO

USE [lawoffice]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE SCHEMA lawoffice;
GO

CREATE LOGIN [lawofficeUser] WITH PASSWORD = '!ooplex_D0tNet!'
GO

CREATE USER [lawofficeUser] FOR LOGIN [lawofficeUser]
GO

GRANT SELECT ON SCHEMA::lawoffice TO [lawofficeUser];
GO

GRANT INSERT ON SCHEMA::lawoffice TO [lawofficeUser];
GO

GRANT UPDATE ON SCHEMA::lawoffice TO [lawofficeUser];
GO

GRANT DELETE ON SCHEMA::lawoffice TO [lawofficeUser];
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'lawoffice.MigrationHistory')
BEGIN
    CREATE TABLE lawoffice.MigrationHistory (
        Id INT PRIMARY KEY IDENTITY(1,1),
        MigrationId NVARCHAR(255) NOT NULL,
        AppliedOn DATETIME NOT NULL
    );
END;