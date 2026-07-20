using Realms;
using VisitzApi.Models.Attachments;
using VisitzModel.Extensions;
using VisitzModel.Formats;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Storage;
using VisitzModel.Storage.Filesystem;

namespace VisitzModel.Models.Attachments;

public partial class Attachment : IRealmObject, IRecordInfo, IApiJson<AttachmentJson>, IEquatable<Attachment>
{
    public static readonly int MaxFilesize = 5 * Sizes.MB;
    public static readonly int ThumbnailSize = 400;

    public static readonly string Pdf = ".pdf";

    public static readonly IEnumerable<string> AllowedImageTypes = [".jpg", ".jpeg"];
    public static readonly IEnumerable<string> AllowedDocumentTypes = [Pdf];

    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string RelatedEntityId { get; set; } = string.Empty;

    private int RelatedEntityTypeInt { get; set; } = (int)EntityType.Unknown;
    public EntityType RelatedEntityType
    {
        get => (EntityType)RelatedEntityTypeInt;
        set => RelatedEntityTypeInt = (int)value;
    }

    private int RelatedEntitySubtypeInt { get; set; } = (int)EntitySubtype.Unknown;
    public EntitySubtype RelatedEntitySubtype
    {
        get => (EntitySubtype)RelatedEntitySubtypeInt;
        set => RelatedEntitySubtypeInt = (int)value;
    }

    public string ServiceRequestNumber { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ClientFlag { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string FinalFlag { get; set; } = string.Empty;
    public string FormDescription { get; set; } = string.Empty;
    public string IncidentId { get; set; } = string.Empty;
    public string IncidentNo { get; set; } = string.Empty;
    public string Internal { get; set; } = string.Empty;
    public string CaseNumber { get; set; } = string.Empty;
    public string PortalVisible { get; set; } = string.Empty;
    public string ShowOnContact { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string Template { get; set; } = string.Empty;
    public string TemplateType { get; set; } = string.Empty;
    public string CaseId { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public string FileAutoUpdFlg { get; set; } = string.Empty;
    public string FileDate { get; set; } = string.Empty;
    public string FileDeferFlg { get; set; } = string.Empty;
    public string FileDockReqFlg { get; set; } = string.Empty;
    public string FileDockStatFlg { get; set; } = string.Empty;
    public string FileSize { get; set; } = string.Empty;
    public string FileSrcPath { get; set; } = string.Empty;
    public string FileSrcType { get; set; } = string.Empty;
    public string MemoId { get; set; } = string.Empty;
    public string MemoNumber { get; set; } = string.Empty;
    public string ServiceRequestId { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;
    public string UpdatedBy { get; set; } = string.Empty;

    public byte[]? Thumbnail { get; set; }

    /// <summary>
    /// Relative path to file on virtualized file system. File name will be different than <see cref="Filename"/>.
    /// </summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>
    /// Virtual name of the attachment as stored in ICM, without the file type extension.
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// The file type extension including the dot '.'
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    public bool FileExistsLocally => RelativePath?.Trim().Length > 0;

    public int? SizeBytes => int.TryParse(FileSize, out int size) ? size : null;

    [Backlink(nameof(AttachmentDraft.Attachment))]
    public IQueryable<AttachmentDraft> AttachmentDrafts { get; } = null!;

#pragma warning disable RLM025 // RealmObject/EmbeddedObject properties usually indicate a relationship
    public AttachmentDraft? Draft => AttachmentDrafts.FirstOrDefault();
#pragma warning restore RLM025 // RealmObject/EmbeddedObject properties usually indicate a relationship

    public bool HasDraft => Draft != null;

    public string FileNumber
    {
        get
        {
            return RelatedEntityType switch
            {
                EntityType.Case => CaseNumber,
                EntityType.Incident => IncidentNo,
                EntityType.Memo => MemoNumber,
                EntityType.ServiceRequest => ServiceRequestNumber,
                _ => throw new NotImplementedException($"'{RelatedEntityType}' not implemented"),
            };
        }
        set
        {
            if (RelatedEntityType == EntityType.Case)
                CaseNumber = value;
            else if (RelatedEntityType == EntityType.Incident)
                IncidentNo = value;
            else if (RelatedEntityType == EntityType.Memo)
                MemoNumber = value;
            else if (RelatedEntityType == EntityType.ServiceRequest)
                ServiceRequestNumber = value;
            else
                throw new NotImplementedException($"'{RelatedEntityType}' not implemented");
        }
    }

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

    public Attachment() { }

    public Attachment(AttachmentJson json, string parentId, EntityType type)
    {
        Id = json.Id;
        RelatedEntityId = parentId;
        RelatedEntityType = type;
        ServiceRequestNumber = json.ApplicationNo;
        Categorie = json.Categorie;
        Category = json.Category;
        ClientFlag = json.ClientFlag;
        EndDate = json.EndDate;
        FinalFlag = json.FinalFlag;
        FormDescription = json.FormDescription;
        IncidentId = json.IncidentId;
        IncidentNo = json.IncidentNo;
        Internal = json.Internal;
        CaseNumber = json.NoIntervention;
        PortalVisible = json.PortalVisible;
        ShowOnContact = json.ShowOnContact;
        Status = json.Status;
        SubCategory = json.SubCategory;
        Template = json.Template;
        TemplateType = json.TemplateType;
        CaseId = json.CaseId;
        Comments = json.Comments;
        FileAutoUpdFlg = json.FileAutoUpdFlg;
        FileDate = json.FileDate;
        FileDeferFlg = json.FileDeferFlg;
        FileDockReqFlg = json.FileDockReqFlg;
        FileDockStatFlg = json.FileDockStatFlg;
        Extension = "." + json.FileExt?.Trim('.');
        FileSize = json.FileSize;
        FileSrcPath = json.FileSrcPath;
        FileSrcType = json.FileSrcType;
        Filename = json.FileName;
        MemoId = json.MemoId;
        MemoNumber = json.MemoNumber;
        ServiceRequestId = json.SrId;
        CreatedBy = json.CreatedByName;
        UpdatedBy = json.UpdatedByName;
        CreatedDate = DateTimeOffset.Parse(json.CreatedDate);
        UpdatedDate = DateTimeOffset.Parse(json.LastUpdatedDate);
    }

    public AttachmentJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            Id = Id,
            ApplicationNo = ServiceRequestNumber,
            Categorie = Categorie,
            Category = Category,
            ClientFlag = ClientFlag,
            EndDate = EndDate,
            FinalFlag = FinalFlag,
            FormDescription = FormDescription,
            IncidentId = IncidentId,
            IncidentNo = IncidentNo,
            Internal = Internal,
            NoIntervention = CaseNumber,
            PortalVisible = PortalVisible,
            ShowOnContact = ShowOnContact,
            Status = Status,
            SubCategory = SubCategory,
            Template = Template,
            TemplateType = TemplateType,
            CaseId = CaseId,
            Comments = Comments,
            FileAutoUpdFlg = FileAutoUpdFlg,
            FileDate = FileDate,
            FileDeferFlg = FileDeferFlg,
            FileDockReqFlg = FileDockReqFlg,
            FileDockStatFlg = FileDockStatFlg,
            FileExt = Extension?.Trim('.') ?? string.Empty,
            FileSize = FileSize,
            FileSrcPath = FileSrcPath,
            FileSrcType = FileSrcType,
            FileName = Filename,
            MemoId = MemoId,
            MemoNumber = MemoNumber,
            SrId = ServiceRequestId,
            CreatedByName = CreatedBy,
            UpdatedByName = UpdatedBy,
            CreatedDate = CreatedDate.ToString(dateFormat),
            LastUpdatedDate = UpdatedDate.ToString(dateFormat),
        };
    }

    public async Task<MemoryStream> GetFile(AttachmentFiler filer, CancellationToken? token = null)
    {
        return await filer.GetAppDataFileAsync(RelativePath, token);
    }

    public static IEnumerable<Attachment> FromApiArray(
        IEnumerable<AttachmentJson> items,
        string parentId,
        EntityType type
    )
    {
        List<Attachment> outList = [];

        foreach (var AttachmentJson in items)
            outList.Add(new Attachment(AttachmentJson, parentId, type));

        return outList;
    }

    public void CopyFrom(Attachment source)
    {
        // We are not overwriting RelativePath
        RelatedEntityId = source.RelatedEntityId;
        RelatedEntityType = source.RelatedEntityType;
        ServiceRequestNumber = source.ServiceRequestNumber;
        Categorie = source.Categorie;
        Category = source.Category;
        ClientFlag = source.ClientFlag;
        EndDate = source.EndDate;
        FinalFlag = source.FinalFlag;
        FormDescription = source.FormDescription;
        IncidentId = source.IncidentId;
        IncidentNo = source.IncidentNo;
        Internal = source.Internal;
        CaseNumber = source.CaseNumber;
        PortalVisible = source.PortalVisible;
        ShowOnContact = source.ShowOnContact;
        Status = source.Status;
        SubCategory = source.SubCategory;
        Template = source.Template;
        TemplateType = source.TemplateType;
        CaseId = source.CaseId;
        Comments = source.Comments;
        FileAutoUpdFlg = source.FileAutoUpdFlg;
        FileDate = source.FileDate;
        FileDeferFlg = source.FileDeferFlg;
        FileDockReqFlg = source.FileDockReqFlg;
        FileDockStatFlg = source.FileDockStatFlg;
        Extension = source.Extension;
        FileSize = source.FileSize;
        FileSrcPath = source.FileSrcPath;
        FileSrcType = source.FileSrcType;
        Filename = source.Filename;
        MemoId = source.MemoId;
        MemoNumber = source.MemoNumber;
        ServiceRequestId = source.ServiceRequestId;
        CreatedBy = source.CreatedBy;
        UpdatedBy = source.UpdatedBy;
        CreatedDate = source.CreatedDate;
        UpdatedDate = source.UpdatedDate;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<AttachmentJson> items,
        string parentId,
        EntityType type
    )
    {
        // Issues with Realm object lifetime and IEnumerable, so materialize everything to lists instead
        var incomingAttachments = FromApiArray(items, parentId, type);
        var existingAttachments = GetAttachments(realm, type, parentId).ToList();
        var remove = existingAttachments.Except(incomingAttachments).ToList();

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                foreach (Attachment item in remove)
                {
                    if (item.IsValid)
                    {
                        if (item.FileExistsLocally)
                            item.RemoveFileFromDevice();
                        realm.Remove(item);
                    }
                }

                foreach (var upsertAttachment in incomingAttachments)
                {
                    if (realm.Find<Attachment>(upsertAttachment.Id) is Attachment existing)
                        existing.CopyFrom(upsertAttachment);
                    else
                        realm.Add(upsertAttachment);
                }
            }
        );
    }

    public static IQueryable<Attachment> GetAttachments(Realm realm, EntityType type, string recordId)
    {
        return realm
            .All<Attachment>()
            .Where(item => item.RelatedEntityTypeInt == (int)type && item.RelatedEntityId == recordId);
    }

    public static IOrderedQueryable<Attachment> GetOrderedAttachments(Realm realm, EntityType type, string recordId)
    {
        return GetAttachments(realm, type, recordId).OrderByDescending(item => item.CreatedDate);
    }

    public static async Task DeleteAsync(Realm realm, Attachment attachment, bool removeContent = true)
    {
        ArgumentNullException.ThrowIfNull(realm);

        string fullpath = AttachmentFiler.GetFullPath(attachment.RelativePath);

        if (removeContent && File.Exists(fullpath))
            File.Delete(fullpath);

        await attachment.CommitAsync(() =>
        {
            realm.Remove(attachment);
        });
    }

    public async Task DeleteAsync(bool removeContent = true)
    {
        if (Realm != null)
            await DeleteAsync(Realm, this, removeContent);
    }

    public void RemoveFileFromDevice()
    {
        AttachmentFiler.DeleteFileFromDevice(RelativePath);
        RelativePathBinding = string.Empty;
    }

    public static void RemoveByParent(
        Realm realm,
        EntityType type,
        string parentId,
        UserIgnoredContentPrefs userIgnoredPrefs
    )
    {
        var attachmentItems = realm
            .All<Attachment>()
            .Where(item => item.RelatedEntityId == parentId && item.RelatedEntityTypeInt == (int)type)
            .ToList();

        foreach (var item in attachmentItems)
        {
            if (item.FileExistsLocally)
                item.RemoveFileFromDevice();
            userIgnoredPrefs?.RemoveUserIgnoredContent(item.Id);
            realm.Remove(item);
        }
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

    public override bool Equals(object? obj)
    {
        return obj is Attachment attachment ? Equals(attachment) : base.Equals(obj);
    }

    public override int GetHashCode()
    {
#pragma warning disable SS008 // GetHashCode() refers to mutable or static member
        // Id is not meant to change
        return Id.GetHashCode();
#pragma warning restore SS008 // GetHashCode() refers to mutable or static member
    }

    public bool Equals(Attachment? other)
    {
        return ReferenceEquals(this, other) || Id == other?.Id;
    }
}
