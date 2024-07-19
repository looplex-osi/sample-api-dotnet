using System.Globalization;
using Looplex.DotNet.Middlewares.ScimV2.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

public partial class Student : Resource
{
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}

public static class Serialize
{
    public static string ToJson(this Student self) => JsonConvert.SerializeObject(self, Student.Converter.Settings);
}