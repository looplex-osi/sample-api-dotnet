using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Samples.WebAPI.Extensions;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Looplex.OpenForExtension.Contexts;

namespace Looplex.DotNet.Samples.WebAPI.Factories;

public class ContextFactory(IServiceProvider serviceProvider) : IContextFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    
    public IContext Create(IEnumerable<string> services)
    {
        var plugins = PluginLoader.Load(/*services*/);
        return DefaultContext.Create(plugins, _serviceProvider);
    }
}