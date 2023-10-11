using Visitz.Models;
using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class NotesPage : VisitzPage
{
    public NotesPage(NotesViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    public static async Task Open(Page fromPage, string caseIncidentId)
    {
        await NavigateTo<NotesPage>(fromPage, new Dictionary<string, object>
        {
            { NotesViewModel.CaseIncidentIdKey, caseIncidentId }
        });
    }

    public void ShowAddNotesPlaceholder(bool show)
    {
        AddNotePlaceholder.IsVisible = show;
    }
}
