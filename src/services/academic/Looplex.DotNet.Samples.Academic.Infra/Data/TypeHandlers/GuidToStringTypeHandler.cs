using System.Data;
using Dapper;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.TypeHandlers;

public class GuidToStringTypeHandler: SqlMapper.TypeHandler<string>
{
    public override void SetValue(IDbDataParameter parameter, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            parameter.Value = Guid.Parse(value);
        else
            parameter.Value = null;
        parameter.DbType = DbType.Guid;
    }

    public override string? Parse(object value)
    {
        return value.ToString();
    }
}