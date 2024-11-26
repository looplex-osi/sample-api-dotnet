using Looplex.DotNet.Core.Application.Abstractions.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Looplex.DotNet.Samples.WebAPI.Caching;

public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    public Task SetCacheValueAsync(string key, string value, TimeSpan? expiry = null)
    {
        var cacheOptions = new MemoryCacheEntryOptions();

        if (expiry.HasValue)
        {
            cacheOptions.SetAbsoluteExpiration(expiry.Value);
        }

        memoryCache.Set(key, value, cacheOptions);

        return Task.CompletedTask;
    }

    public Task<bool> ContainsKeyAsync(string key)
    {
        return Task.FromResult(memoryCache.TryGetValue(key, out _));
    }

    public Task<bool> TryGetCacheValueAsync(string key, out string? value)
    {
        if (memoryCache.TryGetValue(key, out var cachedValue))
        {
            value = cachedValue as string;
            return Task.FromResult(true);
        }

        value = null;
        return Task.FromResult(false);
    }

    public Task RemoveCacheValueAsync(string key)
    {
        memoryCache.Remove(key);
        return Task.CompletedTask;
    }
}