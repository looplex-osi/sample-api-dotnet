USE [dotnetsamples]
GO

-- Migration name defined here
DECLARE @MigrationId NVARCHAR(255) = 'InitAcademicModule'

BEGIN TRY
    BEGIN TRANSACTION
    
    DECLARE @MigrationError NVARCHAR(510) = 'Migration [' + @MigrationId + '] already applied'
    IF EXISTS (SELECT TOP 1 1 FROM MigrationHistory WHERE MigrationId = @MigrationId)        
        THROW 0, @MigrationError, 1;      

    -- Migration code starts here

    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Students')
    BEGIN
        CREATE TABLE Students (
            Id INT PRIMARY KEY IDENTITY(1,1),
            UserId INT,
            RegistrationId VARCHAR(255) NOT NULL
        );
    END;

    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SchoolSubjects')
    BEGIN
        CREATE TABLE SchoolSubjects (
            Id INT PRIMARY KEY IDENTITY(1,1),            
            [Name] VARCHAR(255) NOT NULL
        );
    END;

    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SchoolSubjectsStudents')
    BEGIN
        CREATE TABLE SchoolSubjects_Students (
            Id INT PRIMARY KEY IDENTITY(1,1),
            SchoolSubjectId INT FOREIGN KEY REFERENCES SchoolSubjects(Id),
            StudentId INT FOREIGN KEY REFERENCES Students(Id),
            IsActive BIT NOT NULL
        );
    END;

    -- Migration code ends here

    INSERT INTO MigrationHistory (MigrationId, AppliedOn)
    VALUES (@MigrationId, GETDATE());

    COMMIT;
END TRY
BEGIN CATCH    
    ROLLBACK;
    
    PRINT 'An error occurred when running migration [' + @MigrationId + ']: ' + ERROR_MESSAGE();
END CATCH;