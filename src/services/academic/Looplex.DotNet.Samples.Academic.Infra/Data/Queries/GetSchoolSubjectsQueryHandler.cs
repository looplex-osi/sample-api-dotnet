using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Core.Application.Abstractions.Queries;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities;
using Looplex.DotNet.Samples.Academic.Domain.Entities.SchoolSubjects;
using Looplex.DotNet.Samples.Academic.Domain.Queries;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.Queries
{
    public class GetSchoolSubjectsQueryHandler : IQueryHandler<GetSchoolSubjectsQuery, PaginatedCollection<SchoolSubject>>
    {
        private readonly IDatabaseContext _context;

        public GetSchoolSubjectsQueryHandler(IDatabaseContext context)
        {
            _context = context;
        }

        public async Task<PaginatedCollection<SchoolSubject>> Handle(GetSchoolSubjectsQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var page = (int)request.Context.State.Pagination.Page;
            var perPage = (int)request.Context.State.Pagination.PerPage;

            var select = "id, name";
            var where = "";
            var orderBy = "id DESC";

            var query = @$"
                SELECT COUNT(1) FROM schoolsubjects
                {where}
                ";

            using var connection = _context.CreateConnection();

            int count = await connection.QueryFirstOrDefaultAsync<int>(query);

            query = @$"
                SELECT {select} FROM schoolsubjects
                {where}
                ORDER BY {orderBy}
                OFFSET @offset ROWS
                    FETCH NEXT @perPage ROWS ONLY";

            var records = await connection.QueryAsync<SchoolSubject>(query, new { offset = PaginationUtils.GetOffset(perPage, page), perPage });

            request.Context.State.Pagination.TotalCount = count;
            return new PaginatedCollection<SchoolSubject>
            {
                Page = page,
                PerPage = perPage,
                TotalCount = count,
                Records = records,
            };
        }
    }
}
