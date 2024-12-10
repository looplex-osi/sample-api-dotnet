using System;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

namespace Looplex.DotNet.Samples.Academic.Domain.Queries
{
    public class GetStudentByIdQuery : IQuery<Student>
    {
        public required IScimV2Context Context { get; init; } 
        public required Guid UniqueId { get; init; }
    }
}
