using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Application.Services;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.WebApi.Routes;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Samples.Academic.Infra.WebApi
{
    public static class AcademicRoutes
    {
        public static async Task UseAcademicRoutesAsync(this IEndpointRouteBuilder app, string schemaIdStudent, CancellationToken cancellationToken)
        {
            await app.UseScimV2RoutesAsync<Student, IStudentService>(
                "Students",
                schemaIdStudent,
                DefaultScimV2RouteOptions.CreateFor<StudentService>(),
                cancellationToken);
        }
    }
}