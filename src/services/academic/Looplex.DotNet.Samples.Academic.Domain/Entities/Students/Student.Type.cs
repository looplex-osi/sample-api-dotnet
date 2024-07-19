using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Newtonsoft.Json;

namespace Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

public partial class Student : Resource
{
    [JsonProperty("registrationId")]
    public string? RegistrationId { get; set; }

    [JsonProperty("userId")]
    public string? UserId { get; set; }
}