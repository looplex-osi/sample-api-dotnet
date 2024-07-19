using System.Text.Json.Serialization;

namespace Looplex.DotNet.Samples.Academic.Application.Abstractions.Dtos.Users
{
    public class StudentReadDto : StudentDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}
