using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.FontIcons;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.InPersonVisits;

namespace Visitz.Views.Entity.ChildYouthVisits;

#nullable enable

public partial class ChildYouthVisitListItemViewModel(PersonVisit visit) : VisitzViewModel
{
    [ObservableProperty]
    public partial PersonVisit Visit { get; set; } = visit;

    [ObservableProperty]
    public partial bool Expanded { get; set; }

    [ObservableProperty]
    public partial string ExpandedChevronGlyph { get; set; } = MaterialIcons.Keyboard_arrow_down;

    partial void OnExpandedChanged(bool value)
    {
        ExpandedChevronGlyph = value ? MaterialIcons.Keyboard_arrow_up : MaterialIcons.Keyboard_arrow_down;
    }

    [RelayCommand]
    public void ItemTapped()
    {
        Expanded = !Expanded;
    }
}
