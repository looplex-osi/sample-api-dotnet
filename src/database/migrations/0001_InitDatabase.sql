USE [dotnetsamples]
GO

-- Migration name defined here
DECLARE @MigrationId NVARCHAR(255) = 'InitUsersModule'

IF NOT EXISTS (SELECT TOP 1 1 FROM MigrationHistory WHERE MigrationId = @MigrationId)
BEGIN
    PRINT 'Running migration [' + @MigrationId + ']';

    BEGIN TRY
        BEGIN TRANSACTION   
    
        -- Migration code starts here
    
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'users')
        BEGIN
            CREATE TABLE users (
                [id] int IDENTITY(-2147483648, 1) NOT NULL CONSTRAINT PK_users_id PRIMARY KEY CLUSTERED,
                [uuid] uniqueidentifier NOT NULL CONSTRAINT DF_users_uuid DEFAULT NEWSEQUENTIALID(),
                [name] VARCHAR(255) NOT NULL
            );
            CREATE NONCLUSTERED INDEX IN_users_uuid ON [dbo].[users](uuid) ON [PRIMARY];
        END;
    
        -- TODO finish tables Users / Groups
        
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