using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

internal partial class AttachmentsViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public IDraftItem focusedDraftItem;

    Realm AttachmentsRealm { get; set; }

    AttachmentFiler attachmentFiler;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        AttachmentsRealm = await VisitzRealms.GetAttachmentDraftsRealmAsync();
        attachmentFiler = await VisitzFiles.GetAsync(
            CaseloadItem.EntityType.ParseEntityType(),
            CaseloadItem.CaseIncidentNumber,
            CaseloadItem.KeyPlayer.FirstName,
            CaseloadItem.KeyPlayer.LastName);
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            AttachmentsRealm?.Dispose();

            disposed = true;
        }
        base.Dispose(disposing);
    }

    public async Task SaveFile(FileResult fileResult)
    {
        string extension = fileResult.FileName.GetFileExtension();
        await using Stream stream = await fileResult.OpenReadAsync();

        if (Attachment.AllowedImageTypes.Contains(extension.ToLowerInvariant()))
            await AttachmentDraft.SaveNewPhoto(CaseloadItem, attachmentFiler, AttachmentsRealm, fileResult.FileName, stream);
        else
            await AttachmentDraft.SaveNewFile(CaseloadItem, attachmentFiler, AttachmentsRealm, fileResult.FileName, stream);
    }
}
