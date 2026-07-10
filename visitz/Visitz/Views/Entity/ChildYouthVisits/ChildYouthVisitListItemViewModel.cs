using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.InPersonVisits;

namespace Visitz.Views.Entity.ChildYouthVisits;

#nullable enable

public partial class ChildYouthVisitListItemViewModel(PersonVisit visit) : VisitzViewModel
{
    [ObservableProperty]
    public partial PersonVisit Visit { get; set; } = visit;
}
