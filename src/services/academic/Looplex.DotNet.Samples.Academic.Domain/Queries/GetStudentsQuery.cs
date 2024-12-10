using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;

namespace Looplex.DotNet.Samples.Academic.Domain.Queries
{
    public class GetStudentsQuery : IQuery<ListResponse>
    {
        public required IScimV2Context Context { get; init; } 
    }
}
