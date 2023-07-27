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

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateLayout(width, height);
    }

    public void ShowAddNotesPlaceholder(bool show)
    {
        AddNotePlaceholder.IsVisible = show;
        var actualHeight = (ViewModel as NotesViewModel).CaseIncident.EntityType == IcmEntity.Case
            ? 100 : 70;
        AddNoteRow.Height = show ? actualHeight : 0;
        UpdateLayout(Width, Height);
    }

    private void UpdateLayout(double width, double height)
    {
        var resizableRowHeight = height;
        resizableRowHeight -= (
            AbsoluteLayout.Padding.Top
            + AbsoluteLayout.Padding.Bottom
            + FixedRow.Height.Value
            + AddNoteRow.Height.Value
            + RootGrid.RowSpacing
            + RootGrid.Padding.Top
            + RootGrid.Padding.Bottom
        );
        if (resizableRowHeight > 0)
        {
            // This was done because of a Grid layout issue. (18 June 2023)
            // Issue: ScrollView inside a Grid's row breaks the Grid's layout and
            // goes past the device screen's visible area to a certain extent.
            // Fix: Setting the row height manually seems to prevent the scroll from going beyond the limits.
            ResizableRow.Height = resizableRowHeight;
        }
    }
}
