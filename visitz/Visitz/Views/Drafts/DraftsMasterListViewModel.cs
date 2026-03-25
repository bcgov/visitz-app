using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Foldable;
using System.Collections.ObjectModel;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Drafts;

internal partial class DraftsMasterListViewModel : VisitzViewModel
{
    [ObservableProperty]
    public ObservableCollection<object> masterDraftItems = [];

    [ObservableProperty]
    public MasterDraftItem selectedItem;

    [ObservableProperty]
    public bool showEmptyView;

    readonly ObservableRealmCount realmCount = new();

    [ObservableProperty]
    MasterDraftItem noteDraftItem = new()
    {
        Name = LocalizedStrings.Notes,
        ItemType = typeof(NoteDraft),
    };

    [ObservableProperty]
    MasterDraftItem assessmentDraftItem = new()
    {
        Name = LocalizedStrings.SafetyAssessments,
        ItemType = typeof(AssessmentDraft),
    };

    [ObservableProperty]
    MasterDraftItem attachmentsDraftItem = new()
    {
        Name = LocalizedStrings.Attachments,
        ItemType = typeof(AttachmentDraft),
    };

    [ObservableProperty]
    MasterDraftItem visitsDraftItem = new()
    {
        Name = LocalizedStrings.ChildYouthVisits,
        ItemType = typeof(PersonVisitDraft),
    };

    [ObservableProperty]
    public bool showMenuButton;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        realmCount.CountChanged += RealmCount_CountChanged;

        realmCount.Subscribe<AttachmentDraft>(await VisitzRealms.GetAttachmentDraftsRealmAsync());
        realmCount.Subscribe<NoteDraft>(await VisitzRealms.GetNoteDraftsRealmAsync());
        realmCount.Subscribe<AssessmentDraft>(await VisitzRealms.GetSafetyAssessmentDraftRealmAsync());
        realmCount.Subscribe<PersonVisitDraft>(await VisitzRealms.GetPersonVisitDraftsRealmAsync());

        StrongReferenceMessenger.Default.Register<NavPositionMessage>(this, ReceiveNavPositionMessage);
        ShowMenuButton = StrongReferenceMessenger.Default.Send(new GetNavPositionMessage()) == ((int)TwoPaneViewMode.Tall);
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            realmCount.CountChanged -= RealmCount_CountChanged;
            realmCount.Dispose();

            StrongReferenceMessenger.Default.UnregisterAll(this);

            disposed = true;
        }

        base.Dispose(disposing);
    }

    private void RealmCount_CountChanged(object sender, (Type Kind, int Count) e)
    {
        ShowEmptyView = (sender as ObservableRealmCount).Total <= 0;

        if (e.Kind == typeof(NoteDraft))
            UpdateItem(NoteDraftItem, e.Count);
        else if (e.Kind == typeof(AssessmentDraft))
            UpdateItem(AssessmentDraftItem, e.Count);
        else if (e.Kind == typeof(AttachmentDraft))
            UpdateItem(AttachmentsDraftItem, e.Count);
        else if (e.Kind == typeof(PersonVisitDraft))
            UpdateItem(VisitsDraftItem, e.Count);
    }

    void UpdateItem(MasterDraftItem item, int count)
    {
        item.Count = count;

        if (count <= 0)
            MasterDraftItems.Remove(item);
        else if (!MasterDraftItems.Contains(item))
            InsertSortedAsc(MasterDraftItems, item);
    }

    [RelayCommand]
    public void MasterDraftItemSelected()
    {
        var kind = SelectedItem.ItemType;
        var msg = new DraftMasterSelectedMessage(kind, realmCount[kind].Realm);
        StrongReferenceMessenger.Default.Send(msg);
    }

    // TODO: Use the IList<T>.InsertSortedAsc once MAUI fixes ObservableCollection<object> issues.
    // https://github.com/dotnet/maui/issues/8435#issuecomment-1365586648
    static void InsertSortedAsc(ObservableCollection<object> collection, MasterDraftItem newDraft)
    {
        if (collection.Count == 0)
            collection.Add(newDraft);
        else
        {
            var find = collection.FirstOrDefault(obj => (obj as MasterDraftItem).CompareTo(newDraft) >= 0);
            if (find != null)
                collection.Insert(collection.IndexOf(find), newDraft);
            else
                collection.Add(newDraft);
        }
    }

    [RelayCommand]
    public static void OpenNavDrawer()
    {
        StrongReferenceMessenger.Default.Send(new NavDrawerMessage(isOpen: true));
    }

    void ReceiveNavPositionMessage(object recipient, NavPositionMessage message)
    {
        ShowMenuButton = ((TwoPaneViewMode)message.Value) == TwoPaneViewMode.Tall;
    }
}
