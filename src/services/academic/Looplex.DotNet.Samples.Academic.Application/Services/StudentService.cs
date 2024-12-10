using System;
using System.Linq;
using System.Threading;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using MediatR;
using System.Threading.Tasks;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.OpenForExtension.Abstractions.Commands;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Looplex.OpenForExtension.Abstractions.ExtensionMethods;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ScimPatch;

namespace Looplex.DotNet.Samples.Academic.Application.Services
{
    public class StudentService(
        IMediator mediator, 
        IConfiguration configuration,
        IJsonSchemaProvider jsonSchemaProvider) : IStudentService
    {
        private const string JsonSchemaIdForStudentKey = "JsonSchemaIdForStudent";

        public async Task GetAllAsync(IContext context, CancellationToken cancellationToken)
        {
            await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);
            
            await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

            if (!context.SkipDefaultAction)
            {
                var getStudentsQuery = new GetStudentsQuery()
                {
                    Context = (IScimV2Context)context,
                };
                var result = await mediator.Send(getStudentsQuery, cancellationToken);
                context.Result = JsonConvert.SerializeObject(result, Student.Converter.Settings);
            }

            await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
        }

        public async Task GetByIdAsync(IContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var scimContext = (IScimV2Context)context;
            var id = Guid.Parse((string)scimContext.RouteValues["StudentId"]!);
            await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);

            var getStudentByIdQuery = new GetStudentByIdQuery
            {
                Context = (IScimV2Context)context,
                UniqueId = id
            };
            var student = await mediator.Send(getStudentByIdQuery, cancellationToken);
            if (student == null)
            {
                throw new EntityNotFoundException(nameof(Student), id.ToString());
            }
            await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);

            context.Roles["Student"] = student;
            await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

            if (!context.SkipDefaultAction)
            {
                context.Result = ((Student)context.Roles["Student"]).ToJson();
            }

            await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
        }

        public async Task CreateAsync(IContext context, CancellationToken cancellationToken)
        {
            var json = context.GetRequiredValue<string>("Resource");
            var schemaId = configuration[JsonSchemaIdForStudentKey]!;
            var jsonSchema = await jsonSchemaProvider.ResolveJsonSchemaAsync(context, schemaId);
            var student = Resource.FromJson<Student>(json, jsonSchema, out var messages);
            await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);

            if (messages.Count > 0)
            {
                throw new EntityInvalidException(messages.ToList());
            }
            await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);
            
            context.Roles["Student"] = student;
            await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

            if (!context.SkipDefaultAction)
            {
                var createStudentCommand = new CreateStudentCommand
                {
                    Context = (IScimV2Context)context,
                    Student = context.Roles["Student"]
                };
                await mediator.Send(createStudentCommand, cancellationToken);
                context.Result = context.Roles["Student"].Id;
            }

            await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
        }

        public Task UpdateAsync(IContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task PatchAsync(IContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var json = context.GetRequiredValue<string>("Operations");
            await GetByIdAsync(context, cancellationToken);
            var student = ((Student)context.Roles["Student"])
                .WithObservableProxy();
            context.Roles["Student"] = student;
            var operations = OperationTracker.FromJson(student, json);
            await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);

            if (operations.Count == 0)
            {
                throw new InvalidOperationException("List of operations can't be empty.");
            }
            await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);

            context.Roles["Operations"] = operations;
            await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

            if (!context.SkipDefaultAction)
            {
                var schemaId = configuration[JsonSchemaIdForStudentKey]!;
                var jsonSchema = await jsonSchemaProvider.ResolveJsonSchemaAsync(context, schemaId);
                
                foreach (var operationNode in context.Roles["Operations"])
                {
                    if (!await operationNode.TryApplyAsync())
                    {
                        throw operationNode.OperationException;
                    }
                }
                json = student.ToJson();
                _ = Resource.FromJson<Student>(json, jsonSchema, out var messages);
                if (messages.Count > 0)
                {
                    throw new EntityInvalidException(messages.ToList());
                }
                var command = new UpdateStudentCommand
                {
                    Context = (IScimV2Context)context,
                    Student = student
                };
                await mediator.Send(command, cancellationToken);
            }

            await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
        }

        public async Task DeleteAsync(IContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var scimContext = (IScimV2Context)context;
            var id = Guid.Parse((string)scimContext.RouteValues["StudentId"]!);
            await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);

            await GetByIdAsync(context, cancellationToken);
            var student = (Student)context.Roles["Student"];
            if (student == null)
            {
                throw new EntityNotFoundException(nameof(Student), id.ToString());
            }
            await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

            if (!context.SkipDefaultAction)
            {
                var command = new DeleteStudentCommand
                {
                    Context = (IScimV2Context)context,
                    UniqueId = id
                };
                await mediator.Send(command, cancellationToken);
            }

            await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

            await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
        }
    }
}
