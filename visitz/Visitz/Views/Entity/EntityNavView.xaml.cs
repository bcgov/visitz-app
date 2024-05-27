using Visitz.ViewModels.Entity;
using VisitzModel.Models;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.Entity;

public partial class EntityNavView : ViewModelContentView, ICaseloadItemHolder
{
	new EntityNavViewModel ViewModel => base.ViewModel as EntityNavViewModel;

    public CaseloadItem CaseloadItem
	{
		get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public EntityNavView() : base(ServiceProvider.GetService<EntityNavViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

	public void SetSelectedSection(EntitySection section)
	{
		ViewModel.SetSelectedSection(section);
	}
}
