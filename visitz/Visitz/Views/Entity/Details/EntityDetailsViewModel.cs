using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.ViewModels;
using VisitzModel.Models;

namespace Visitz.Views.Entity.Details;

public partial class EntityDetailsViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;
}
