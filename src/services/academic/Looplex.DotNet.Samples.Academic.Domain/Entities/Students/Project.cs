using System.Collections.Generic;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Abstractions;

namespace Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

public partial class Project : ObservableType, IEntity
{
    public override IList<string> ChangedProperties { get; } = new List<string>();
    public override IDictionary<string, IList<object>> AddedItems { get; } = new Dictionary<string, IList<object>>();
    public override IDictionary<string, IList<object>> RemovedItems { get; } = new Dictionary<string, IList<object>>();
}