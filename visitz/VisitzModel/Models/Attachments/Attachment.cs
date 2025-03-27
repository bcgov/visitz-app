using System.Data;
using Realms;
using VisitzApi.Models.Attachments;
using VisitzModel.Extensions;
using VisitzModel.Formats;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Storage.Filesystem;

namespace VisitzModel.Models.Attachments;

public partial class Attachment : IRealmObject, IRecordInfo, IApiJson<AttachmentJson>
{
    public static readonly int MaxFilesize = 5 * Sizes.MB;
    public static readonly int ThumbnailSize = 400;

    public static readonly IEnumerable<string> AllowedImageTypes = [".jpg", ".jpeg"];
    public static readonly IEnumerable<string> AllowedDocumentTypes = [".pdf"];

    [PrimaryKey]
    public string Id {get; set;} = Guid.NewGuid().ToString();

    public string RelatedEntityId { get; set; }

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

    public string ServiceRequestNumber { get; set; }
    public string Categorie { get; set; }
    public string Category { get; set; }
    public string ClientFlag { get; set; }
    public string EndDate { get; set; }
    public string FinalFlag { get; set; }
    public string FormDescription { get; set; }
    public string IncidentId { get; set; }
    public string IncidentNo { get; set; }
    public string Internal { get; set; }
    public string CaseNumber { get; set; }
    public string PortalVisible { get; set; }
    public string ShowOnContact { get; set; }
    public string Status { get; set; }
    public string SubCategory { get; set; }
    public string Template { get; set; }
    public string TemplateType { get; set; }
    public string CaseId { get; set; }
    public string Comments { get; set; }
    public string FileAutoUpdFlg { get; set; }
    public string FileDate { get; set; }
    public string FileDeferFlg { get; set; }
    public string FileDockReqFlg { get; set; }
    public string FileDockStatFlg { get; set; }
    public string FileSize { get; set; }
    public string FileSrcPath { get; set; }
    public string FileSrcType { get; set; }
    public string MemoId { get; set; }
    public string MemoNumber { get; set; }
    public string ServiceRequestId { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }

    public byte[] Thumbnail { get; set; }

    /// <summary>
    /// Relative path to file on virtualized file system. File name will be different than <see cref="Filename"/>.
    /// </summary>
    public string RelativePath { get; set; }

    /// <summary>
    /// Virtual name of the attachment as stored in ICM, without the file type extension.
    /// </summary>
    public string Filename { get; set; }

    /// <summary>
    /// The file type extension including the dot '.'
    /// </summary>
    public string Extension { get; set; }

    [Backlink(nameof(AttachmentDraft.Attachment))]
    public IQueryable<AttachmentDraft> AttachmentDrafts { get; }

#pragma warning disable RLM025 // RealmObject/EmbeddedObject properties usually indicate a relationship
    public AttachmentDraft Draft => AttachmentDrafts.FirstOrDefault();
#pragma warning restore RLM025 // RealmObject/EmbeddedObject properties usually indicate a relationship

    public bool HasDraft => Draft != null;

    public static async Task DeleteAsync(Realm realm, Attachment attachment)
    {
        string fullpath = AttachmentFiler.GetFullPath(attachment.RelativePath);

        if (File.Exists(fullpath))
            File.Delete(fullpath);

        await attachment.CommitAsync(() =>
        {
            if (attachment.HasDraft)
                realm.Remove(attachment.Draft);

            realm.Remove(attachment);
        });
    }

    public async Task DeleteAsync()
    {
        await DeleteAsync(Realm, this);
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
            FileExt = Extension?.Trim('.'),
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

    public static IEnumerable<Attachment> FromApiArray(
        IEnumerable<AttachmentJson> items,
        string parentId,
        EntityType type)
    {
        List<Attachment> outList = [];

        foreach (var AttachmentJson in items)
            outList.Add(new Attachment(AttachmentJson, parentId, type));

        return outList;
    }

    public static Attachment FromApiArray(
        AttachmentJson item,
        string parentId,
        EntityType type)
    {
        return new Attachment(item, parentId, type);
    }

    public static async Task SaveAttachmentsAsync(
        Realm realm,
        IEnumerable<AttachmentJson> items,
        string parentId,
        EntityType type)
    {
        await RealmExtensions.CommitAsync(realm, () => realm.Upsert(FromApiArray(items, parentId, type)));
    }

        public static async Task SaveAttachmentAndDetailsAsync(
        Realm realm,
        AttachmentJson item,
        string parentId,
        EntityType type,
        AttachmentFiler filer)
    {
        // if (string.IsNullOrWhiteSpace(item.AttachmentId))
        //     throw new InvalidDataException("Expected attachment content is null");
        if (!string.IsNullOrWhiteSpace(item.AttachmentId))
        {
            string fullpath = await filer.SaveFileAsync(item.AttachmentId, item.FileExt);
            await RealmExtensions.CommitAsync(realm, () => realm.Upsert(FromApiArray(item, parentId, type)));
        }
    }

    public static IEnumerable<Attachment> GetAttachments(Realm realm, EntityType type, string recordId)
    {
        var attachments = realm.All<Attachment>()
            .Where(item => item.RelatedEntityTypeInt == (int)type && item.RelatedEntityId == recordId)
            .OrderByDescending(item => item.CreatedDate);
        return attachments;
    }
}
