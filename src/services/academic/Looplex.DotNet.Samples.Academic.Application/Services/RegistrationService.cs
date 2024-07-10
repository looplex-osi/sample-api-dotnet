using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using MediatR;
using System.Threading.Tasks;

namespace Looplex.DotNet.Samples.Academic.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IMediator _mediator;

        public RegistrationService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task RegisterUserToSchoolSubjectAsync(int studentId, int schoolSubjectId)
        {
            var registerStudentToSchoolSubjectCommand = new RegisterStudentToSchoolSubjectCommand
            {
                StudentId = studentId,
                SchoolSubjectId = schoolSubjectId
            };
            return _mediator.Send(registerStudentToSchoolSubjectCommand);
        }
    }
}
