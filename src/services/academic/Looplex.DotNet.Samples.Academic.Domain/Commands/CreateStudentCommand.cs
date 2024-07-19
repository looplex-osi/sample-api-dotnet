using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

namespace Looplex.DotNet.Samples.Academic.Domain.Commands
{
    public class CreateStudentCommand : ICommand
    {
        public Student Student { get; set; }
    }
}
