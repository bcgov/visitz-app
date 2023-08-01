using Visitz.Models;
using Visitz.ViewModels;

namespace Visitz.Pages;
/// <summary>
/// The screen that shows the full note and has a link to NoteEntry screen.
/// </summary>
public partial class NoteDetailsPage : VisitzPage
{
    public NoteDetailsPage(NoteDetailsViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    public static async Task Open(Page fromPage, CaseloadItem caseIncident, NoteItem noteItem, bool canAppendNotes)
    {
        await NavigateTo<NoteDetailsPage>(fromPage, new Dictionary<string, object> {
            { NoteDetailsViewModel.NoteItemKey, noteItem },
            { NoteDetailsViewModel.CaseIncidentKey, caseIncident },
            { NoteDetailsViewModel.CanAppendNotesKey, canAppendNotes }
        });
    }

    public async void ScrollToBottom()
    {
        if (ContentLabel.Height > ResizableRow.Height.Value)
        {
            await ContentScroll.ScrollToAsync(ContentLabel, ScrollToPosition.End, true);
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateLayout(width, height);
    }

    private void UpdateLayout(double width, double height)
    {
        var resizableRowHeight = height;
        resizableRowHeight -= RootGrid.RowSpacing + RootGrid.Padding.Bottom;
        if (resizableRowHeight > 0)
        {
            // This was done because of a Grid layout issue. (13 June 2023)
            // Issue: ScrollView inside a Grid's row breaks the Grid's layout and
            // goes past the device screen's visible area to a certain extent.
            // Fix: Setting the row height manually seems to prevent the scroll from going beyond the limits.
            ResizableRow.Height = resizableRowHeight;
        }
    }

    void ContentLabel_SizeChanged(System.Object sender, System.EventArgs e)
    {
        ScrollToBottom();
    }
}

