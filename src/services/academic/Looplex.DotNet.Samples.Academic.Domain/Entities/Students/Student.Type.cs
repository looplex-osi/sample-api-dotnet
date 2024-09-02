using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

public partial class Student
{
    private string? _registrationId;
    private string? _userId;
    private IList<Project> _projects = new ObservableCollection<Project>();

    [JsonProperty("registrationId")]
    public string? RegistrationId
    {
        get => _registrationId;
        set
        {
            if (value != _registrationId)
            {
                _registrationId = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("userId")]
    public string? UserId 
    {
        get => _userId;
        set
        {
            _userId = value;
            if (value != _userId)
            {
                _userId = value;
                OnPropertyChanged();
            }
        }
    }
    
    [JsonProperty("projects")]
    public IList<Project> Projects 
    {
        get => _projects;
        set
        {
            if (value != _projects)
            {
                _projects = value;
                if (value is INotifyCollectionChanged collection)
                    BindOnCollectionChanged(ref collection);
            }
        }
    }
}