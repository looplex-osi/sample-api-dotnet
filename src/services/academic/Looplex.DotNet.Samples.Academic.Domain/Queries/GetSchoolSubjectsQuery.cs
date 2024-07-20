using Looplex.DotNet.Core.Domain;
using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Samples.Academic.Domain.Queries
{
    public class GetSchoolSubjectsQuery : IQuery<PaginatedCollection>
    {
        public required IContext Context { get; init; }
    }
}
