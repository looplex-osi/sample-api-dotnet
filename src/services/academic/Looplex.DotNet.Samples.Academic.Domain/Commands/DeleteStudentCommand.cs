using Looplex.DotNet.Core.Domain;

namespace Looplex.DotNet.Samples.Academic.Domain.Commands
{
    public class DeleteStudentCommand : ICommand
    {
        public required string Id { get; init; }
    }
}
