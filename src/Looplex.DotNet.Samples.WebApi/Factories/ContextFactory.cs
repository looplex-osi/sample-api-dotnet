using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.Abstractions.Providers;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Samples.WebApi.Extensions;
using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Samples.WebApi.Factories;

public class ContextFactory(
    IServiceProvider serviceProvider,
    ISqlDatabaseProvider sqlDatabaseProvider) : IContextFactory
{
    public IContext Create(IEnumerable<string> services)
    {
        var plugins = PluginLoader.Load(/*services*/); // TODO
        var context = new DefaultScimV2Context(serviceProvider, sqlDatabaseProvider);
        foreach (var plugin in plugins)
        {
            context.Plugins.Add(plugin);
        }
        return context;
    }
}