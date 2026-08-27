namespace Visitz.Views.Entity.Attachments;

internal class AttachmentDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? DraftTemplate { get; set; }

    public DataTemplate? DownloadedTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        return ((AttachmentsListItemUi)item).Attachment.HasDraft ? DraftTemplate : DownloadedTemplate;
    }
}
