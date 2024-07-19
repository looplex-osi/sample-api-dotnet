using System.Threading;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using Looplex.OpenForExtension.Context;
using MediatR;
using System.Threading.Tasks;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.OpenForExtension.Commands;
using Looplex.OpenForExtension.ExtensionMethods;

namespace Looplex.DotNet.Samples.Academic.Application.Services
{
    public class StudentService(IMediator mediator) : IStudentService
    {
        public async Task GetAllAsync(IDefaultContext context, CancellationToken cancellationToken)
        {
            context.Plugins.Execute<IHandleInput>(context);

            context.Plugins.Execute<IValidateInput>(context);
            
            context.Plugins.Execute<IDefineActors>(context);

            context.Plugins.Execute<IBind>(context);

            context.Plugins.Execute<IBeforeAction>(context);

            if (!context.SkipDefaultAction)
            {
                var getStudentsQuery = new GetStudentsQuery()
                {
                    Context = context,
                };
                var result = await mediator.Send(getStudentsQuery);
                context.Result = result.ToJson(Student.Converter.Settings);
            }

            context.Plugins.Execute<IAfterAction>(context);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context);
        }

        public Task GetByIdAsync(IDefaultContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task CreateAsync(IDefaultContext context, CancellationToken cancellationToken)
        {
            var student = context.GetRequiredValue<Student>("Resource");
            context.Plugins.Execute<IHandleInput>(context);

            context.Plugins.Execute<IValidateInput>(context);
            
            context.Actors["Student"] = student;
            context.Plugins.Execute<IDefineActors>(context);

            context.Plugins.Execute<IBind>(context);

            context.Plugins.Execute<IBeforeAction>(context);

            if (!context.SkipDefaultAction)
            {
                var createStudentCommand = new CreateStudentCommand
                {
                    Student = context.Actors["Student"]
                };
                await mediator.Send(createStudentCommand);
                context.Result = context.Actors["Student"].Id;
            }

            context.Plugins.Execute<IAfterAction>(context);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context);
            
        }

        public Task DeleteAsync(IDefaultContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
