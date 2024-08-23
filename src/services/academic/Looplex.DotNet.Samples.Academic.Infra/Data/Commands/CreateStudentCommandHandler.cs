using Looplex.DotNet.Core.Application.Abstractions.Commands;
using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Samples.Academic.Domain.Commands;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.Commands
{
    public class CreateStudentCommandHandler : ICommandHandler<CreateStudentCommand>
    {
        private readonly IDatabaseContext _context;

        public CreateStudentCommandHandler(IDatabaseContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(request.Student.Id)) request.Student.Id = Guid.NewGuid().ToString();
            
            var query = "insert into students (id, registrationid, userid) values (@Id, @RegistrationId, @UserId)";

            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                query,
                new { request.Student.Id, request.Student.RegistrationId, request.Student.UserId });
        }
    }
}
