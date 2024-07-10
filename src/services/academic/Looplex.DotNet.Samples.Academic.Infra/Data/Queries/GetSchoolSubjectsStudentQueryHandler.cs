using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Core.Application.Abstractions.Queries;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities;
using Looplex.DotNet.Samples.Academic.Domain.Queries;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.Queries
{
    public class GetSchoolSubjectsStudentQueryHandler : IQueryHandler<GetSchoolSubjectStudentsQuery, PaginatedCollection<Student>>
    {
        private readonly IDatabaseContext _context;

        public GetSchoolSubjectsStudentQueryHandler(IDatabaseContext context)
        {
            _context = context;
        }

        public async Task<PaginatedCollection<Student>> Handle(GetSchoolSubjectStudentsQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var page = (int)request.Context.State.Pagination.Page;
            var perPage = (int)request.Context.State.Pagination.PerPage;

            var select = "s.id, s.registrationid, s.userid";
            var where = "";
            var orderBy = "id DESC";

            var query = @$"
                SELECT COUNT(1) FROM schoolsubjects
                WHERE
                    id = @SchoolSubjectId
                ";

            using var connection = _context.CreateConnection();

            int count = await connection.QueryFirstOrDefaultAsync<int>(query, new { request.SchoolSubjectId });

            if (count == 0)
            {
                throw new EntityNotFoundException(nameof(SchoolSubject), request.SchoolSubjectId.ToString());
            }

            query = @$"
                SELECT 
                    s.id, s.registrationid, s.userid
                FROM schoolsubjects_students sss 
                JOIN students s on 
                    sss.studentid = s.id
                WHERE
                    sss.schoolsubjectid = @SchoolSubjectId
                    AND sss.isactive = 1
                    {where}
                ";

            count = await connection.QueryFirstOrDefaultAsync<int>(query, new { request.SchoolSubjectId });

            query = @$"
                SELECT 
                    {select}
                FROM schoolsubjects_students sss 
                JOIN students s on 
                    sss.studentid = s.id
                WHERE
                    sss.schoolsubjectid = @SchoolSubjectId
                    AND sss.isactive = 1
                    {where}
                ORDER BY {orderBy}
                OFFSET @offset ROWS
                    FETCH NEXT @perPage ROWS ONLY";
            
            var records = await connection.QueryAsync<Student>(query, new { offset = PaginationUtils.GetOffset(perPage, page), perPage, request.SchoolSubjectId });

            request.Context.State.Pagination.TotalCount = count;
            return new PaginatedCollection<Student>
            {
                Page = page,
                PerPage = perPage,
                TotalCount = count,
                Records = records,
            };
        }
    }
}
