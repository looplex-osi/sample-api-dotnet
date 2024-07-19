using Looplex.DotNet.Samples.Academic.Application.Abstractions.DTOs;
using Looplex.OpenForExtension.Context;
using System.Threading.Tasks;
using Looplex.DotNet.Core.Application.Abstractions.Dtos;

namespace Looplex.DotNet.Samples.Academic.Application.Abstractions.Services
{
    public interface ISchoolSubjectService
    {
        Task<PaginatedCollectionDto<SchoolSubjectDTO>> GetSchoolSubjectsAsync(IDefaultContext context);
        Task<int> CreateSchoolSubjectAsync(SchoolSubjectWriteDTO schoolSubjectWriteDTO);
        Task<PaginatedCollectionDto<StudentReadDTO>> GetStudentsAsync(IDefaultContext context, int schoolSubjectId);
    }
}
