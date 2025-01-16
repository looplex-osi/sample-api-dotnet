using Looplex.DotNet.Core.Application.Abstractions.Commands;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.CommandHandlers
{
    public class DeleteStudentCommandHandler() : ICommandHandler<DeleteStudentCommand>
    {
        public async Task Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = "DELETE FROM students WHERE uuid = @UniqueId";

            using var dbService = await request.Context.GetSqlDatabaseService();

            var count = await dbService.ExecuteAsync(query, new { request.UniqueId });

            if (count == 0) throw new EntityNotFoundException(nameof(Student), request.UniqueId.ToString());
        }
    }
}
