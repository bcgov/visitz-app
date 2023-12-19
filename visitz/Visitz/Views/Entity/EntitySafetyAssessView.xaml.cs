using Visitz.Models;

namespace Visitz.Views.Entity;

public partial class EntitySafetyAssessView : ViewModelContentView, ICaseloadItemHolder
{
	protected new EntitySafetyAssessViewModel ViewModel => (EntitySafetyAssessViewModel)base.ViewModel;

    public CaseloadItem CaseloadItem 
	{
		get => ViewModel.CaseloadItem;
		set => ViewModel.CaseloadItem = value;
	}

	public EntitySafetyAssessView() : base(ServiceProvider.GetService<EntitySafetyAssessViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}