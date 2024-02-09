using CommunityToolkit.Mvvm.ComponentModel;
using VisitzModel.Models;

namespace Visitz.ViewModels.Entity;

public partial class EntityContainerViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;
}
