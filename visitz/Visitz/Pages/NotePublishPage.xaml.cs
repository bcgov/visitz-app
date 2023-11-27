using Visitz.Models;
using Visitz.Storage;
using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class NotePublishPage : VisitzPage
{
	public NotePublishPage(NotePublishViewModel viewModel) : base(viewModel)
    {
		InitializeComponent();
        BindingContext = viewModel;
    }

    public static async Task Open(CaseloadItem caseloadItem, string draft)
    {
        using var realm = await VisitzRealm.GetIcmDataAsync();
        var noteItem = NoteItem.GetLatestByEntityId(realm, caseloadItem.CaseIncidentNumber);

        await Open(caseloadItem, noteItem, draft);
    }

    public static async Task Open(CaseloadItem caseIncident, NoteItem noteItem, string draft)
    {
        var notePublishPage = ServiceProvider.GetService<NotePublishPage>();
        (notePublishPage.ViewModel as NotePublishViewModel).InitWith(caseIncident, noteItem, draft);
        
        await Navigator.Navigation.PushAsync(notePublishPage);
    }

    protected override bool OnBackButtonPressed()
    {
        // Prevent the user from backing out of this screen in favour of using the dismiss button.
        return false;
    }
}
