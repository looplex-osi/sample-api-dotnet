using System.Threading.Tasks;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;

namespace Looplex.DotNet.Samples.Academic.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        public Task RegisterUserToSchoolSubjectAsync(int studentId, int schoolSubjectId)
        {
            throw new System.NotImplementedException();
        }
    }
}
