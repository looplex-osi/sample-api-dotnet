using Looplex.DotNet.Core.WebAPI.Factories;
using Looplex.DotNet.Samples.WebAPI.Extensions;
using Looplex.OpenForExtension.Context;

namespace Looplex.DotNet.Samples.WebAPI.Factories;

public class ContextFactory(IServiceProvider serviceProvider) : IContextFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    
    public IDefaultContext Create(IEnumerable<string> services)
    {
        var plugins = PluginLoader.Load(/*services*/);
        return DefaultContext.Create(plugins, _serviceProvider);
    }
}