namespace Visitz.Views.Caseload;

public partial class CaseloadFilterView : ViewModelContentView
{
    public CaseloadFilterView() : base(ServiceProvider.GetService<CaseloadFilterViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}    
}