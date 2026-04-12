using Caliburn.Micro;
using SCaddins.ExportSchedules;

public class ScheduleItemViewModel : PropertyChangedBase
{
    public ScheduleItemViewModel(Schedule schedule)
    {
        Schedule = schedule;
    }

    public Schedule Schedule { get; }

    private bool isSelected;
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (isSelected != value)
            {
                isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }
    }
}
