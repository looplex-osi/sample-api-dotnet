using System;
using System.Linq;
using System.Threading;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using MediatR;
using System.Threading.Tasks;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.OpenForExtension.Abstractions.Commands;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Looplex.OpenForExtension.Abstractions.ExtensionMethods;

namespace Looplex.DotNet.Samples.Academic.Application.Services
{
    public class StudentService(IMediator mediator) : IStudentService
    {
        private readonly IMediator _mediator = mediator;
        
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
                var result = await _mediator.Send(getStudentsQuery, cancellationToken);
                context.Result = result.ToJson(Student.Converter.Settings);
            }

            context.Plugins.Execute<IAfterAction>(context, cancellationToken);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context, cancellationToken);
        }

        public async Task GetByIdAsync(IContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var id = Guid.Parse(context.GetRequiredValue<string>("Id"));
            context.Plugins.Execute<IHandleInput>(context, cancellationToken);

            var getStudentByIdQuery = new GetStudentByIdQuery
            {
                Id = id.ToString()
            };
            var student = await _mediator.Send(getStudentByIdQuery, cancellationToken);
            if (student == null)
            {
                throw new EntityNotFoundException(nameof(Student), id.ToString());
            }
            context.Plugins.Execute<IValidateInput>(context, cancellationToken);

            context.Roles["Student"] = student;
            context.Plugins.Execute<IDefineRoles>(context, cancellationToken);

            context.Plugins.Execute<IBind>(context, cancellationToken);

            context.Plugins.Execute<IBeforeAction>(context, cancellationToken);

            if (!context.SkipDefaultAction)
            {
                context.Result = ((Student)context.Roles["Student"]).ToJson();
            }

            context.Plugins.Execute<IAfterAction>(context, cancellationToken);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context, cancellationToken);
        }

        public async Task CreateAsync(IContext context, CancellationToken cancellationToken)
        {
            var json = context.GetRequiredValue<string>("Resource");
            var student = Resource.FromJson<Student>(json, out var messages);
            context.Plugins.Execute<IHandleInput>(context, cancellationToken);

            if (messages.Count > 0)
            {
                throw new EntityInvalidException(messages.ToList());
            }
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
                await _mediator.Send(createStudentCommand, cancellationToken);
                context.Result = context.Roles["Student"].Id;
            }

            context.Plugins.Execute<IAfterAction>(context, cancellationToken);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context, cancellationToken);
        }

        public Task PatchAsync(IContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteAsync(IContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var id = Guid.Parse(context.GetRequiredValue<string>("Id"));
            context.Plugins.Execute<IHandleInput>(context, cancellationToken);

            await GetByIdAsync(context, cancellationToken);
            var student = (Student)context.Roles["Student"];
            if (student == null)
            {
                throw new EntityNotFoundException(nameof(Student), id.ToString());
            }
            context.Plugins.Execute<IValidateInput>(context, cancellationToken);

            context.Plugins.Execute<IDefineRoles>(context, cancellationToken);

            context.Plugins.Execute<IBind>(context, cancellationToken);

            context.Plugins.Execute<IBeforeAction>(context, cancellationToken);

            if (!context.SkipDefaultAction)
            {
                var command = new DeleteStudentCommand
                {
                    Id = id.ToString()
                };
                await _mediator.Send(command, cancellationToken);
            }

            context.Plugins.Execute<IAfterAction>(context, cancellationToken);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context, cancellationToken);
        }
    }
}
