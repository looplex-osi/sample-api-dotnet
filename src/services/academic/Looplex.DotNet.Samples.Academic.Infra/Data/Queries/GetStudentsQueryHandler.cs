using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Core.Application.Abstractions.Queries;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Domain.Queries;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.Queries
{
    public class GetStudentsQueryHandler : IQueryHandler<GetStudentsQuery, PaginatedCollection>
    {
        private readonly IDatabaseContext _context;

        public GetStudentsQueryHandler(IDatabaseContext context)
        {
            _context = context;
        }

        public async Task<PaginatedCollection> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var page = (int)request.Context.State.Pagination.Page;
            var perPage = (int)request.Context.State.Pagination.PerPage;

            var select = "cast(id as varchar(36)) as id, registrationid, cast(userid as varchar(36)) as userid";
            var where = "";
            var orderBy = "id DESC";

            var query = @$"
                SELECT COUNT(1) FROM students
                {where}
                ";

            using var connection = _context.CreateConnection();

            int count = await connection.QueryFirstOrDefaultAsync<int>(query);

            query = @$"
                SELECT {select} FROM students
                {where}
                ORDER BY {orderBy}
                OFFSET @offset ROWS
                    FETCH NEXT @perPage ROWS ONLY";

            var records = await connection.QueryAsync<Student>(query, new { offset = PaginationUtils.GetOffset(perPage, page), perPage });

            request.Context.State.Pagination.TotalCount = count;
            return new PaginatedCollection
            {
                Page = page,
                PerPage = perPage,
                TotalCount = count,
                Records = records.Select(r => (object)r).ToList(),
            };
        }
    }
}
