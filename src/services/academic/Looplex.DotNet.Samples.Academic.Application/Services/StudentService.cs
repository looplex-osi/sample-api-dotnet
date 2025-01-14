using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using MediatR;
using System.Threading.Tasks;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.OpenForExtensions;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ScimPatch;
using Looplex.DotNet.Middlewares.ScimV2.Services;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;

namespace Looplex.DotNet.Samples.Academic.Application.Services;

public class StudentService(
    IExtensionPointOrchestrator extensionPointOrchestrator,
    IMediator mediator, 
    IConfiguration configuration,
    IJsonSchemaProvider jsonSchemaProvider) : BaseCrudService(extensionPointOrchestrator), IStudentService
{
    private const string JsonSchemaIdForStudentKey = "JsonSchemaIdForStudent";
    protected override Task GetAllHandleInputAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task GetAllValidateInputAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task GetAllDefineRolesAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task GetAllBindAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;    }

    protected override Task GetAllBeforeActionAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;    }

    protected override async Task GetAllDefaultActionAsync(IContext context, CancellationToken cancellationToken)
    {
        var getStudentsQuery = new GetStudentsQuery()
        {
            Context = (IScimV2Context)context,
        };
        var result = await mediator.Send(getStudentsQuery, cancellationToken);
        context.Result = JsonConvert.SerializeObject(result, Student.Converter.Settings);
    }

    protected override Task GetAllAfterActionAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;    }

    protected override Task GetAllReleaseUnmanagedResourcesAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;    }

    protected override Task GetByIdHandleInputAsync(IContext context, CancellationToken cancellationToken)
    {
        context.State.StudentId = Guid.Parse(context.GetRequiredRouteValue<string>("StudentId"));
        return Task.CompletedTask;
    }

    protected override async Task GetByIdValidateInputAsync(IContext context, CancellationToken cancellationToken)
    {
        var id = context.GetRequiredValue<Guid>("StudentId");
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

        context.State.Student = student;
    }

    protected override Task GetByIdDefineRolesAsync(IContext context, CancellationToken cancellationToken)
    {
        var student = context.GetRequiredValue<Student>("Student");
        context.Roles["Student"] = student;
        return Task.CompletedTask;
    }

    protected override Task GetByIdBindAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task GetByIdBeforeActionAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task GetByIdDefaultActionAsync(IContext context, CancellationToken cancellationToken)
    {
        context.Result = ((Student)context.Roles["Student"]).ToJson();
        return Task.CompletedTask;
    }

    protected override Task GetByIdAfterActionAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task GetByIdReleaseUnmanagedResourcesAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override async Task CreateHandleInputAsync(IContext context, CancellationToken cancellationToken)
    {
        var json = context.GetRequiredValue<string>("Resource");
        var schemaId = configuration[JsonSchemaIdForStudentKey]!;
        var jsonSchema = await jsonSchemaProvider.ResolveJsonSchemaAsync(context, schemaId);
        var student = Resource.FromJson<Student>(json, jsonSchema, out var messages);
        context.State.Messages = messages;
        context.State.Student = student;
    }

    protected override Task CreateValidateInputAsync(IContext context, CancellationToken cancellationToken)
    {
        var messages = context.GetRequiredValue<IList<string>>("Messages");
        if (messages.Count > 0)
        {
            throw new EntityInvalidException(messages.ToList());
        }
        return Task.CompletedTask;
    }

    protected override Task CreateDefineRolesAsync(IContext context, CancellationToken cancellationToken)
    {
        var student = context.GetRequiredValue<Student>("Student");
        context.Roles["Student"] = student;
        return Task.CompletedTask;
    }

    protected override Task CreateBindAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task CreateBeforeActionAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override async Task CreateDefaultActionAsync(IContext context, CancellationToken cancellationToken)
    {
        var createStudentCommand = new CreateStudentCommand
        {
            Context = (IScimV2Context)context,
            Student = context.Roles["Student"]
        };
        await mediator.Send(createStudentCommand, cancellationToken);
        context.Result = context.Roles["Student"].Id;
    }

    protected override Task CreateAfterActionAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task CreateReleaseUnmanagedResourcesAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task UpdateHandleInputAsync(IContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task UpdateValidateInputAsync(IContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task UpdateDefineRolesAsync(IContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task UpdateBindAsync(IContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task UpdateBeforeActionAsync(IContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task UpdateDefaultActionAsync(IContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task UpdateAfterActionAsync(IContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task UpdateReleaseUnmanagedResourcesAsync(IContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override async Task PatchHandleInputAsync(IContext context, CancellationToken cancellationToken)
    {
        var json = context.GetRequiredValue<string>("Operations");
        await GetByIdAsync(context, cancellationToken);
        var student = ((Student)context.Roles["Student"])
            .WithObservableProxy();
        context.Roles["Student"] = student;
        var operations = OperationTracker.FromJson(student, json);
        context.State.Operations = operations;
    }

    protected override Task PatchValidateInputAsync(IContext context, CancellationToken cancellationToken)
    {        
        var operations = context.GetRequiredValue<IList<OperationNode>>("Operations");
        if (operations.Count == 0)
        {
            throw new InvalidOperationException("List of operations can't be empty.");
        }
        return Task.CompletedTask;
    }

    protected override Task PatchDefineRolesAsync(IContext context, CancellationToken cancellationToken)
    {
        var operations = context.GetRequiredValue<IList<OperationNode>>("Operations");
        context.Roles["Operations"] = operations;
        return Task.CompletedTask;
    }

    protected override Task PatchBindAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task PatchBeforeActionAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override async Task PatchDefaultActionAsync(IContext context, CancellationToken cancellationToken)
    {
        var schemaId = configuration[JsonSchemaIdForStudentKey]!;
        var jsonSchema = await jsonSchemaProvider.ResolveJsonSchemaAsync(context, schemaId);
        var student = (Student)context.Roles["Student"];
        var operations = (IList<OperationNode>)context.Roles["Operations"];

        foreach (var operationNode in operations)
        {
            if (!await operationNode.TryApplyAsync())
            {
                throw operationNode.OperationException;
            }
        }
        var json = student.ToJson();
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

    protected override Task PatchAfterActionAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task PatchReleaseUnmanagedResourcesAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task DeleteHandleInputAsync(IContext context, CancellationToken cancellationToken)
    {
        context.State.StudentId = Guid.Parse(context.GetRequiredRouteValue<string>("StudentId"));
        return Task.CompletedTask;
    }

    protected override async Task DeleteValidateInputAsync(IContext context, CancellationToken cancellationToken)
    {
        var id = context.GetRequiredValue<Guid>("StudentId");
        await GetByIdAsync(context, cancellationToken);
        var student = (Student)context.Roles["Student"];
        if (student == null)
        {
            throw new EntityNotFoundException(nameof(Student), id.ToString());
        }
    }

    protected override Task DeleteDefineRolesAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task DeleteBindAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task DeleteBeforeActionAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override async Task DeleteDefaultActionAsync(IContext context, CancellationToken cancellationToken)
    {
        var id = context.GetRequiredValue<Guid>("StudentId");
        var command = new DeleteStudentCommand
        {
            Context = (IScimV2Context)context,
            UniqueId = id
        };
        await mediator.Send(command, cancellationToken);
    }

    protected override Task DeleteAfterActionAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task DeleteReleaseUnmanagedResourcesAsync(IContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}