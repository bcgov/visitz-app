using Visitz.Models;
using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class NoteEntryPage : VisitzPage
{
    public NoteEntryPage(NoteEntryViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public static async Task Open(Page fromPage, CaseloadItem caseIncident, NoteItem noteItem)
    {
        await NavigateTo<NoteEntryPage>(fromPage, new Dictionary<string, object>
        {
            { NoteEntryViewModel.NoteItemKey, noteItem },
            { NoteEntryViewModel.CaseIncidentKey, caseIncident }
        });
    }

    void NotesEditor_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        ((NoteEntryViewModel)BindingContext).EditorTextChanged();
    }
}