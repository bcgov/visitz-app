using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Extensions;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;
using VisitzModel.Storage;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsListViewModel : IcmRecordViewModel
{
    private bool _disposed;

    Realm? _draftRealm;

    IQueryable<Attachment>? _draftQuery;

    IQueryable<Attachment>? _downloadedQuery;

    IDisposable? _draftQueryToken;

    IDisposable? _downloadedQueryToken;

    readonly ObservableCollection<Attachment> _draftAttachments = [];

    readonly ObservableCollection<Attachment> _downloadedAttachments = [];

    [ObservableProperty]
    public partial ObservableCollection<AttachmentsListItemUi> AttachmentsList { get; set; } = [];

    readonly IComparer<AttachmentsListItemUi> _insertComparer = Comparer<AttachmentsListItemUi>.Create(
        (l, r) =>
        {
            int draftCompare = l.Attachment.HasDraft.CompareTo(r.Attachment.HasDraft);
            if (draftCompare != 0)
                return draftCompare;

            int downloadedCompare = l.Attachment.FileExistsLocally.CompareTo(r.Attachment.FileExistsLocally);
            if (downloadedCompare != 0)
                return downloadedCompare;

            int createdCompare = l.Attachment.CreatedDate.CompareTo(r.Attachment.CreatedDate);
            return createdCompare;
        }
    );

    [ObservableProperty]
    public partial bool IsLoading { get; set; } = true;

    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    public UserIgnoredContentPrefs? UserIgnoredContentPrefs { get; set; }

    public TaskCompletionSource AttachmentsLoadedTcs { get; } = new();

    public AttachmentsListViewModel()
    {
        UserIgnoredContentPrefs = new UserIgnoredContentPrefs(Preferences.Default);
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        _draftAttachments.CollectionChanged += SubListAttachments_CollectionChanged;
        _downloadedAttachments.CollectionChanged += SubListAttachments_CollectionChanged;

        _draftRealm = await VisitzRealms.GetAttachmentDraftsRealmAsync();
        _draftQuery = Attachment.GetAttachments(_draftRealm, EntityType, RowId);
        _draftQueryToken = _draftQuery.SubscribeForNotifications(DraftAttachments_ItemsChanged);

        _downloadedQuery = Attachment.GetAttachments(DataRealm, EntityType, RowId);
        _downloadedQueryToken = _downloadedQuery.SubscribeForNotifications(DownloadedAttachments_ItemsChanged);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _draftAttachments.CollectionChanged -= SubListAttachments_CollectionChanged;
            _downloadedAttachments.CollectionChanged -= SubListAttachments_CollectionChanged;

            _draftRealm?.Dispose();
            _draftQueryToken?.Dispose();
            _downloadedQueryToken?.Dispose();

            _draftQuery = null;
            _downloadedQuery = null;

            foreach (var item in AttachmentsList)
                item.Dispose();

            _draftAttachments.Clear();
            _downloadedAttachments.Clear();
            AttachmentsList.Clear();

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    void DraftAttachments_ItemsChanged(IRealmCollection<Attachment> sender, ChangeSet? changes)
    {
        UpdateList(sender, changes, _draftAttachments);
    }

    void DownloadedAttachments_ItemsChanged(IRealmCollection<Attachment> sender, ChangeSet? changes)
    {
        UpdateList(sender, changes, _downloadedAttachments);
    }

    void UpdateList(
        IRealmCollection<IRealmObject> items,
        ChangeSet? changes,
        ObservableCollection<Attachment> listToUpdate
    )
    {
        IsLoading = false;

        if (changes == null)
        {
            listToUpdate.AddAll(items.Cast<Attachment>());
        }
        else
        {
            foreach (int deleted in changes.DeletedIndices.Reverse())
            {
                listToUpdate.ElementAt(deleted).Dispose();
                listToUpdate.RemoveAt(deleted);
            }

            foreach (int inserted in changes.InsertedIndices)
                listToUpdate.Insert(inserted, (Attachment)items[inserted]);
        }
    }

    private void SubListAttachments_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (var attachment in e.NewItems.Cast<Attachment>())
                AttachmentsList.InsertSorted(MakeItemUi(attachment), _insertComparer, ascending: false);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (var attachment in e.OldItems.Cast<Attachment>())
                if (AttachmentsList.FirstOrDefault(vm => vm.Attachment == attachment) is AttachmentsListItemUi found)
                    AttachmentsList.Remove(found);
        }

        IsEmpty = !AttachmentsList.Any();
        AttachmentsLoadedTcs.TrySetResult();
    }

    AttachmentsListItemUi MakeItemUi(Attachment attachment)
    {
        return new AttachmentsListItemUi(BusinessObject.EntityType, BusinessObject.Id, attachment);
    }

    [RelayCommand]
    public void DeleteDownloadedAttachmentFromDevice(AttachmentsListItemUi item)
    {
        _ = PromptRemoveCachedAttachmentAsync(item);
    }

    /// <summary>
    /// Removes the cached file for a given attachment row that was downloaded
    /// from upstream server. Does not delete the file from that server.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task PromptRemoveCachedAttachmentAsync(AttachmentsListItemUi item)
    {
        bool shouldRemove = await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.RemoveAttachmentFromDevice,
            LocalizedStrings.RemoveAttachmentDescription,
            LocalizedStrings.Remove,
            LocalizedStrings.Cancel
        );

        if (shouldRemove)
        {
            item.Attachment.RemoveFileFromDevice();
            UserIgnoredContentPrefs?.SetUserIgnoredContent(item.Attachment.Id, true);
            string removedText = string.Format(LocalizedStrings.RemovedAttachmentFromDevice, item.Attachment.Filename);
            SnackbarHandler.ShowText(removedText);
        }
    }

    [RelayCommand]
    public void DownloadAttachmentForDevice(AttachmentsListItemUi listItem)
    {
        if (BusinessObject is IBusinessObject item)
        {
            var recordServiceInfo = new RecordServiceInfo(item);
            var attachmentId = listItem.Attachment.Id;
            var force = true;

            var tuple = (recordServiceInfo, attachmentId, force);
            var msg = GetAttachmentContentService.MakeStartMessage(tuple);
            WeakReferenceMessenger.Default.Send(msg);
            UserIgnoredContentPrefs?.SetUserIgnoredContent(attachmentId, false);
        }
        else
            throw new InvalidOperationException(nameof(IBusinessObject));
    }

    [RelayCommand]
    public async Task OpenAttachment(AttachmentsListItemUi listItem)
    {
        Attachment attachment = listItem.Attachment;

        if (!attachment.FileExistsLocally || !File.Exists(AttachmentFiler.GetFullPath(attachment.RelativePath)))
        {
            if (attachment.HasDraft)
            {
                await Navigator.CurrentOpenPage.DisplayAlertAsync(
                    LocalizedStrings.FileMissing,
                    LocalizedStrings.RelatedDraftAttachmentMissingDesc,
                    LocalizedStrings.Ok
                );
            }

            return;
        }

        string path = attachment.RelativePath.Trim();

        BaseContentView view = path.EndsWith(Attachment.Pdf.Trim('.'))
            ? MakePdfDetailsView(attachment)
            : MakePhotoDetailsView(attachment);

        await Navigator.Navigation.PushAsync(view);
    }

    PhotoDetailsView MakePhotoDetailsView(Attachment attachment)
    {
        return new()
        {
            Attachment = attachment,
            RowId = RowId,
            EntityType = EntityType,
        };
    }

    PdfDetailsView MakePdfDetailsView(Attachment attachment)
    {
        return new()
        {
            Attachment = attachment,
            RowId = RowId,
            EntityType = EntityType,
        };
    }
}
