using Looplex.DotNet.Core.Application.Abstractions.Services;

namespace Looplex.DotNet.Samples.WebAPI.Services;

/// <summary>
/// In memory secrets service while implementation using keyvault is not finished
/// </summary>
public class InMemorySecretsService : ISecretsService
{
    private static readonly Dictionary<string, string> Secrets = new()
    {
        { "sampleapi", "Server=looplex.dotnet.samples.academic.db, 1433;Database=dotnetsamples;User Id=samplesUser;Password=!ooplex_D0tNet!;TrustServerCertificate=True" }
    };
    
    public Task<string?> GetSecretAsync(string secretName)
    {
        Secrets.TryGetValue(secretName, out var secretValue);
        return Task.FromResult(secretValue);
    }
}