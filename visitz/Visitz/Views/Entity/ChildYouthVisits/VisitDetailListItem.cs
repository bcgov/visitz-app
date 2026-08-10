using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.InPersonVisits;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class VisitDetailListItem : VisitzViewModel
{
    [ObservableProperty]
    public partial string DetailValue { get; set; }

    [ObservableProperty]
    public partial bool IsChecked { get; set; }

    PersonVisit Visit { get; }

    public VisitDetailListItem(string detailValue, PersonVisit visit)
    {
        DetailValue = detailValue;
        Visit = visit;

        IsChecked = visit.VisitDetails.Contains(DetailValue);
    }

    partial void OnIsCheckedChanged(bool value)
    {
        Visit.ToggleVisitDetail(DetailValue, value);
    }
}
