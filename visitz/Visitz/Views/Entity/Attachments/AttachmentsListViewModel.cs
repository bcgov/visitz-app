using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using System.Collections.ObjectModel;
using Visitz.Storage;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models.Attachments;
using Visitz.Services.Attachments;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Services;

namespace Visitz.Views.Entity.Attachments;

internal partial class AttachmentsListViewModel : VisitzViewModel, ICaseloadItemHolder
{
    private bool _disposed;

    readonly ObservableRealmQueryMap realmQuery = new();

    [ObservableProperty]
    public ObservableCollection<AttachmentsListItemUi> attachmentsList = [];

    [ObservableProperty]
    public CaseloadItem caseloadItem;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        Realm icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;

        realmQuery.Subscribe(icmDataRealm, Attachment.GetOrderedAttachments(icmDataRealm, CaseloadItem.EntityType.ParseEntityType(), CaseloadItem.RowId));
    }

    private void RealmQuery_ItemsChanged(object sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
    {
        if (e.Type == typeof(Attachment))
            UpdateAttachmentsList(e.Items, e.Changes);
    }

    private void UpdateAttachmentsList(IRealmCollection<IRealmObject> items, ChangeSet changes)
    {
        if (changes == null)
        {
            foreach (var item in items)
                AttachmentsList.Add(new AttachmentsListItemUi(
                    CaseloadItem.EntityType.ParseEntityType(),
                    CaseloadItem.RowId,
                    item as Attachment));
        }
        else
        {
            foreach (int deleted in changes.DeletedIndices.Reverse())
            {
                AttachmentsList.ElementAt(deleted).Dispose();
                AttachmentsList.RemoveAt(deleted);
            }

            foreach (int modified in changes.ModifiedIndices)
                AttachmentsList[modified] = new AttachmentsListItemUi(
                    CaseloadItem.EntityType.ParseEntityType(),
                    CaseloadItem.RowId,
                    items[modified] as Attachment);

            foreach (int inserted in changes.InsertedIndices)
                AttachmentsList.Insert(inserted, new AttachmentsListItemUi(
                    CaseloadItem.EntityType.ParseEntityType(),
                    CaseloadItem.RowId,
                    items[inserted] as Attachment));
        }
    }

    [RelayCommand]
    public static void DeleteDownloadedAttachmentFromDevice(AttachmentsListItemUi item)
    {
        _ = PromptRemoveAttachmentAsync(item);
    }

    static async Task PromptRemoveAttachmentAsync(AttachmentsListItemUi item)
    {
        bool shouldRemove = await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.RemoveAttachmentFromDevice,
            LocalizedStrings.RemoveAttachmentDescription,
            LocalizedStrings.Remove,
            LocalizedStrings.Cancel);

        if (shouldRemove)
        {
            item.Attachment.RemoveFileFromDevice();
            string removedText = string.Format(LocalizedStrings.RemovedAttachmentFromDevice, item.Attachment.Filename);
            SnackbarHandler.ShowText(removedText);
        }
    }

    [RelayCommand]
    public void DownloadAttachmentForDevice(AttachmentsListItemUi item)
    {
        var recordServiceInfo = new RecordServiceInfo(
            CaseloadItem.EntityType.ParseEntityType(),
            CaseloadItem.RowId,
            CaseloadItem.CaseIncidentNumber,
            CaseloadItem.KeyPlayer.FirstName,
            CaseloadItem.KeyPlayer.LastName);
        var attachmentId = item.Attachment.Id;
        var force = true;

        var tuple = (recordServiceInfo, attachmentId, force);
        WeakReferenceMessenger.Default.Send(GetAttachmentContentService.MakeStartMessage(tuple));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            realmQuery.ItemsChanged -= RealmQuery_ItemsChanged;
            realmQuery.Dispose();

            foreach (var item in AttachmentsList)
                item.Dispose();

            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
