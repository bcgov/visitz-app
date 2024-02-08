using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Models;
using Visitz.ViewModels;
using VisitzModel.Models;

namespace Visitz.Views.Entity;

public partial class EntityContainerViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;
}
