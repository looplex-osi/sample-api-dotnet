using System.Threading.Tasks;

namespace Looplex.DotNet.Samples.Academic.Application.Abstractions.Services
{
    public interface IRegistrationService
    {
        Task RegisterUserToSchoolSubjectAsync(int studentId, int schoolSubjectId);
    }
}
