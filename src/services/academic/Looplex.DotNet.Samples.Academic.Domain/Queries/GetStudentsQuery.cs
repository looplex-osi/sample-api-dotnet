using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Samples.Academic.Domain.Queries
{
    public class GetStudentsQuery : IQuery<ListResponse>
    {
        public required IContext Context { get; init; }
    }
}
