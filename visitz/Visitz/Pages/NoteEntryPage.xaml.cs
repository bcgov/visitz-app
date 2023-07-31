using Visitz.Models;
using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class NoteEntryPage : VisitzPage
{
    public NoteEntryPage(NoteEntryViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        RootGrid.KeyboardAppearanceEvent += grid_KeyboardAppearanceEvent;
    }

    void grid_KeyboardAppearanceEvent(object sender, EventArgs e)
    {
        UpdateLayout(Width, Height);
    }

    public static async Task Open(Page fromPage, CaseloadItem caseIncident, NoteItem noteItem)
    {
        await NavigateTo<NoteEntryPage>(fromPage, new Dictionary<string, object>
        {
            { NoteEntryViewModel.NoteItemKey, noteItem },
            { NoteEntryViewModel.CaseIncidentKey, caseIncident }
        });
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateLayout(width, height);
    }

    void NotesEditor_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        ((NoteEntryViewModel)BindingContext).EditorTextChanged();
    }

    private void UpdateLayout(double width, double height)
    {
        var resizableRowHeight = height;
        resizableRowHeight -= (
            TitleRow.Height.Value
            + DescriptionRow.Height.Value
            + ValidationRow.Height.Value
            + RootGrid.RowSpacing
            + RootGrid.Padding.Top
            + RootGrid.Padding.Bottom
            + RootGrid.KeyboardHeight
        );
        if (resizableRowHeight > 0)
        {
            // This was done because of a Grid layout issue. (18 June 2023)
            // Issue: ScrollView inside a Grid's row breaks the Grid's layout and
            // goes past the device screen's visible area to a certain extent.
            // Fix: Setting the row height manually seems to prevent the scroll from going beyond the limits.
            EditorRow.Height = resizableRowHeight;
            EditorScroll.HeightRequest = resizableRowHeight;
        }
    }

    async void Scroll_To_Bottom_Clicked(System.Object sender, System.EventArgs e)
    {
        await EditorScroll.ScrollToAsync(NotesEditor, ScrollToPosition.End, true);
    }
}