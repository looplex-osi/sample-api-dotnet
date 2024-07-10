using Looplex.DotNet.Core.Application.Abstractions.Pagination;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.DTOs;
using Looplex.OpenForExtension.Context;
using System.Threading.Tasks;

namespace Looplex.DotNet.Samples.Academic.Application.Abstractions.Services
{
    public interface ISchoolSubjectService
    {
        Task<PaginatedCollectionDTO<SchoolSubjectDTO>> GetSchoolSubjectsAsync(IDefaultContext context);
        Task<int> CreateSchoolSubjectAsync(SchoolSubjectWriteDTO schoolSubjectWriteDTO);
        Task<PaginatedCollectionDTO<StudentReadDTO>> GetStudentsAsync(IDefaultContext context, int schoolSubjectId);
    }
}
