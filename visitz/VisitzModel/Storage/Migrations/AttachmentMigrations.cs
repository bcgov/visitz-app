using Realms;
using VisitzModel.Models.Attachments;
using VisitzModel.Storage.Filesystem;

namespace VisitzModel.Storage.Migrations;

internal static class AttachmentMigrations
{
    public static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version3_0_0)
        {
            VisitzRealmBase.MapAll<Attachment>(
                "Attachment",
                migration,
                (n, o) =>
                {
                    n.Id = o.DynamicApi.Get<string>("Id") ?? Guid.NewGuid().ToString();
                    n.RelatedEntityId = o.DynamicApi.Get<string>("RelatedEntityId") ?? string.Empty;
                    n.RelatedEntityTypeInt = o.DynamicApi.Get<int>("RelatedEntityTypeInt");
                    n.RelatedEntitySubtypeInt = o.DynamicApi.Get<int>("RelatedEntitySubtypeInt");
                    n.ServiceRequestNumber = o.DynamicApi.Get<string>("ServiceRequestNumber") ?? string.Empty;
                    n.Categorie = o.DynamicApi.Get<string>("Categorie") ?? string.Empty;
                    n.Category = o.DynamicApi.Get<string>("Category") ?? string.Empty;
                    n.ClientFlag = o.DynamicApi.Get<string>("ClientFlag") ?? string.Empty;
                    n.EndDate = o.DynamicApi.Get<string>("EndDate") ?? string.Empty;
                    n.FinalFlag = o.DynamicApi.Get<string>("FinalFlag") ?? string.Empty;
                    n.FormDescription = o.DynamicApi.Get<string>("FormDescription") ?? string.Empty;
                    n.IncidentId = o.DynamicApi.Get<string>("IncidentId") ?? string.Empty;
                    n.IncidentNo = o.DynamicApi.Get<string>("IncidentNo") ?? string.Empty;
                    n.Internal = o.DynamicApi.Get<string>("Internal") ?? string.Empty;
                    n.CaseNumber = o.DynamicApi.Get<string>("CaseNumber") ?? string.Empty;
                    n.PortalVisible = o.DynamicApi.Get<string>("PortalVisible") ?? string.Empty;
                    n.ShowOnContact = o.DynamicApi.Get<string>("ShowOnContact") ?? string.Empty;
                    n.Status = o.DynamicApi.Get<string>("Status") ?? string.Empty;
                    n.SubCategory = o.DynamicApi.Get<string>("SubCategory") ?? string.Empty;
                    n.Template = o.DynamicApi.Get<string>("Template") ?? string.Empty;
                    n.TemplateType = o.DynamicApi.Get<string>("TemplateType") ?? string.Empty;
                    n.CaseId = o.DynamicApi.Get<string>("CaseId") ?? string.Empty;
                    n.Comments = o.DynamicApi.Get<string>("Comments") ?? string.Empty;
                    n.FileAutoUpdFlg = o.DynamicApi.Get<string>("FileAutoUpdFlg") ?? string.Empty;
                    n.FileDate = o.DynamicApi.Get<string>("FileDate") ?? string.Empty;
                    n.FileDeferFlg = o.DynamicApi.Get<string>("FileDeferFlg") ?? string.Empty;
                    n.FileDockReqFlg = o.DynamicApi.Get<string>("FileDockReqFlg") ?? string.Empty;
                    n.FileDockStatFlg = o.DynamicApi.Get<string>("FileDockStatFlg") ?? string.Empty;
                    n.FileSrcPath = o.DynamicApi.Get<string>("FileSrcPath") ?? string.Empty;
                    n.FileSrcType = o.DynamicApi.Get<string>("FileSrcType") ?? string.Empty;
                    n.MemoId = o.DynamicApi.Get<string>("MemoId") ?? string.Empty;
                    n.MemoNumber = o.DynamicApi.Get<string>("MemoNumber") ?? string.Empty;
                    n.ServiceRequestId = o.DynamicApi.Get<string>("ServiceRequestId") ?? string.Empty;
                    n.CreatedBy = o.DynamicApi.Get<string>("CreatedBy") ?? string.Empty;
                    n.CreatedDate = o.DynamicApi.Get<DateTimeOffset>("CreatedDate");
                    n.UpdatedDate = o.DynamicApi.Get<DateTimeOffset>("UpdatedDate");
                    n.UpdatedBy = o.DynamicApi.Get<string>("UpdatedBy") ?? string.Empty;
                    n.Thumbnail = o.DynamicApi.Get<byte[]?>("Thumbnail");
                    n.RelativePath = o.DynamicApi.Get<string>("RelativePath") ?? string.Empty;
                    n.Filename = o.DynamicApi.Get<string>("Filename") ?? string.Empty;
                    n.Extension = o.DynamicApi.Get<string>("Extension") ?? string.Empty;

                    if (n.RelativePath.Length > 0 && n.FileSize.Length == 0)
                    {
                        string filepath = AttachmentFiler.GetFullPath(n.RelativePath);
                        n.FileSize = new FileInfo(filepath).Length.ToString();
                    }
                }
            );
        }
    }
}
