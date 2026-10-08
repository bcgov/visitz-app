using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.Banners;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Models;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitListViewModel : IcmRecordViewModel, IRequestedEntitySection
{
    private bool _disposed;

    ObservableQuery<PersonVisitDraft>? _draftsQuery;

    readonly IComparer<ChildYouthVisitListItemViewModel> _insertComparer =
        Comparer<ChildYouthVisitListItemViewModel>.Create(
            (l, r) => l.Visit.DateOfVisitBinding.CompareTo(r.Visit.DateOfVisitBinding)
        );

    public EntitySection RequestedSection { get; set; }

    [ObservableProperty]
    public partial ObservableQuery<PersonVisit, ChildYouthVisitListItemViewModel>? VisitsQuery { get; set; }

    [ObservableProperty]
    public partial DateTimeOffset DateOfVisit { get; set; }

    [ObservableProperty]
    public partial AlertLevel BannerLevel { get; set; }

    [ObservableProperty]
    public partial string BannerText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasVisitData { get; set; } = true;

    [ObservableProperty]
    public partial bool ShowEmptyIcon { get; set; } = false;

    [ObservableProperty]
    public partial string OpenAddVisitIconGlyph { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string OpenAddVisitText { get; set; } = string.Empty;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        VisitsQuery = new(
            DataRealm,
            PersonVisit.GetVisitsByCaseId(DataRealm, BusinessObject.Id),
            addItem: (item, targetList) => targetList.InsertSorted(new(item), _insertComparer, ascending: false),
            removeItem: TryRemove
        );
        VisitsQuery.Items.CollectionChanged += VisitsQueryItems_CollectionChanged;

        Realm visitDraftRealm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
        _draftsQuery = new(
            visitDraftRealm,
            visitDraftRealm.All<PersonVisitDraft>().Where(visit => visit.RelatedEntityId == BusinessObject.Id)
        );
        _draftsQuery.Items.CollectionChanged += DraftVisits_CollectionChanged;

        ApplyDraftButtonText(false);

        if (RequestedSection == EntitySection.ChildYouthVisitsEntry)
            await OpenVisitEntry();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            VisitsQuery?.Items.CollectionChanged -= VisitsQueryItems_CollectionChanged;
            VisitsQuery?.Dispose();
            VisitsQuery = null;

            _draftsQuery?.Items.CollectionChanged -= DraftVisits_CollectionChanged;
            _draftsQuery?.Dispose();
            _draftsQuery = null;

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void VisitsQueryItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdatePersonVisitRelatedInfo();
    }

    private void DraftVisits_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ApplyDraftButtonText(isDraftAvailable: (sender as ObservableCollection<PersonVisitDraft>)?.Any() ?? false);
    }

    private void UpdatePersonVisitRelatedInfo()
    {
        HasVisitData = VisitsQuery?.HasAnyItems ?? false;
        ShowEmptyIcon = !HasVisitData;

        // Note that if we ever change sort order, we need to query for latest
        // visit instead of just using FirstOrDefault here
        if (VisitsQuery?.Items.FirstOrDefault() is ChildYouthVisitListItemViewModel lastVisit)
            SetBannerInfo(lastVisit.Visit);
    }

    private void SetBannerInfo(PersonVisit personVisit)
    {
        string dateInBanner = personVisit.DueDate.ToString("MMMM d, yyyy");
        var threshold = personVisit.CurrentDueDateThreshold;
        switch (threshold)
        {
            case VisitDaysThreshold.Info:
                BannerLevel = AlertLevel.Info;
                BannerText = string.Format(LocalizedStrings.NextVisitDueBy, dateInBanner);
                break;
            case VisitDaysThreshold.Warning:
                BannerLevel = AlertLevel.Warning;
                BannerText = string.Format(LocalizedStrings.VisitDueBy, dateInBanner);
                break;
            case VisitDaysThreshold.Danger:
                BannerLevel = AlertLevel.Danger;
                BannerText = string.Format(LocalizedStrings.VisitDueBy, dateInBanner);
                break;
            case VisitDaysThreshold.Critical:
            default:
                BannerLevel = AlertLevel.Critical;
                BannerText = string.Format(LocalizedStrings.OverdueVisitOn, dateInBanner);
                break;
        }
    }

    static void TryRemove(PersonVisit item, ObservableCollection<ChildYouthVisitListItemViewModel> targetList)
    {
        ChildYouthVisitListItemViewModel? found = targetList.FirstOrDefault(vm => vm.Visit.Id == item.Id);
        if (found != null)
            targetList.Remove(found);
    }

    void ApplyDraftButtonText(bool isDraftAvailable)
    {
        OpenAddVisitIconGlyph = isDraftAvailable ? MaterialIcons.Edit : MaterialIcons.Add;
        OpenAddVisitText = isDraftAvailable ? LocalizedStrings.ContinueDraft : LocalizedStrings.AddVisit;
    }

    [RelayCommand]
    public async Task OpenVisitEntry()
    {
        var visitEntryView = ServiceProvider.GetService<ChildYouthVisitView>();
        visitEntryView.ViewModel.RowId = RowId;
        visitEntryView.ViewModel.EntityType = EntityType;

        await Navigator.Navigation.PushModalAsync(visitEntryView, ViewModalSize.Wide);
    }
}
