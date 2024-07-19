using System.Text.Json.Serialization;

namespace Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos.Users
{
    public class StudentDto
    {
        [JsonPropertyName("registrationId")]
        public string? RegistrationId { get; set; }
        
        [JsonPropertyName("userId")]
        public int ?UserId { get; set; }
    }
}
