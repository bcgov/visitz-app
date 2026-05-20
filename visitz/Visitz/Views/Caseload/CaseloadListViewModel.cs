using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Realms;
using Visitz.Controls;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadListViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
{
#if WINDOWS
    private static readonly string PromptText = LocalizedStrings.ButtonToRefreshCaseload;
#else
    private static readonly string PromptText = LocalizedStrings.PullToRefreshCaseload;
#endif

    bool _disposed;

    DraftIndicatorHelper DraftIndicatorHelper { get; } = new();

    OidcSessionInfo? SessionInfo { get; set; }

    ObservableRealmQueryMap QueryMap { get; } = new();

    ObservableCollection<CaseRecord> CaseRecords { get; set; } = [];

    ObservableCollection<IncidentRecord> IncidentRecords { get; set; } = [];

    ObservableCollection<IBusinessObject> AllItems { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<CaseloadItemViewModel> FilteredItems { get; set; } = [];

    [ObservableProperty]
    public partial string SearchQuery { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? CollectionViewPrompt { get; set; } = PromptText;

    [ObservableProperty]
    public partial FilterOption<IBusinessObject>? SelectedOfficeFilter { get; set; }

    [ObservableProperty]
    public partial FilterOption<IBusinessObject> SelectedFilter { get; set; } =
        new(LocalizedStrings.AllTypes, _ => true);

    [ObservableProperty]
    public partial SortOption<CaseloadItemViewModel> SelectedSort { get; set; } =
        new(LocalizedStrings.Id, Comparer<CaseloadItemViewModel>.Default);

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    [ObservableProperty]
    public partial bool ShowEmptyCaseloadMessage { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        await DraftIndicatorHelper.InitTask;
        SessionInfo = await OidcSessionInfo.GetAsync();

        CaseRecords.CollectionChanged += SupportingRecordsCollection_CollectionChanged;
        IncidentRecords.CollectionChanged += SupportingRecordsCollection_CollectionChanged;
        AllItems.CollectionChanged += AllItems_CollectionChanged;

        var dataRealm = await VisitzRealms.GetIcmDataRealmAsync();

        QueryMap.ItemsChanged += QueryMap_ItemsChanged;
        QueryMap.Subscribe(dataRealm, dataRealm.All<CaseRecord>());
        QueryMap.Subscribe(dataRealm, dataRealm.All<IncidentRecord>());

        WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());
        WeakReferenceMessenger.Default.Send(AutoRefreshService.MakeStartMessage());
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            QueryMap.Dispose();

            AllItems.CollectionChanged -= AllItems_CollectionChanged;
            CaseRecords.CollectionChanged -= SupportingRecordsCollection_CollectionChanged;
            IncidentRecords.CollectionChanged -= SupportingRecordsCollection_CollectionChanged;

            DraftIndicatorHelper.Dispose();

            WeakReferenceMessenger.Default.UnregisterAll(this);

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    void QueryMap_ItemsChanged(object? sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e)
    {
        if (e.Type == typeof(CaseRecord))
            UpdateList(CaseRecords, e.Items, e.Changes);
        else if (e.Type == typeof(IncidentRecord))
            UpdateList(IncidentRecords, e.Items, e.Changes);
    }

    static void UpdateList<TRecord>(
        ObservableCollection<TRecord> records,
        IRealmCollection<IRealmObject> items,
        ChangeSet? changes
    )
        where TRecord : IBusinessObject
    {
        if (changes == null)
        {
            records.AddAll(items.Cast<TRecord>());
        }
        else
        {
            foreach (var deleteIndex in changes.DeletedIndices.Reverse())
                records.RemoveAt(deleteIndex);

            foreach (var insertIndex in changes.InsertedIndices)
                records.Insert(insertIndex, (TRecord)items.ElementAt(insertIndex));
        }
    }

    void SupportingRecordsCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            IEnumerable<IBusinessObject> newItems = e.NewItems.Cast<IBusinessObject>();

            foreach (var item in newItems)
                AllItems.Add(item);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            IEnumerable<IBusinessObject> oldItems = e.OldItems.Cast<IBusinessObject>();

            foreach (var item in oldItems)
                AllItems.Remove(item);
        }
    }

    private void AllItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(SessionInfo);

        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            IEnumerable<IBusinessObject> newItems = e.NewItems.Cast<IBusinessObject>();

            foreach (var item in newItems)
                if (MatchesFilters(item))
                {
                    CaseloadItemViewModel vm = new(DraftIndicatorHelper, item, SessionInfo);

                    FilteredItems.InsertSorted(vm, ascending: true, comparer: SelectedSort.Comparer);
                }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            IEnumerable<IBusinessObject> oldItems = e.OldItems.Cast<IBusinessObject>();

            foreach (var item in oldItems)
                if (FilteredItems.FirstOrDefault(vm => vm.BusinessObject == item) is CaseloadItemViewModel vm)
                    FilteredItems.Remove(vm);
        }
    }

    partial void OnFilteredItemsChanging(
        ObservableCollection<CaseloadItemViewModel> oldValue,
        ObservableCollection<CaseloadItemViewModel> newValue
    )
    {
        if (oldValue != null)
            oldValue.CollectionChanged -= FilteredItems_CollectionChanged;

        newValue.CollectionChanged += FilteredItems_CollectionChanged;

        ApplyEmptyViewPrompt();
    }

    void FilteredItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is not ObservableCollection<CaseloadItemViewModel>)
            return;

        ApplyEmptyViewPrompt();
    }

    void ApplyEmptyViewPrompt()
    {
        CollectionViewPrompt =
            FilteredItems.Count <= 0 && SearchQuery.Length > 0
                ? LocalizedStrings.NoResultsForSearch.Format(SearchQuery)
                : PromptText;
    }

    /// <summary>
    /// Uses <see cref="SearchQuery"/> to find <see cref="IBusinessObject"/>s by searching their
    /// <see cref="IBusinessObject.DisplayName"/> and <see cref="IBusinessObject.FileNumber"/> fields. Optionally, you
    /// can start the search query with '#' to also search the <see cref="IBusinessObject.Id"/> field.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool SearchQueryMatched(IBusinessObject item) =>
        item.DisplayName.Contains(SearchQuery, StringComparison.InvariantCultureIgnoreCase)
        || item.FileNumberBinding.Contains(SearchQuery, StringComparison.InvariantCultureIgnoreCase)
        || (
            SearchQuery.StartsWith('#')
            && item.IdBinding.Contains(SearchQuery[1..], StringComparison.InvariantCultureIgnoreCase)
        );

    partial void OnSearchQueryChanged(string value)
    {
        ApplyFilter();
    }

    bool MatchesFilters(IBusinessObject item)
    {
        return SearchQueryMatched(item)
            && (SelectedFilter?.WherePredicate(item) ?? true)
            && (SelectedOfficeFilter?.WherePredicate(item) ?? true);
    }

    partial void OnSelectedOfficeFilterChanged(FilterOption<IBusinessObject>? value)
    {
        ApplyFilter();
    }

    partial void OnSelectedSortChanged(SortOption<CaseloadItemViewModel> value)
    {
        FilteredItems.Sort(SelectedSort.Comparer);
    }

    partial void OnSelectedFilterChanged(FilterOption<IBusinessObject> value)
    {
        ApplyFilter();
    }

    void ApplyFilter()
    {
        if (SessionInfo == null)
            return;

        var itemsMatchingFilter = AllItems
            .Where(MatchesFilters)
            .Select(bo => new CaseloadItemViewModel(DraftIndicatorHelper, bo, SessionInfo))
            .Order(SelectedSort.Comparer);

        FilteredItems = new(itemsMatchingFilter);

        // TODO: add/remove individual items instead of clobbering entire collection to improve performance
    }

    public async void Receive(ServiceStateMessage message)
    {
        IsRefreshing = message.Status == VisitzService.State.Running;

        try
        {
            if (message.FinishedSuccess)
                await TryShowEmptyMessage();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    async Task TryShowEmptyMessage()
    {
        using Realm realm = await VisitzRealms.GetIcmDataRealmAsync();
        bool anyExist =
            realm.All<CaseRecord>().Any()
            || realm.All<IncidentRecord>().Any()
            || realm.All<MemoRecord>().Any()
            || realm.All<ServiceRequestRecord>().Any();

        ShowEmptyCaseloadMessage = !anyExist;
    }

    [RelayCommand]
    public void RefreshCaseload()
    {
        WeakReferenceMessenger.Default.Send(GetAllDataForOfflineService.MakeStartMessage(forceDownload: true));
        ShowEmptyCaseloadMessage = false;
    }
}
