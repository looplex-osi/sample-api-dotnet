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

            var select = "cast(id as varchar(36)) as id, registrationid, cast(userid as varchar(36)) as userid";
            var where = "id = @Id";

            var query = @$"
                SELECT {select} FROM students
                WHERE {where}
                ";

            using var connection = context.CreateConnection();

            var record = await connection.QueryFirstOrDefaultAsync<Student>(query, new { request.Id });

            return record ?? throw new EntityNotFoundException(nameof(Student), request.Id);;
        }
    }
}
