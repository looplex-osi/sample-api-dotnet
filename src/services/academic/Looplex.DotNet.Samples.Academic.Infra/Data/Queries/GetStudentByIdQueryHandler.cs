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

            var select = "id, uuid, registration_id, user_id, created_at, updated_at";
            var where = "uuid = @Id";

            var query = @$"
                SELECT {select} FROM students
                WHERE {where}
                ";

            using var connection = context.CreateConnection();

            var record = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { Id = request.UniqueId });
            if (record == null)
                throw new EntityNotFoundException(nameof(Student), request.UniqueId.ToString());

            var student = new Student
            {
                Id = record.id,
                UniqueId = record.uuid,
                RegistrationId = record.registration_id,
                Meta = new()
                {
                    Created = record.created_at,
                    LastModified = record.updated_at
                }
            };
            select = "id Id, uuid UniqueId, student_id StudentId, name Name"; 
            where = "student_id = @Id";
            
            query = @$"
                SELECT {select} FROM projects
                WHERE {where}
                ";
            var records = await connection.QueryAsync<Project>(query, new { Id = student.Id });
            
            student.Projects = records.ToList();
            return student;
        }
    }
}
