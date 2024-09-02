using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Core.Application.Abstractions.Queries;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Domain.Queries;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.Queries
{
    public class GetStudentByIdQueryHandler(IDatabaseContext context) : IQueryHandler<GetStudentByIdQuery, Student>
    {
        public async Task<Student> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var select = "id Id, uuid UniqueId, registration_id RegistrationId, user_id UserId";
            var where = "uuid = @Id";

            var query = @$"
                SELECT {select} FROM students
                WHERE {where}
                ";

            using var connection = context.CreateConnection();

            var record = await connection.QueryFirstOrDefaultAsync<Student>(query, new { Id = request.UniqueId });
            if (record == null)
                throw new EntityNotFoundException(nameof(Student), request.UniqueId.ToString());
            
            select = "id Id, uuid UniqueId, student_id StudentId, name Name"; 
            where = "student_id = @Id";
            
            query = @$"
                SELECT {select} FROM projects
                WHERE {where}
                ";
            var records = await connection.QueryAsync<Project>(query, new { Id = record.Id });
            
            record.Projects = records.ToList();
            return record;
        }
    }
}
