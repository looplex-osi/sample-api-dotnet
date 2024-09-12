using System.Data.Common;
using Looplex.DotNet.Core.Application.Abstractions.Commands;
using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Infra.Data.Mappers;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.CommandHandlers
{
    public class UpdateStudentCommandHandler(IDatabaseContext context) : ICommandHandler<UpdateStudentCommand>
    {
        public async Task Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = context.CreateConnection();
            connection.Open();
            await using var transaction = connection.BeginTransaction();

            await UpdateStudentIfNecessaryAsync(request.Student, connection, transaction);
            
            // TODO update projects
            
            await transaction.CommitAsync(cancellationToken);
        }

        private async Task UpdateStudentIfNecessaryAsync(Student student, IDatabaseConnection connection, DbTransaction transaction)
        {
            if (student.ChangedPropertyNotification.ChangedProperties.Count > 0)
            {
                var parameters = new Dictionary<string, object>();
                var studentType = typeof(Student);
                var update = "UPDATE students SET ";
                var sets = new List<string>();
                foreach (var changedProperty in student.ChangedPropertyNotification.ChangedProperties)
                {
                    sets.Add($"{StudentMapper.Maps[changedProperty]} = @{changedProperty}");
                    parameters[$"{changedProperty}"] = studentType.GetProperty(changedProperty)!.GetValue(student)!;
                }
                parameters.Add("UniqueId", student.UniqueId!.Value);
                var where = "WHERE uuid = @UniqueId";

                var query = $@"
                    {update}
                    {string.Join(',', sets)}
                    {where}
                ";

                var count = await connection.ExecuteAsync(query, parameters, transaction);
                if (count == 0) throw new EntityNotFoundException(nameof(Student), student.UniqueId!.Value.ToString());
            }
        }
    }
}
