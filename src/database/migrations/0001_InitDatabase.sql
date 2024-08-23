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
    
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
        BEGIN
            CREATE TABLE Users (
                Id INT PRIMARY KEY IDENTITY(1,1),
                [Name] VARCHAR(255) NOT NULL
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
END;