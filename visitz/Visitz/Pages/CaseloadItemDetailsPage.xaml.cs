using Visitz.ViewModels;
namespace Visitz.Pages;

public partial class CaseloadItemDetailsPage : VisitzPage
{
    public CaseloadItemDetailsPage(CaseloadItemDetailsViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

	public static async Task Open(Page fromPage, string caseIncidentId)
	{
        await NavigateTo<CaseloadItemDetailsPage>(fromPage, new Dictionary<string, object>
        {
            { CaseloadItemDetailsViewModel.CaseIncidentIdKey, caseIncidentId }
        });
    }
}
