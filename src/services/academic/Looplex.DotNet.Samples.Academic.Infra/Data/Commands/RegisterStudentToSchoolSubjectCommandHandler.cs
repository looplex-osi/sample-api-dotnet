using Looplex.DotNet.Core.Application.Abstractions.Commands;
using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Samples.Academic.Domain.Commands;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.Commands
{
    public class RegisterStudentToSchoolSubjectCommandHandler : ICommandHandler<RegisterStudentToSchoolSubjectCommand>
    {
        private readonly IDatabaseContext _context;

        public RegisterStudentToSchoolSubjectCommandHandler(IDatabaseContext context)
        {
            _context = context;
        }

        public async Task Handle(RegisterStudentToSchoolSubjectCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = "insert into schoolsubjects_students (schoolsubjectid, studentid, isactive) values (@SchoolSubjectId, @StudentId, 1)";

            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(query, new { request.SchoolSubjectId, request.StudentId });
        }
    }
}
