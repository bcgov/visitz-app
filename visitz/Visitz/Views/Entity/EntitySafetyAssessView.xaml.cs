using Visitz.Models;

namespace Visitz.Views.Entity;

public partial class EntitySafetyAssessView : ViewModelContentView, ICaseloadItemHolder
{
    public CaseloadItem CaseloadItem 
	{
		get => (ViewModel as EntitySafetyAssessViewModel).CaseloadItem;
		set => (ViewModel as EntitySafetyAssessViewModel).CaseloadItem = value;
	}

	public EntitySafetyAssessView() : base(ServiceProvider.GetService<EntitySafetyAssessViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}