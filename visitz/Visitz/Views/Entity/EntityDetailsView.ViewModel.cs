using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Models;
using Visitz.ViewModels;

namespace Visitz.Views.Entity;

public partial class EntityDetailsViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;
}
