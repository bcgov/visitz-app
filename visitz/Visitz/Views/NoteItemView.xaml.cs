using Visitz.Models;
using Visitz.Storage;

namespace Visitz.Views;

public partial class NoteItemView : ContentView
{
    static readonly BindableProperty.BindingPropertyChangedDelegate NoteItemPropertyChanged =
        (boundObj, oldValue, newValue) =>
        {
            (boundObj as NoteItemView).UpdateUI();
        };

    public static readonly BindableProperty LatestNoteProperty =
        BindableProperty.Create(nameof(LatestNote), typeof(NoteItem), typeof(NoteItemView),
            propertyChanged: NoteItemPropertyChanged);

    public static readonly BindableProperty IsAddNotesPlaceholderVisibleProperty =
        BindableProperty.Create(nameof(IsAddNotesPlaceholderVisible), typeof(bool), typeof(NoteItemView),
            propertyChanged: NoteItemPropertyChanged);


    public NoteItem LatestNote
    {
        get => (NoteItem)GetValue(LatestNoteProperty);
        set => SetValue(LatestNoteProperty, value);
    }

    public bool IsAddNotesPlaceholderVisible
    {
        get => (bool)GetValue(IsAddNotesPlaceholderVisibleProperty);
        set => SetValue(IsAddNotesPlaceholderVisibleProperty, value);
    }

    public NoteItemView()
	{
		InitializeComponent();
	}

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        UpdateUI();
    }

    private void UpdateUI()
    {
        var note = BindingContext as NoteItem;

        if (DebugOptions.ShowNoteItemViewDebugInfo)
        {
            ContentLabel.Text = $"note: {NoteItem.ToStringLite(note)}\n"
                + $"LatestNote: {NoteItem.ToStringLite(LatestNote)}\n"
                + $"IsAddNotesPlaceholderVisible: {IsAddNotesPlaceholderVisible}";
        }

        if (note is null 
            || LatestNote is null
            || !note.IsValid
            || !LatestNote.IsValid)
            return;

        var isVisible = !IsAddNotesPlaceholderVisible && NoteItem.EqualByDates(note, LatestNote);

        AddNotesLabel.IsVisible = isVisible;
        AddNotesImage.IsVisible = isVisible;
    }
}
