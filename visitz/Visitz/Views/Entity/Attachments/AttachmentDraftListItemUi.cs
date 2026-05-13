using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.FontIcons;
using VisitzModel.Models.Attachments;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

public partial class AttachmentDraftListItemUi : ObservableObject
{
    [ObservableProperty]
    public partial Attachment Attachment { get; set; }

    [ObservableProperty]
    public partial bool HasThumbnail { get; set; }

    [ObservableProperty]
    public partial string FontFamily { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string IconGlyph { get; set; } = string.Empty;

    public AttachmentDraftListItemUi(Attachment attachment)
    {
        Attachment = attachment;
        HasThumbnail = attachment.Thumbnail?.Length > 0;

        FontFamily = FluentIcons.FontConfig.FontFamily;
        IconGlyph = FluentIcons.Document_pdf_20_regular;
    }

    public AttachmentDraftListItemUi(AttachmentDraft draft)
        : this(draft.Attachment ?? new()) { }
}
