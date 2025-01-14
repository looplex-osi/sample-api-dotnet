using System.Reflection;
using Casbin;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Infra.IoC;
using Looplex.DotNet.Samples.WebAPI.Factories;
using Looplex.DotNet.Samples.WebAPI.Routes.Academic;
using Looplex.DotNet.Samples.WebAPI.Services;
using Looplex.DotNet.WebApi;

namespace Looplex.DotNet.Samples.WebAPI
{
    public class Program : BaseProgram
    {
        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        protected override void UseRoutes(IEndpointRouteBuilder app)
        {
            var schemaIdStudent = Configuration["JsonSchemaIdForStudent"]!;
            app.UseStudentRoutesAsync(schemaIdStudent, CancellationToken.None).GetAwaiter().GetResult();
        }

        protected override void AddSecretsService(IServiceCollection services)
        {
            services.AddSingleton<ISecretsService, InMemorySecretsService>();
        }

        protected override void AddServices(IServiceCollection services)
        {
            services.AddAcademicServices();
        }

        protected override void AddFactories(IServiceCollection services)
        {
            services.AddTransient<IContextFactory, ContextFactory>();
        }

        protected override IEnforcer InitRbacEnforcer()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                ?? throw new InvalidOperationException("Could not determine directory");
            var modelPath = Path.Combine(dir, "model.conf");
            var policyPath = Path.Combine(dir, "policy.csv");

            if (!File.Exists(modelPath) || !File.Exists(policyPath))
            {
                throw new FileNotFoundException("Required Casbin configuration files are missing");
            }

            return new Enforcer(modelPath, policyPath);
        }
    }
}