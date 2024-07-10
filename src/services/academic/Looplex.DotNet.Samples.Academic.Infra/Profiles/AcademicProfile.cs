using AutoMapper;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.DTOs;
using Looplex.DotNet.Samples.Academic.Domain.Entities;

namespace Looplex.DotNet.Samples.Academic.Infra.Profiles
{
    public class AcademicProfile : Profile
    {
        public AcademicProfile()
        {
            CreateMap<Student, StudentReadDTO>()
                .ReverseMap();
            CreateMap<SchoolSubject, SchoolSubjectDTO>()
                .ReverseMap();
        }
    }
}
