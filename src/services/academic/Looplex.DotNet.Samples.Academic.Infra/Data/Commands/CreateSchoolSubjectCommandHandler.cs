using Looplex.DotNet.Core.Application.Abstractions.Commands;
using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Samples.Academic.Domain.Commands;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.Commands
{
    public class CreateSchoolSubjectCommandHandler : ICommandHandler<CreateSchoolSubjectCommand>
    {
        private readonly IDatabaseContext _context;

        public CreateSchoolSubjectCommandHandler(IDatabaseContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateSchoolSubjectCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = "insert into schoolsubjects (name) output inserted.id values (@Name)";

            using var connection = _context.CreateConnection();

            var id = await connection.QueryFirstOrDefaultAsync<int>(query, new { request.SchoolSubject.Name });
            request.SchoolSubject.Id = id;
        }
    }
}
