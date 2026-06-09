namespace VisitzModel.Models.Notes;

public partial class NoteItem
{
    public string ContentBinding
    {
        get => IsValid ? Content : string.Empty;
        set
        {
            Content = value;
            RaisePropertyChanged(nameof(Content));
        }
    }
}
