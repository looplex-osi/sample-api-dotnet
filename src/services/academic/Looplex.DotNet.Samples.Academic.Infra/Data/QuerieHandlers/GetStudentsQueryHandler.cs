using Looplex.DotNet.Core.Application.Abstractions.Queries;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Domain.Queries;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.QuerieHandlers
{
    public class GetStudentsQueryHandler() : IQueryHandler<GetStudentsQuery, ListResponse>
    {
        public async Task<ListResponse> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var startIndex = request.Context.GetRequiredValue<int>("Pagination.StartIndex");
            var itemsPerPage = request.Context.GetRequiredValue<int>("Pagination.ItemsPerPage");
            
            var select = "id, uuid, registration_id, user_id, created_at, updated_at";
            var where = "";
            var orderBy = "id DESC";

            var query = @$"
                SELECT COUNT(1) FROM students
                {where}
                ";

            var dbService = await request.Context.GetSqlDatabaseService();

            int count = await dbService.QueryFirstOrDefaultAsync<int>(query);
            
            var offset = startIndex - 1;
            
            query = @$"
                SELECT {select} FROM students
                {where}
                ORDER BY {orderBy}
                OFFSET @offset ROWS
                    FETCH NEXT @itemsPerPage ROWS ONLY";

            var students = (await dbService
                .QueryAsync<dynamic>(query, new { offset, itemsPerPage }))
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
                
                var projects = (await dbService
                    .QueryAsync<Project>(query, new { Ids = students.Select(r => r.Id) }))
                    .ToList();

                foreach (var student in students)
                {
                    student.Projects = projects.Where(p => p.StudentId == student.Id).ToList();
                }
            }
            
            request.Context.State.Pagination.TotalCount = count;

            return new ListResponse
            {
                Resources = students.Select(r => (object)r).ToList(),
                StartIndex = startIndex,
                ItemsPerPage = itemsPerPage,
                TotalResults = count,
            };
        }
    }
}
