using Visitz.ViewModels.Entity;
using VisitzModel.Models;

namespace Visitz.Views.Entity;

public partial class EntityNavView : ViewModelContentView, ICaseloadItemHolder
{
    public CaseloadItem CaseloadItem
	{
		get => (ViewModel as EntityNavViewModel).CaseloadItem;
        set => (ViewModel as EntityNavViewModel).CaseloadItem = value;
    }

    public EntityNavView() : base(ServiceProvider.GetService<EntityNavViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}