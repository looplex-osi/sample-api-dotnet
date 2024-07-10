using System.Text.Json.Serialization;

namespace Looplex.DotNet.Samples.Academic.Application.Abstractions.DTOs
{
    public class StudentDTO
    {
        [JsonPropertyName("registration_id")]
        public string RegistrationId { get; set; }
        
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
    }
}
