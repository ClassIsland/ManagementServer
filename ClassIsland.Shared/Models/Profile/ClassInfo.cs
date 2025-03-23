using System.Text.Json.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassIsland.Shared.Models.Profile;

/// <summary>
/// 代表一个在<see cref="ClassPlan"/>中的课程。
/// </summary>
public class ClassInfo : ObservableRecipient
{
    private string _subjectId = "";
    
    /// <summary>
    /// 课程ID
    /// </summary>
    public string SubjectId
    {
        get => _subjectId;
        set
        {
            if (value == _subjectId) return;
            _subjectId = value;
            OnPropertyChanged();
        }
    }
}