using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities;
using Looplex.DotNet.Samples.Academic.Domain.Entities.SchoolSubjects;

namespace Looplex.DotNet.Samples.Academic.Domain.Commands
{
    public class CreateSchoolSubjectCommand : ICommand
    {
        public SchoolSubject SchoolSubject { get; set; }
    }
}
