using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

public partial class Student
{
    [JsonProperty("registrationId")]
    public virtual string? RegistrationId { get; set; }

    [JsonProperty("userId")]
    public virtual int? UserId { get; set; }

    [JsonProperty("projects")]
    public virtual IList<Project> Projects { get; set; } = new ObservableCollection<Project>();
}