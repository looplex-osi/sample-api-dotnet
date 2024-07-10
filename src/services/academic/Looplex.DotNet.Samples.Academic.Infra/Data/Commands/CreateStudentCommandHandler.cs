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

            var query = "insert into students (registrationid, userid) output inserted.id values (@RegistrationId, @UserId)";

            using var connection = _context.CreateConnection();

            var id = await connection.QueryFirstOrDefaultAsync<int>(query, new { request.Student.RegistrationId, request.Student.UserId });
            request.Student.Id = id.ToString();
        }
    }
}
