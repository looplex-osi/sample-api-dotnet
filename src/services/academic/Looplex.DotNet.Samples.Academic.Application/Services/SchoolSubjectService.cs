using System.Threading;
using System.Threading.Tasks;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.OpenForExtension.Context;

namespace Looplex.DotNet.Samples.Academic.Application.Services
{
    public class SchoolSubjectService : ISchoolSubjectService
    {
        public Task GetAllAsync(IDefaultContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task GetByIdAsync(IDefaultContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task CreateAsync(IDefaultContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(IDefaultContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
