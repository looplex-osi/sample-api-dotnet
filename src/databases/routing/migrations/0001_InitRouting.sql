USE [lawoffice]
GO

-- Migration name defined here
DECLARE @MigrationId NVARCHAR(255) = 'InitRouting'

IF NOT EXISTS (SELECT TOP 1 1 FROM lawoffice.MigrationHistory WHERE MigrationId = @MigrationId)
BEGIN
    PRINT 'Running migration [' + @MigrationId + ']';

    BEGIN TRY
        BEGIN TRANSACTION   
        
        -- Migration code starts here

        CREATE TABLE [lawoffice].[customers]
        (
            [id] int IDENTITY(-2147483648, 1) NOT NULL CONSTRAINT PK_customers_id PRIMARY KEY CLUSTERED,
            [uuid] uniqueidentifier NOT NULL CONSTRAINT DF_customers_uuid DEFAULT NEWSEQUENTIALID(),
            [domain] varchar(255) NOT NULL,
            [status] tinyint NOT NULL CONSTRAINT DF_customers_status DEFAULT 1,
            [custom_fields] varchar(2047) NOT NULL CONSTRAINT DF_customers_custom_fields DEFAULT '{}',
            [created_by] varchar(127) NOT NULL,
            [updated_by] varchar(127) NOT NULL,
            [created_at] datetime2 NOT NULL CONSTRAINT DF_customers_created_at DEFAULT SYSUTCDATETIME(),
            [updated_at] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
            [__valid_until__] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
            PERIOD FOR SYSTEM_TIME([updated_at], [__valid_until__])
        ) ON [PRIMARY]
          WITH
              (
              SYSTEM_VERSIONING = ON (HISTORY_TABLE = [lawoffice].[customersHistory])
        );
        CREATE NONCLUSTERED INDEX IN_customers_uuid ON [lawoffice].[customers](uuid) ON [PRIMARY];
        CREATE UNIQUE NONCLUSTERED INDEX IN_customers_domain ON [lawoffice].[customers](domain) ON [PRIMARY];
        
        CREATE TABLE [lawoffice].[databases]
        (
            [id] int IDENTITY(-2147483648, 1) NOT NULL CONSTRAINT PK_databases_id PRIMARY KEY CLUSTERED,
            [uuid] uniqueidentifier NOT NULL CONSTRAINT DF_databases_uuid DEFAULT NEWSEQUENTIALID(),
            [name] varchar(32) NOT NULL,
            [keyvault_id] varchar(255) NOT NULL,
            [status] tinyint NOT NULL CONSTRAINT DF_databases_status DEFAULT 1,
            [custom_fields] varchar(2047) NOT NULL CONSTRAINT DF_databases_custom_fields DEFAULT '{}',
            [created_by] varchar(32) NOT NULL,
            [updated_by] varchar(32) NOT NULL,
            [created_at] datetime2 NOT NULL CONSTRAINT DF_databases_created_at DEFAULT SYSUTCDATETIME(),
            [updated_at] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
            [__valid_until__] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
            PERIOD FOR SYSTEM_TIME ([updated_at], [__valid_until__])
        ) ON [PRIMARY]
          WITH (
              SYSTEM_VERSIONING = ON (HISTORY_TABLE = [lawoffice].[databasesHistory])
        );
        CREATE NONCLUSTERED INDEX IN_databases_uuid ON [lawoffice].[databases](uuid) ON [PRIMARY];
        
        CREATE TABLE [lawoffice].[customers_databases]
        (
            [customer_id] int NOT NULL CONSTRAINT FK_customers_databases_customer_id FOREIGN KEY REFERENCES [lawoffice].[customers](id) ON DELETE CASCADE,
            [database_id] int NOT NULL CONSTRAINT FK_customers_databases_database_id FOREIGN KEY REFERENCES [lawoffice].[databases](id) ON DELETE CASCADE,
            [created_by] varchar(32) NOT NULL,
            [updated_by] varchar(32) NOT NULL,
            [created_at] datetime2 NOT NULL CONSTRAINT DF_customers_databases_created_at DEFAULT SYSUTCDATETIME(),
            [updated_at] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
            [__valid_until__] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
            PERIOD FOR SYSTEM_TIME ([updated_at], [__valid_until__])
        ) ON [PRIMARY];
        
        -----------------------------------------
        
        CREATE TABLE [lawoffice].[subscription_plan]
        (
            [id] int IDENTITY(-2147483648, 1) NOT NULL CONSTRAINT PK_subscription_plan_id PRIMARY KEY CLUSTERED,
            [uuid] uniqueidentifier NOT NULL CONSTRAINT DF_subscription_plan_uuid DEFAULT NEWSEQUENTIALID(),
            [name] varchar(255) NOT NULL,
            [status] tinyint NOT NULL CONSTRAINT DF_subscription_plan_status DEFAULT 1,
            [credits] int NOT NULL,
            [custom_fields] varchar(2047) NOT NULL CONSTRAINT DF_subscription_plan_custom_fields DEFAULT '{}',
            [created_by] varchar(127) NOT NULL,
            [updated_by] varchar(127) NOT NULL,
            [created_at] datetime2 NOT NULL CONSTRAINT DF_subscription_plan_created_at DEFAULT SYSUTCDATETIME(),
            [updated_at] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
            [__valid_until__] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
            PERIOD FOR SYSTEM_TIME([updated_at], [__valid_until__])
        ) ON [PRIMARY]
          WITH
              (
              SYSTEM_VERSIONING = ON (HISTORY_TABLE = [lawoffice].[subscription_planHistory])
        );
        CREATE NONCLUSTERED INDEX IN_subscription_plan_uuid ON [lawoffice].[subscription_plan](uuid) ON [PRIMARY];
        
        CREATE TABLE [lawoffice].[transaction]
        (
            [id] int IDENTITY(-2147483648, 1) NOT NULL CONSTRAINT PK_transaction_id PRIMARY KEY CLUSTERED,
            [uuid] uniqueidentifier NOT NULL CONSTRAINT DF_transaction_uuid DEFAULT NEWSEQUENTIALID(),
            [customer_id] int NOT NULL CONSTRAINT FK_transaction_customer_id FOREIGN KEY REFERENCES [lawoffice].[customers](id),
            [subscription_plan_id] int NOT NULL CONSTRAINT FK_transaction_subscription_plan_id FOREIGN KEY REFERENCES [lawoffice].[subscription_plan](id),
            [hiring_date] datetime2 NOT NULL,
            [due_date] datetime2 NOT NULL,
            [credits] int NOT NULL,
            [custom_fields] varchar(2047) NOT NULL CONSTRAINT DF_transaction_custom_fields DEFAULT '{}',
            [created_by] varchar(127) NOT NULL,
            [updated_by] varchar(127) NOT NULL,
            [created_at] datetime2 NOT NULL CONSTRAINT DF_transaction_created_at DEFAULT SYSUTCDATETIME(),
            [updated_at] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
            [__valid_until__] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
            PERIOD FOR SYSTEM_TIME([updated_at], [__valid_until__])
        ) ON [PRIMARY]
          WITH
              (
              SYSTEM_VERSIONING = ON (HISTORY_TABLE = [lawoffice].[transactionHistory])
        );
        CREATE NONCLUSTERED INDEX IN_customers_uuid ON [lawoffice].[transaction](uuid) ON [PRIMARY];
    
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

