using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities;
using Looplex.OpenForExtension.Context;

namespace Looplex.DotNet.Samples.Academic.Domain.Queries
{
    public class GetSchoolSubjectsQuery : IQuery<PaginatedCollection<SchoolSubject>>
    {
        public IDefaultContext Context { get; set; }
    }
}
