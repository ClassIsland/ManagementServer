using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassIsland.Shared.Models.Profile;

/// <summary>
/// 代表一个课表<see cref="ClassPlan"/>触发规则。
/// </summary>
public class TimeRule : ObservableRecipient
{
    private int _weekDay = new();
    private int _weekCountDiv = 0;
    private int _weekCountDivTotal = 2;

    public int WeekDay
    {
        get => _weekDay;
        set
        {
            if (Equals(value, _weekDay)) return;
            _weekDay = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 单周/双周
    /// </summary>
    /// <value>
    /// 0 - 不限<br/>
    /// 1 - 单周<br/>
    /// 2 - 双周
    /// </value>
    public int WeekCountDiv
    {
        get => _weekCountDiv;
        set
        {
            if (value == _weekCountDiv) return;
            _weekCountDiv = value;
            OnPropertyChanged();
        }
    }
    
    /// <summary>
    /// 多周轮换总周数
    /// </summary>
    /// <value>
    /// 2 - 双周轮换<br/>
    /// 3 - 三周轮换<br/>
    /// 4 - 四周轮换
    /// </value>
    public int WeekCountDivTotal
    {
        get => _weekCountDivTotal;
        set
        {
            if (value == _weekCountDivTotal) return;
            _weekCountDivTotal = value;
            OnPropertyChanged();
        }
    }
}