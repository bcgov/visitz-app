using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;

namespace Visitz.Views.Entity.Details;

public partial class EntityDetailsView : ViewModelContentView, ICaseloadItemHolder
{
    public CaseloadItem CaseloadItem
    {
        get => (ViewModel as ICaseloadItemHolder).CaseloadItem;
        set => (ViewModel as ICaseloadItemHolder).CaseloadItem = value;
    }
    public EntityDetailsView() : base(ServiceProvider.GetService<EntityDetailsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
