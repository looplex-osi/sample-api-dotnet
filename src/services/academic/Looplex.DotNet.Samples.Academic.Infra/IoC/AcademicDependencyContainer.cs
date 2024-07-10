using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Samples.Academic.Infra.IoC
{
    public class AcademicDependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<ISchoolSubjectService, SchoolSubjectService>();
            services.AddScoped<IStudentService, StudentService>();
        }
    }
}