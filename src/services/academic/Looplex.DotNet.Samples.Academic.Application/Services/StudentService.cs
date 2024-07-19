using AutoMapper;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Entities;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using Looplex.OpenForExtension.Context;
using MediatR;
using System.Threading.Tasks;
using Looplex.DotNet.Core.Application.Abstractions.Dtos;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos.Users;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.OpenForExtension.Commands;
using Looplex.OpenForExtension.ExtensionMethods;

namespace Looplex.DotNet.Samples.Academic.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public StudentService(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task GetAllAsync(IDefaultContext context)
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
                var students = await _mediator.Send(getStudentsQuery);
                context.Result = _mapper.Map<PaginatedCollection<Student>, PaginatedCollectionDto<StudentReadDto>>(students);;
            }

            context.Plugins.Execute<IAfterAction>(context);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context);
        }

        public Task GetByIdAsync(IDefaultContext context)
        {
            throw new System.NotImplementedException();
        }

        public async Task CreateAsync(IDefaultContext context)
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
                await _mediator.Send(createStudentCommand);
                context.Result = context.Actors["Student"].Id;
            }

            context.Plugins.Execute<IAfterAction>(context);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context);
            
        }

        public Task DeleteAsync(IDefaultContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
