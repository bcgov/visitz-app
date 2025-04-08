using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using System.Collections.ObjectModel;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;

namespace Visitz.Views.Entity.Attachments;

internal partial class AttachmentsListViewModel : VisitzViewModel, ICaseloadItemHolder
{
    private bool _disposed;

    readonly ObservableRealmQueryMap realmQuery = new();

    [ObservableProperty]
    public ObservableCollection<Attachment> attachmentsList = [];

    [ObservableProperty]
    public CaseloadItem caseloadItem;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        Realm icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;

        realmQuery.Subscribe(icmDataRealm, Attachment.GetOrderedAttachments(icmDataRealm, CaseloadItem.EntityType.ParseEntityType(), CaseloadItem.CaseIncidentNumber));
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
                AttachmentsList.Add(item as Attachment);
        }
        else
        {
            foreach (int deleted in changes.DeletedIndices.Reverse())
                AttachmentsList.RemoveAt(deleted);

            foreach (int inserted in changes.InsertedIndices)
                AttachmentsList.Add(items[inserted] as Attachment);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            realmQuery.ItemsChanged -= RealmQuery_ItemsChanged;
            realmQuery.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
