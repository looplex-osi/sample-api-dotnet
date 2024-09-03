using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Core.Application.Abstractions.Queries;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Domain.Queries;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.Queries
{
    public class GetStudentsQueryHandler(IDatabaseContext context)
        : IQueryHandler<GetStudentsQuery, PaginatedCollection>
    {
        public async Task<PaginatedCollection> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var page = (int)request.Context.State.Pagination.Page;
            var perPage = (int)request.Context.State.Pagination.PerPage;
            
            var select = "id, uuid, registration_id, user_id, created_at, updated_at";
            var where = "";
            var orderBy = "id DESC";

            var query = @$"
                SELECT COUNT(1) FROM students
                {where}
                ";

            using var connection = context.CreateConnection();

            int count = await connection.QueryFirstOrDefaultAsync<int>(query);

            query = @$"
                SELECT {select} FROM students
                {where}
                ORDER BY {orderBy}
                OFFSET @offset ROWS
                    FETCH NEXT @perPage ROWS ONLY";

            var students = (await connection
                .QueryAsync<dynamic>(query, new { offset = PaginationUtils.GetOffset(perPage, page), perPage }))
                .Select(r => new Student
                {
                    Id = r.id,
                    UniqueId = r.uuid,
                    RegistrationId = r.registration_id,
                    Meta = new()
                    {
                        Created = r.created_at,
                        LastModified = r.updated_at
                    }
                })
                .ToList();
            
            if (students.Count > 0)
            {
                select = "id Id, uuid UniqueId, student_id StudentId, name Name"; 
                where = "student_id IN @Ids";
                orderBy = "id DESC";
                
                query = @$"
                SELECT {select} FROM projects
                WHERE {where}
                ORDER BY {orderBy}
                ";
                
                var projects = (await connection
                    .QueryAsync<Project>(query, new { Ids = students.Select(r => r.Id) }))
                    .ToList();

                foreach (var student in students)
                {
                    student.Projects = projects.Where(p => p.StudentId == student.Id).ToList();
                }
            }
            
            request.Context.State.Pagination.TotalCount = count;
            return new PaginatedCollection
            {
                Page = page,
                PerPage = perPage,
                TotalCount = count,
                Records = students.Select(r => (object)r).ToList(),
            };
        }
    }
}
