using AutoMapper;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.DTOs;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Entities;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using Looplex.OpenForExtension.Context;
using MediatR;
using System.Threading.Tasks;
using Looplex.DotNet.Core.Application.Abstractions.Dtos;

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

        public async Task<PaginatedCollectionDto<SchoolSubjectDTO>> GetSchoolSubjectsAsync(IDefaultContext context)
        {
            var getSchoolSubjectsQuery = new GetSchoolSubjectsQuery()
            {
                Context = context,
            };
            var schoolSubjects = await _mediator.Send(getSchoolSubjectsQuery);
            return _mapper.Map<PaginatedCollection<SchoolSubject>, PaginatedCollectionDto<SchoolSubjectDTO>>(schoolSubjects);
        }

        public async Task<PaginatedCollectionDto<StudentReadDTO>> GetStudentsAsync(IDefaultContext context, int schoolSubjectId)
        {
            var getSchoolSubjectStudentsQuery = new GetSchoolSubjectStudentsQuery()
            {
                Context = context,
                SchoolSubjectId = schoolSubjectId
            };
            var students = await _mediator.Send(getSchoolSubjectStudentsQuery);
            return _mapper.Map<PaginatedCollection<Student>, PaginatedCollectionDto<StudentReadDTO>>(students);
        }

        public async Task<int> CreateSchoolSubjectAsync(SchoolSubjectWriteDTO schoolSubjectWriteDTO)
        {
            var createSchoolSubjectCommand = new CreateSchoolSubjectCommand
            {
                SchoolSubject = new SchoolSubject
                {
                    Name = schoolSubjectWriteDTO.Name,
                }
            };
            await _mediator.Send(createSchoolSubjectCommand);
            return createSchoolSubjectCommand.SchoolSubject.Id;
        }
    }
}
