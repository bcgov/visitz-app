using Visitz.ViewModels;

namespace Visitz.Views;

public partial class NotesPage : VisitzPage
{
    public NotesPage(NotesViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    public static async Task Open(string caseIncidentId)
    {
        await NavigateTo(typeof(NotesPage), new Dictionary<string, object>
        {
            { NotesViewModel.CaseIncidentIdKey, caseIncidentId }
        });
    }

    async void CaseDetailsTapped(object sender, EventArgs e)
    {
        if (ViewModel is NotesViewModel notesVm)
            await notesVm.CaseDetailsTapped();
    }
}
