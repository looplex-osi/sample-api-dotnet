using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Services.SqlDatabases;
using Microsoft.Data.SqlClient;

namespace Looplex.DotNet.Samples.IntegrationTests;

public abstract class IntegrationTestsBase
{
    private ISqlDatabaseService? _sqlDatabaseService;
    protected ISqlDatabaseService SqlDatabaseService
    {
        get 
        {
            if (_sqlDatabaseService == null)
            {
                var connectionString = "Server=localhost,1434;Database=dotnetsamples;User Id=samplesUser;Password=!ooplex_D0tNet!;";
                var connection = new SqlConnection(connectionString);
                _sqlDatabaseService = new SqlDatabaseService(connection);
            }
            return _sqlDatabaseService;
        }
    }
}