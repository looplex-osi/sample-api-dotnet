using Looplex.DotNet.Middlewares.ScimV2.Entities;

namespace Looplex.DotNet.Samples.Academic.Domain.Entities
{
    public class Student : Resource
    {
        public string RegistrationId { get; set; }
        public int UserId { get; set; }
    }
}
