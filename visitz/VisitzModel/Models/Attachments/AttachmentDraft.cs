using Realms;
using VisitzApi.Models.Attachments;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Formats;
using VisitzModel.Imaging;
using VisitzModel.Interfaces;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Resources.Localization;
using VisitzModel.Storage.Filesystem;

namespace VisitzModel.Models.Attachments;

public partial class AttachmentDraft : IRealmObject, IDraftItem
{
    public string RelatedEntityId { get; set; }

    private int RelatedEntityTypeInt { get; set; }
    public EntityType RelatedEntityType
    {
        get => (EntityType)RelatedEntityTypeInt;
        set => RelatedEntityTypeInt = (int)value;
    }

    private int RelatedEntitySubtypeInt { get; set; }
    public EntitySubtype RelatedEntitySubtype
    {
        get => (EntitySubtype)RelatedEntitySubtypeInt;
        set => RelatedEntitySubtypeInt = (int)value;
    }

    public DateTimeOffset DraftCreated { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public string Preview => Attachment.Filename;

    public string DraftLocation { get; set; }

    public Attachment Attachment { get; set; }

    public AttachmentDraft() { }

    AttachmentDraft(
        CaseloadItem caseloadItem,
        string filename,
        string relativePath,
        byte[] thumbnail)
    {
        int dotIndex = filename.LastIndexOf('.');

        Attachment = new()
        {
            Filename = dotIndex != -1 ? filename[..dotIndex] : filename,
            Extension = dotIndex != -1 ? filename[dotIndex..] : filename,
            RelativePath = relativePath,
            Thumbnail = thumbnail,
        };

        this.InitDraftWith(caseloadItem);
        Attachment.InitWith(caseloadItem);

        Attachment.FileNumber = caseloadItem.CaseIncidentNumber;
    }

    public static async Task<AttachmentDraft> SaveNewPhoto(
        CaseloadItem caseloadItem,
        AttachmentFiler filer,
        Realm realm,
        string filename,
        Stream stream)
    {
        var imgProc = new ImageProcessor(stream);

        byte[] thumbnail = await (await imgProc.Downsize(Attachment.ThumbnailSize)).AsBytesAsync();

        if (stream.Length > Attachment.MaxFilesize)
            stream = await imgProc.DownsizeByFilesize(Attachment.MaxFilesize);

        return await MakeAndSaveDraft(caseloadItem, filer, realm, filename, stream, thumbnail);
    }

    public static async Task<AttachmentDraft> SaveNewFile(
        CaseloadItem caseloadItem,
        AttachmentFiler filer,
        Realm realm,
        string filename,
        Stream stream)
    {
        return await MakeAndSaveDraft(caseloadItem, filer, realm, filename, stream);
    }

    static async Task<AttachmentDraft> MakeAndSaveDraft(
        CaseloadItem caseloadItem,
        AttachmentFiler filer,
        Realm realm,
        string filename,
        Stream stream,
        byte[] thumbnail = null)
    {
        if (stream.Length > Attachment.MaxFilesize)
            ThrowSizeError(stream);

        string fullpath = await filer.SaveFileAsync(stream, filename.GetFileExtension());
        var draft = new AttachmentDraft(caseloadItem, filename, fullpath, thumbnail);

        try
        {
            await realm.WriteAsync(() => realm.Add(draft));
        }
        catch
        {
            if (File.Exists(fullpath))
                File.Delete(fullpath);

            throw;
        }

        return draft;
    }

    static void ThrowSizeError(Stream stream)
    {
        double tooLargeSize = stream.Length / (double)Sizes.MB;
        throw new ArgumentException(GeneralStrings.FileTooLarge.Format(tooLargeSize), nameof(stream));
    }

    public async Task<SubmitAttachmentEntity> ToSubmitAttachmentEntity(
        AttachmentFiler attachmentFiler,
        IStreamConverter streamConverter = null,
        CancellationToken? token = null)
    {
        token ??= CancellationToken.None;

        await using var attachmentStream = await attachmentFiler.GetAppDataFileAsync(Attachment.RelativePath, token);
        byte[] attachmentBytes;

        if (streamConverter != null)
        {
            var convertedStream = await streamConverter.ConvertAsync(attachmentStream);

            attachmentBytes = new byte[convertedStream.Length];
            await convertedStream.ReadAsync(attachmentBytes.AsMemory(0, attachmentBytes.Length), token.Value);
        }
        else
        {
            attachmentBytes = new byte[attachmentStream.Length];
            await attachmentStream.ReadAsync(attachmentBytes.AsMemory(0, attachmentBytes.Length), token.Value);
        }

        return new()
        {
            AttachmentId = Attachment.Id,
            EntityNumber = RelatedEntityId,
            EntityType = RelatedEntityType.GetDisplayString(),
            CaseType = RelatedEntitySubtype.GetDisplayString(),
            FormName = IcmFormNames.GenericDocument,
            FileName = Attachment.Filename,
            FormDescription = "",
            FormCategory = "",
            Section13Exists = "",
            InvestigationResponse = "",
            Attachment = new()
            {
                PdfString = Convert.ToBase64String(attachmentBytes),
            }
        };
    }
}
