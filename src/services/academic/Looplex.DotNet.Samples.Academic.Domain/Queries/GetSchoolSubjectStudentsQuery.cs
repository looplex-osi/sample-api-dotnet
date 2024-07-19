using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.OpenForExtension.Context;

namespace Looplex.DotNet.Samples.Academic.Domain.Queries
{
    public class GetSchoolSubjectStudentsQuery : IQuery<PaginatedCollection<Student>>
    {
        public int SchoolSubjectId { get; set; }
        public IDefaultContext Context { get; set; }
    }
}
