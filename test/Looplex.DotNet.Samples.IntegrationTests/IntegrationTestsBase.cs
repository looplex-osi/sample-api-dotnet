using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Core.Infra.Data;
using Microsoft.Extensions.Configuration;

namespace Looplex.DotNet.Samples.WebAPI.IntegrationTests;

public abstract class IntegrationTestsBase
{
    protected readonly IConfiguration Configuration 
        = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "DbServer", "localhost" },
                { "DbPort", "1434" },
                { "DbUser", "samplesUser" },
                { "Password", "!ooplex_D0tNet!" },
                { "Database", "dotnetsamples" }
            })
            .Build();


    private IDatabaseContext? _databaseContext;
    protected IDatabaseContext DatabaseContext
    {
        get 
        {
            if (_databaseContext ==  null)
            {
                _databaseContext = new DatabaseContext(Configuration);
            }
            return _databaseContext;
        }
    }
}