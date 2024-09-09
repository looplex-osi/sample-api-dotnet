using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Samples.Academic.Infra.IoC
{
    public static class AcademicDependencyContainer
    {
        public static void AddAcademicServices(this IServiceCollection services)
        {
            services.AddScoped<IStudentService, StudentService>();

            RegisterMediatR(services);
        }

        private static void RegisterMediatR(IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AcademicDependencyContainer).Assembly));
        }
    }
}