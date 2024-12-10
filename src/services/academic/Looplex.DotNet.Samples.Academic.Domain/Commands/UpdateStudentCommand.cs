using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

namespace Looplex.DotNet.Samples.Academic.Domain.Commands
{
    public class UpdateStudentCommand : ICommand
    {
        public required IScimV2Context Context { get; init; } 
        public required Student Student { get; init; }
    }
}
