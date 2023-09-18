using Visitz.Models;
using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class NotePublishPage : VisitzPage
{
	public NotePublishPage(NotePublishViewModel viewModel) : base(viewModel)
    {
		InitializeComponent();
        BindingContext = viewModel;
    }

    public static async Task Open(Page fromPage, CaseloadItem caseIncident, NoteItem noteItem, string draft)
    {
        await NavigateTo<NotePublishPage>(fromPage, new Dictionary<string, object>
        {
            { NotePublishViewModel.NoteItemKey, noteItem },
            { NotePublishViewModel.CaseIncidentKey, caseIncident },
            { NotePublishViewModel.DraftItemKey, draft }
        }, modal: false);
    }

    protected override bool OnBackButtonPressed()
    {
        // Prevent the user from backing out of this screen in favour of using the dismiss button.
        return false;
    }
}
