using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Messaging;
using Visitz.Models;
using Visitz.ViewModels;

namespace Visitz.Views.Entity;

public partial class EntityContainerViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;
}
