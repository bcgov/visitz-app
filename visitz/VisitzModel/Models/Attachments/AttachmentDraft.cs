using Realms;
using VisitzApi.Models.Attachments;
using VisitzModel.Extensions;
using VisitzModel.Formats;
using VisitzModel.Imaging;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Resources.Localization;
using VisitzModel.Storage.Filesystem;

namespace VisitzModel.Models.Attachments;

public partial class AttachmentDraft : IRealmObject, IDraftItem
{
    public string RelatedEntityId { get; set; } = string.Empty;

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

    public string Preview => GeneralStrings.Attachment;

    public string DraftLocation { get; set; } = string.Empty;

    public Attachment? Attachment { get; set; }

    private bool disposedValue;
    private bool? relatedEntityAvailable;
    private bool? relatedEntityDownloaded;

    [Ignored]
    public Realm? RelatedEntityRealm { get; set; }

    [Ignored]
    public IQueryable<IBusinessObject>? RelatedEntitySubscriptionQuery { get; set; }

    [Ignored]
    public IDisposable? RelatedEntitySubscriptionToken { get; set; }

    /// <summary>
    /// Whether or not the related entity is available for the app to interact
    /// with at all.
    /// </summary>
    [Ignored]
    public bool? RelatedEntityAvailable
    {
        get => relatedEntityAvailable;
        set
        {
            relatedEntityAvailable = value;
            RaisePropertyChanged(nameof(RelatedEntityAvailable));
        }
    }

    /// <summary>
    /// Whether or not the related entity's depdendent data has been
    /// downloaded (or marked for download).
    /// </summary>
    [Ignored]
    public bool? RelatedEntityDownloaded
    {
        get => relatedEntityDownloaded;
        set
        {
            relatedEntityDownloaded = value;
            RaisePropertyChanged(nameof(RelatedEntityDownloaded));
        }
    }

    public AttachmentDraft() { }

    AttachmentDraft(IBusinessObject businessObject, string filename, string relativePath, byte[]? thumbnail)
    {
        int dotIndex = filename.LastIndexOf('.');

        Attachment = new()
        {
            Filename = dotIndex != -1 ? filename[..dotIndex] : filename,
            Extension = dotIndex != -1 ? filename[dotIndex..] : filename,
            RelativePath = relativePath,
            Thumbnail = thumbnail,
        };

        this.InitDraftWith(businessObject);
        Attachment.InitWith(businessObject);

        Attachment.FileNumber = businessObject.FileNumber;
    }

    public static async Task<AttachmentDraft> SaveNewPhoto(
        IBusinessObject businessObject,
        AttachmentFiler filer,
        Realm realm,
        string filename,
        Stream stream
    )
    {
        var imgProc = new ImageProcessor(stream);

        byte[] thumbnail = await (await imgProc.DownsizeImage(Attachment.ThumbnailSize)).AsBytesAsync();

        if (stream.Length > Attachment.MaxFilesize)
            stream = await imgProc.DownsizeImageByFilesize(Attachment.MaxFilesize);

        return await MakeAndSaveDraft(businessObject, filer, realm, filename, stream, thumbnail);
    }

    public static async Task<AttachmentDraft> SaveNewFile(
        IBusinessObject businessObject,
        AttachmentFiler filer,
        Realm realm,
        string filename,
        Stream stream
    )
    {
        return await MakeAndSaveDraft(businessObject, filer, realm, filename, stream);
    }

    static async Task<AttachmentDraft> MakeAndSaveDraft(
        IBusinessObject businessObject,
        AttachmentFiler filer,
        Realm realm,
        string filename,
        Stream stream,
        byte[]? thumbnail = null
    )
    {
        if (stream.Length > Attachment.MaxFilesize)
            ThrowSizeError(stream);

        string fullpath = await filer.SaveFileAsync(stream, filename.GetFileExtension());
        var draft = new AttachmentDraft(businessObject, filename, fullpath, thumbnail);

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

    public async Task<AttachmentFormData> ToAttachmentFormData(
        AttachmentFiler attachmentFiler,
        string? category = null,
        string? description = null,
        string? status = null,
        string? template = null,
        CancellationToken? token = null
    )
    {
        ArgumentNullException.ThrowIfNull(Attachment);
        token ??= CancellationToken.None;

        var attachmentStream = await attachmentFiler.GetAppDataFileAsync(Attachment.RelativePath, token);

        return new AttachmentFormData(
            Attachment.Filename + Attachment.Extension,
            attachmentStream,
            category,
            description,
            status,
            template
        );
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                RelatedEntitySubscriptionToken?.Dispose();
                RelatedEntitySubscriptionToken = null;
                RelatedEntitySubscriptionQuery = null;
                RelatedEntityRealm = null;
                RelatedEntityAvailable = null;
                RelatedEntityDownloaded = null;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public int CompareTo(IDraftItem? other)
    {
        return this.CompareDraftItem(other);
    }
}
