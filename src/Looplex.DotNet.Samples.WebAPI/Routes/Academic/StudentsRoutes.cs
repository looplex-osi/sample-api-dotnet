using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Looplex.DotNet.Samples.Academic.Application.Services;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

namespace Looplex.DotNet.Samples.WebAPI.Routes.Academic
{
    public static class StudentsRoutes
    {
        public static void UseStudentRoutes(this IEndpointRouteBuilder app)
        {
            app.UseScimV2Routes<IStudentService>(
                "students",
                DefaultScimV2RouteOptions.CreateFor<StudentService>());
        }
    }
}