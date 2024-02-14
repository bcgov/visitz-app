using CommunityToolkit.Mvvm.ComponentModel;
using VisitzModel.Models;

namespace Visitz.ViewModels.Entity;

public partial class EntityDetailsViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;
}
