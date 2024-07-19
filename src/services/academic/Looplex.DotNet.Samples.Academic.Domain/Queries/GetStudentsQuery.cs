using Looplex.DotNet.Core.Domain;
using Looplex.OpenForExtension.Context;

namespace Looplex.DotNet.Samples.Academic.Domain.Queries
{
    public class GetStudentsQuery : IQuery<PaginatedCollection>
    {
        public required IDefaultContext Context { get; init; }
    }
}
