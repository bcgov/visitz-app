using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.Navigation;

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

	public void SetRequestedSection(EntitySection section)
	{
		ViewModel.SetRequestedSection(section);
	}
}
