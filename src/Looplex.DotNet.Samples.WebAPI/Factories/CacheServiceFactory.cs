using System.Collections.Concurrent;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.Abstractions.Services;

namespace Looplex.DotNet.Samples.WebAPI.Factories;

public class CacheServiceFactory : ICacheServiceFactory
{
    private readonly ConcurrentDictionary<string, ICacheService> _providers = new();

    public void RegisterCacheService(string name, ICacheService cacheService)
    {
        _providers[name] = cacheService;
    }

    public ICacheService GetCacheService(string name)
    {
        if (_providers.TryGetValue(name, out var provider))
        {
            return provider;
        }

        throw new ArgumentException($"No cache service registered for {name}");    }
}