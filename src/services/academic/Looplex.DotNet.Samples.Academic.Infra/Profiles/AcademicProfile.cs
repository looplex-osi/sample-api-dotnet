using AutoMapper;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos.SchoolSubjects;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos.Users;
using Looplex.DotNet.Samples.Academic.Domain.Entities;
using Looplex.DotNet.Samples.Academic.Domain.Entities.SchoolSubjects;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

namespace Looplex.DotNet.Samples.Academic.Infra.Profiles
{
    public class AcademicProfile : Profile
    {
        public AcademicProfile()
        {
            CreateMap<Student, StudentReadDto>()
                .ReverseMap();
            CreateMap<SchoolSubject, SchoolSubjectDto>()
                .ReverseMap();
        }
    }
}
