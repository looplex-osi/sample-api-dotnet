using System.Threading;
using System.Threading.Tasks;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Samples.Academic.Application.Services
{
    public class SchoolSubjectService : ISchoolSubjectService
    {
        public Task GetAllAsync(IContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task GetByIdAsync(IContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task CreateAsync(IContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(IContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
