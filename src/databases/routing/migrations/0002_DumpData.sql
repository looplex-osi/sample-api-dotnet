USE [lawoffice]
GO

-- Migration name defined here
DECLARE @MigrationId NVARCHAR(255) = 'DumpData'

IF NOT EXISTS (SELECT TOP 1 1 FROM lawoffice.MigrationHistory WHERE MigrationId = @MigrationId)
BEGIN
    PRINT 'Running migration [' + @MigrationId + ']';

    BEGIN TRY
        BEGIN TRANSACTION   
        
        -- Migration code starts here

        DECLARE @CustomerId INT;
        INSERT INTO [lawoffice].[customers]
        ([domain], [status], [created_by], [updated_by])
        VALUES
            ('sampleapi', 1, 'system', 'system');
        
        SET @CustomerId = SCOPE_IDENTITY();
        
        DECLARE @DatabaseId INT;
        INSERT INTO [lawoffice].[databases]
        ([name], [keyvault_id], [status], [created_by], [updated_by])
        VALUES
            ('academic', 'sampleapi', 1, 'system', 'system');
        
        SET @DatabaseId = SCOPE_IDENTITY(); 
        
        INSERT INTO [lawoffice].[customers_databases]
        ([customer_id], [database_id], [created_by], [updated_by])
        VALUES
            (@CustomerId, @DatabaseId, 'system', 'system');
    
        -- Migration code ends here
    
        INSERT INTO lawoffice.MigrationHistory (MigrationId, AppliedOn)
        VALUES (@MigrationId, GETDATE());
        
        COMMIT;
    END TRY
    BEGIN CATCH
    ROLLBACK;
        
        PRINT 'An error occurred when running migration [' + @MigrationId + ']: ' + ERROR_MESSAGE();
    END CATCH;
END;

