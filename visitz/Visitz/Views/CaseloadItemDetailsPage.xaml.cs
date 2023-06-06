using Visitz.ViewModels;
namespace Visitz.Views;

public partial class CaseloadItemDetailsPage : VisitzPage
{
    public CaseloadItemDetailsPage(CaseloadItemDetailsViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

	public static async Task Open(string caseIncidentId)
	{
        await NavigateTo(typeof(CaseloadItemDetailsPage), new Dictionary<string, object>
        {
            { CaseloadItemDetailsViewModel.CaseIncidentIdKey, caseIncidentId }
        });
    }
}
