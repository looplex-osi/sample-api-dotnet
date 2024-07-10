using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities;
using Looplex.OpenForExtension.Context;

namespace Looplex.DotNet.Samples.Academic.Domain.Queries
{
    public class GetStudentsQuery : IQuery<PaginatedCollection<Student>>
    {
        public IDefaultContext Context { get; set; }
    }
}
