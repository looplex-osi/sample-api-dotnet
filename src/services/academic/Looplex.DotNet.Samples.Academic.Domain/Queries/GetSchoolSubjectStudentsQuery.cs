using Looplex.DotNet.Core.Domain;
using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Samples.Academic.Domain.Queries
{
    public class GetSchoolSubjectStudentsQuery : IQuery<PaginatedCollection>
    {
        public required int SchoolSubjectId { get; set; }
        public required IContext Context { get; init; }
    }
}
