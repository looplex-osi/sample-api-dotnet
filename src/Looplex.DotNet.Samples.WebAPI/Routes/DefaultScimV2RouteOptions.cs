using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

namespace Looplex.DotNet.Samples.WebAPI.Routes;

public class DefaultScimV2RouteOptions
{
    public static ScimV2RouteOptions CreateFor<TService>() where TService : class, ICrudService
    {
        var service = nameof(TService);
        return new ScimV2RouteOptions
        {
            ServicesForGet = [$"{service}.{nameof(ICrudService.GetAllAsync)}"],
            ServicesForGetById = [$"{service}.{nameof(ICrudService.GetByIdAsync)}"],
            ServicesForPost = [$"{service}.{nameof(ICrudService.CreateAsync)}"],
            ServicesForDelete = [$"{service}.{nameof(ICrudService.DeleteAsync)}"]
        };
    }
}