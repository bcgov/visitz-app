using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.InPersonVisits;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitListItemViewModel(PersonVisit visit) : VisitzViewModel
{
    [ObservableProperty]
    public partial PersonVisit Visit { get; set; } = visit;

    public bool MoreVisitDetails => Visit.VisitDetailsBinding.Count > 1;

    public string MoreVisitDetailsIndicator => MoreVisitDetails ? " [+]" : string.Empty;
}
