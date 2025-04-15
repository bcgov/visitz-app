using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using System.Collections.ObjectModel;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

internal partial class AttachmentsListViewModel : VisitzViewModel, ICaseloadItemHolder
{
    private bool _disposed;

    readonly ObservableRealmQueryMap realmQuery = new();

    [ObservableProperty]
    public ObservableCollection<AttachmentsListItemUi> attachmentsList = [];

    [ObservableProperty]
    public CaseloadItem? caseloadItem;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        Realm icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;

        if (CaseloadItem == null)
            throw new InvalidOperationException(nameof(CaseloadItem));

        realmQuery.Subscribe(icmDataRealm, Attachment.GetOrderedAttachments(
            icmDataRealm,
            CaseloadItem.EntityType.ParseEntityType(),
            CaseloadItem.RowId));
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

    private void RealmQuery_ItemsChanged(object? sender,
        (Type Type,
        IRealmCollection<IRealmObject> Items,
        ChangeSet Changes) e)
    {
        if (e.Type == typeof(Attachment))
            UpdateAttachmentsList(e.Items, e.Changes);
    }

    private void UpdateAttachmentsList(IRealmCollection<IRealmObject> items, ChangeSet changes)
    {
        if (changes == null)
        {
            foreach (var item in items)
                AttachmentsList.Add(MakeItemUi(item as Attachment));
        }
        else
        {
            foreach (int deleted in changes.DeletedIndices.Reverse())
            {
                AttachmentsList.ElementAt(deleted).Dispose();
                AttachmentsList.RemoveAt(deleted);
            }

            foreach (int modified in changes.ModifiedIndices)
                AttachmentsList[modified] = MakeItemUi(items[modified] as Attachment);

            foreach (int inserted in changes.InsertedIndices)
                AttachmentsList.Insert(inserted, MakeItemUi(items[inserted] as Attachment));
        }
    }

    AttachmentsListItemUi MakeItemUi(Attachment? attachment)
    {
        return new AttachmentsListItemUi(
            CaseloadItem?.EntityType.ParseEntityType() ?? EntityType.Unknown,
            CaseloadItem?.RowId,
            attachment);
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
            string removedText = string.Format(
                LocalizedStrings.RemovedAttachmentFromDevice,
                item.Attachment.Filename);
            SnackbarHandler.ShowText(removedText);
        }
    }

    [RelayCommand]
    public void DownloadAttachmentForDevice(AttachmentsListItemUi listItem)
    {
        if (CaseloadItem is CaseloadItem item)
        {
            var recordServiceInfo = new RecordServiceInfo(
                item.EntityType.ParseEntityType(),
                item.RowId,
                item.CaseIncidentNumber,
                item.KeyPlayer.FirstName,
                item.KeyPlayer.LastName);
            var attachmentId = listItem.Attachment.Id;
            var force = true;

            var tuple = (recordServiceInfo, attachmentId, force);
            var msg = GetAttachmentContentService.MakeStartMessage(tuple);
            WeakReferenceMessenger.Default.Send(msg);
        }
        else
            throw new InvalidOperationException(nameof(CaseloadItem));
    }

    [RelayCommand]
    public async Task OpenAttachment(AttachmentsListItemUi listItem)
    {

    }
}
