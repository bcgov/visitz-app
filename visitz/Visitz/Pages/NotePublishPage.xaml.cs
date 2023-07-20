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

    public static async Task OpenModal(Page fromPage, CaseloadItem caseIncident, NoteItem noteItem, string draft)
    {
        await NavigateTo<NotePublishPage>(fromPage, new Dictionary<string, object>
        {
            { NotePublishViewModel.NoteItemKey, noteItem },
            { NotePublishViewModel.CaseIncidentKey, caseIncident },
            { NotePublishViewModel.DraftItemKey, draft }
        }, true);
    }
}
