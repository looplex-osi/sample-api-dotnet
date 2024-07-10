USE [master]
GO

IF DB_ID('dotnetsamples') IS NOT NULL
  set noexec on 

CREATE DATABASE [dotnetsamples];
GO

USE [dotnetsamples]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE LOGIN [samplesUser] WITH PASSWORD = '!ooplex_D0tNet!'
GO

CREATE USER [samplesUser] FOR LOGIN [samplesUser]
GO

GRANT SELECT ON SCHEMA::dbo TO [samplesUser];
GO

GRANT INSERT ON SCHEMA::dbo TO [samplesUser];
GO

GRANT UPDATE ON SCHEMA::dbo TO [samplesUser];
GO

GRANT DELETE ON SCHEMA::dbo TO [samplesUser];
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MigrationHistory')
BEGIN
    CREATE TABLE MigrationHistory (
        Id INT PRIMARY KEY IDENTITY(1,1),
        MigrationId NVARCHAR(255) NOT NULL,
        AppliedOn DATETIME NOT NULL
    );
END;