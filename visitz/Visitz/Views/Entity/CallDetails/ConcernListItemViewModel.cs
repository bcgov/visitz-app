using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.CallDetails;

namespace Visitz.Views.Entity.CallDetails;

public partial class ConcernListItemViewModel : VisitzViewModel, IComparable<ConcernListItemViewModel>
{
    [ObservableProperty]
    public partial IncidentConcerns Concerns { get; set; }

    public int CompareTo(ConcernListItemViewModel? other)
    {
        return Concerns.CreatedBinding.CompareTo(other?.Concerns.CreatedBinding ?? DateTimeOffset.MinValue) * -1;
    }
}
