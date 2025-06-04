The ImagePdfStreamConverter class was used to wrap images in a PDF, adjusting the PDF's orientation to allow the image to fit the correct aspect ratio.

This was needed because our API at the time forcibly renamed all incoming files by appending ".pdf" at the end. We don't expect users to rename files to view them, so if you would try opening one of these, you would be met with a "corrupted" PDF file rather than an image.

We no longer need this implementation because of an API upgrade.

It was used in only one place, viewable in this git diff:

    diff --git a/visitz/Visitz/Views/Entity/Attachments/AttachmentDraftPublishViewModel.cs b/visitz/Visitz/Views/Entity/Attachments/AttachmentDraftPublishViewModel.cs
    index 1777347d..800f2c00 100644
    --- a/visitz/Visitz/Views/Entity/Attachments/AttachmentDraftPublishViewModel.cs
    +++ b/visitz/Visitz/Views/Entity/Attachments/AttachmentDraftPublishViewModel.cs
    @@ -1,6 +1,5 @@
    using CommunityToolkit.Mvvm.Messaging;
    using Realms;
    -using Visitz.Documents;
    using Visitz.Resources.Localization;
    using Visitz.Services;
    using Visitz.Services.Attachments;
    @@ -164,12 +163,4 @@ internal class AttachmentDraftPublishViewModel : PublishViewModel, IRecipient<Se
        {
            await attachmentDraft.Attachment.DeleteAsync(removeContent: false);
        }
    -
    -    [Obsolete("No longer used")]
    -    static ImagePdfStreamConverter TryMakeImageToPdfConverter(AttachmentDraft attachmentDraft)
    -    {
    -        return Attachment.AllowedImageTypes.Contains(attachmentDraft.Attachment.Extension)
    -            ? new ImagePdfStreamConverter(attachmentDraft.Attachment.Filename, DisplayOrientation.Unknown)
    -            : null;
    -    }
