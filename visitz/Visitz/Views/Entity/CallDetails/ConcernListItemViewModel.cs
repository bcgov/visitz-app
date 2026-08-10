using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.FontIcons;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.CallDetails;

namespace Visitz.Views.Entity.CallDetails;

public partial class ConcernListItemViewModel : VisitzViewModel, IComparable<ConcernListItemViewModel>
{
    [ObservableProperty]
    public partial IncidentConcerns Concerns { get; set; }

    [ObservableProperty]
    public partial bool Expanded { get; set; }

    [ObservableProperty]
    public partial string ExpandedChevronGlyph { get; set; } = MaterialIcons.Keyboard_arrow_down;

    [RelayCommand]
    public void ToggleExpanded()
    {
        Expanded = !Expanded;
        ExpandedChevronGlyph = Expanded ? MaterialIcons.Keyboard_arrow_up : MaterialIcons.Keyboard_arrow_down;
    }

    public int CompareTo(ConcernListItemViewModel? other)
    {
        return Concerns.CreatedBinding.CompareTo(other?.Concerns.CreatedBinding ?? DateTimeOffset.MinValue) * -1;
    }
}
