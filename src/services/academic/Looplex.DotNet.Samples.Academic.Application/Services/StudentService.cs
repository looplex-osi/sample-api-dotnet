using System.Threading;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using MediatR;
using System.Threading.Tasks;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.OpenForExtension.Abstractions.Commands;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Looplex.OpenForExtension.Abstractions.ExtensionMethods;

namespace Looplex.DotNet.Samples.Academic.Application.Services
{
    public class StudentService(IMediator mediator) : IStudentService
    {
        public async Task GetAllAsync(IContext context, CancellationToken cancellationToken)
        {
            context.Plugins.Execute<IHandleInput>(context, cancellationToken);

            context.Plugins.Execute<IValidateInput>(context, cancellationToken);
            
            context.Plugins.Execute<IDefineRoles>(context, cancellationToken);

            context.Plugins.Execute<IBind>(context, cancellationToken);

            context.Plugins.Execute<IBeforeAction>(context, cancellationToken);

            if (!context.SkipDefaultAction)
            {
                var getStudentsQuery = new GetStudentsQuery()
                {
                    Context = context,
                };
                var result = await mediator.Send(getStudentsQuery);
                context.Result = result.ToJson(Student.Converter.Settings);
            }

            context.Plugins.Execute<IAfterAction>(context, cancellationToken);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context, cancellationToken);
        }

        public Task GetByIdAsync(IContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task CreateAsync(IContext context, CancellationToken cancellationToken)
        {
            var student = context.GetRequiredValue<Student>("Resource");
            context.Plugins.Execute<IHandleInput>(context, cancellationToken);

            context.Plugins.Execute<IValidateInput>(context, cancellationToken);
            
            context.Roles["Student"] = student;
            context.Plugins.Execute<IDefineRoles>(context, cancellationToken);

            context.Plugins.Execute<IBind>(context, cancellationToken);

            context.Plugins.Execute<IBeforeAction>(context, cancellationToken);

            if (!context.SkipDefaultAction)
            {
                var createStudentCommand = new CreateStudentCommand
                {
                    Student = context.Roles["Student"]
                };
                await mediator.Send(createStudentCommand);
                context.Result = context.Roles["Student"].Id;
            }

            context.Plugins.Execute<IAfterAction>(context, cancellationToken);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context, cancellationToken);
            
        }

        public Task DeleteAsync(IContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
