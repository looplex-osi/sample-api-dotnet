using Looplex.DotNet.Core.Application.Abstractions.Commands;
using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Samples.Academic.Domain.Commands;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.CommandHandlers
{
    public class CreateStudentCommandHandler(IDatabaseContext context) : ICommandHandler<CreateStudentCommand>
    {
        public async Task Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = @"
                insert into students (external_id, registration_id, user_id) 
                output inserted.id, inserted.uuid, inserted.created_at
                values (@ExternalId, @RegistrationId, @UserId)";

            using var connection = context.CreateConnection();
            connection.Open();
            await using var transaction = connection.BeginTransaction();
            
            var (id, uniqueId, createdAt) = await connection.QueryFirstOrDefaultAsync<(int, Guid, DateTimeOffset)>(
                query,
                new
                {
                    request.Student.ExternalId,
                    request.Student.RegistrationId,
                    request.Student.UserId
                },
                transaction);

            request.Student.Id = id;
            request.Student.UniqueId = uniqueId;
            request.Student.Meta = new Meta
            {
                Created = createdAt
            };
            foreach (var project in request.Student.Projects)
            {
                query = @"
                    insert into projects (student_id, name)
                    output inserted.id, inserted.uuid
                    values (@StudentId, @Name)";
                
                var ids = await connection.QueryFirstOrDefaultAsync<(int, Guid)>(
                    query,
                    new { StudentId = id , project.Name },
                    transaction);
                
                project.Id = ids.Item1;
                project.UniqueId = ids.Item2;
                project.StudentId = id;
            }
            
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
