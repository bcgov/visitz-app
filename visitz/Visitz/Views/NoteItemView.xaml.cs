using Microsoft.Maui.Graphics.Text;
using Visitz.Models;

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
        if (note is null || note?.IsValid == false || LatestNote?.IsValid == false)
            return;
        
        var isVisible = note.CreatedDate == LatestNote?.CreatedDate
            && note.NotePeriod == LatestNote?.NotePeriod
            && !IsAddNotesPlaceholderVisible;

        AddNotesLabel.IsVisible = isVisible;
        AddNotesImage.IsVisible = isVisible;
    }
}
