using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Application.Services;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.WebApi.Routes;

namespace Looplex.DotNet.Samples.WebAPI.Routes.Academic
{
    public static class StudentsRoutes
    {
        public static Task UseStudentRoutesAsync(this IEndpointRouteBuilder app, string schemaIdStudent, CancellationToken cancellationToken)
        {
            return app.UseScimV2RoutesAsync<Student, IStudentService>(
                "students",
                schemaIdStudent,
                DefaultScimV2RouteOptions.CreateFor<StudentService>(),
                cancellationToken);
        }
    }
}