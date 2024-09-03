USE [dotnetsamples]
GO

-- Migration name defined here
DECLARE @MigrationId NVARCHAR(255) = 'InitAcademicModule'

IF NOT EXISTS (SELECT TOP 1 1 FROM MigrationHistory WHERE MigrationId = @MigrationId) 
BEGIN
    PRINT 'Running migration [' + @MigrationId + ']';

    BEGIN TRY
        BEGIN TRANSACTION  
    
        -- Migration code starts here
    
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'students')
        BEGIN
            CREATE TABLE students (
                [id] int IDENTITY(-2147483648, 1) NOT NULL CONSTRAINT PK_students_id PRIMARY KEY CLUSTERED,
                [uuid] uniqueidentifier NOT NULL CONSTRAINT DF_students_uuid DEFAULT NEWSEQUENTIALID(),
                [external_id] varchar(255) NULL,
                [registration_id] varchar(255) NOT NULL,
                [user_id] int CONSTRAINT FK_students_users FOREIGN KEY REFERENCES users([id]),
                [created_at] DATETIMEOFFSET NOT NULL CONSTRAINT DF_students_created_at DEFAULT (SYSDATETIMEOFFSET()), 
                [updated_at] DATETIMEOFFSET NULL
            );
            CREATE NONCLUSTERED INDEX IN_students_uuid ON [dbo].[students](uuid) ON [PRIMARY];
        END;
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'projects')
        BEGIN
            CREATE TABLE projects (
                [id] int IDENTITY(-2147483648, 1) NOT NULL CONSTRAINT PK_projects_id PRIMARY KEY CLUSTERED,
                [uuid] uniqueidentifier NOT NULL CONSTRAINT DF_projects_uuid DEFAULT NEWSEQUENTIALID(),
                [student_id] int NOT NULL CONSTRAINT FK_projects_students FOREIGN KEY REFERENCES students([id]) ON DELETE CASCADE,
                [name] varchar(255) NOT NULL
            );
            CREATE NONCLUSTERED INDEX IN_projects_uuid ON [dbo].[projects](uuid) ON [PRIMARY];
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
END;