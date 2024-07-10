using System.Text.Json.Serialization;

namespace Looplex.DotNet.Samples.Academic.Application.Abstractions.DTOs
{
    public class StudentReadDTO : StudentDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
