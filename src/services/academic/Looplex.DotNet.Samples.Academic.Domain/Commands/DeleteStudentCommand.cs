using System;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain;

namespace Looplex.DotNet.Samples.Academic.Domain.Commands
{
    public class DeleteStudentCommand : ICommand
    {
        public required IScimV2Context Context { get; init; } 
        public required Guid UniqueId { get; init; }
    }
}
