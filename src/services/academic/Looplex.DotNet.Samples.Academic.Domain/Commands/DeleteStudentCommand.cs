using System;
using Looplex.DotNet.Core.Domain;

namespace Looplex.DotNet.Samples.Academic.Domain.Commands
{
    public class DeleteStudentCommand : ICommand
    {
        public required Guid UniqueId { get; init; }
    }
}
