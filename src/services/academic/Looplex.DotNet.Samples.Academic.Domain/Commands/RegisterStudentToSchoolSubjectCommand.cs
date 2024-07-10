using Looplex.DotNet.Core.Domain;

namespace Looplex.DotNet.Samples.Academic.Domain.Commands
{
    public class RegisterStudentToSchoolSubjectCommand : ICommand
    {
        public int SchoolSubjectId { get; set; }
        public int StudentId { get; set; }
    }
}
