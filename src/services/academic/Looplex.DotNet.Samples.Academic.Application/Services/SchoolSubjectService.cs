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
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos.SchoolSubjects;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos.Users;
using Looplex.DotNet.Samples.Academic.Domain.Entities.SchoolSubjects;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

namespace Looplex.DotNet.Samples.Academic.Application.Services
{
    public class SchoolSubjectService : ISchoolSubjectService
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public SchoolSubjectService(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<PaginatedCollectionDto<SchoolSubjectDto>> GetSchoolSubjectsAsync(IDefaultContext context)
        {
            var getSchoolSubjectsQuery = new GetSchoolSubjectsQuery()
            {
                Context = context,
            };
            var schoolSubjects = await _mediator.Send(getSchoolSubjectsQuery);
            return _mapper.Map<PaginatedCollection<SchoolSubject>, PaginatedCollectionDto<SchoolSubjectDto>>(schoolSubjects);
        }

        public async Task<PaginatedCollectionDto<StudentReadDto>> GetStudentsAsync(IDefaultContext context, int schoolSubjectId)
        {
            var getSchoolSubjectStudentsQuery = new GetSchoolSubjectStudentsQuery()
            {
                Context = context,
                SchoolSubjectId = schoolSubjectId
            };
            var students = await _mediator.Send(getSchoolSubjectStudentsQuery);
            return _mapper.Map<PaginatedCollection<Student>, PaginatedCollectionDto<StudentReadDto>>(students);
        }

        public async Task<int> CreateSchoolSubjectAsync(SchoolSubjectWriteDto schoolSubjectWriteDto)
        {
            var createSchoolSubjectCommand = new CreateSchoolSubjectCommand
            {
                SchoolSubject = new SchoolSubject
                {
                    Name = schoolSubjectWriteDto.Name!,
                }
            };
            await _mediator.Send(createSchoolSubjectCommand);
            return createSchoolSubjectCommand.SchoolSubject.Id;
        }
    }
}
