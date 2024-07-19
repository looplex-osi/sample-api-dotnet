using Looplex.OpenForExtension.Context;
using System.Threading.Tasks;
using Looplex.DotNet.Core.Application.Abstractions.Dtos;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos.SchoolSubjects;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos.Users;

namespace Looplex.DotNet.Samples.Academic.Application.Abstractions.Services
{
    public interface ISchoolSubjectService
    {
        Task<PaginatedCollectionDto<SchoolSubjectDto>> GetSchoolSubjectsAsync(IDefaultContext context);
        Task<int> CreateSchoolSubjectAsync(SchoolSubjectWriteDto schoolSubjectWriteDto);
        Task<PaginatedCollectionDto<StudentReadDto>> GetStudentsAsync(IDefaultContext context, int schoolSubjectId);
    }
}
