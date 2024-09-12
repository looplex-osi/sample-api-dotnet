using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.Mappers;

internal static class StudentMapper
{
    private static IDictionary<string, string>? _maps;

    public static IDictionary<string, string> Maps
    {
        get
        {
            if (_maps == null)
                CreateMap();
            
            return _maps!;
        }
    }

    private static void CreateMap()
    {
        _maps = new Dictionary<string, string>();
        _maps.Add(nameof(Student.Id), "id");
        _maps.Add(nameof(Student.RegistrationId), "registration_id");
        _maps.Add(nameof(Student.UserId), "user_id");
    }
}