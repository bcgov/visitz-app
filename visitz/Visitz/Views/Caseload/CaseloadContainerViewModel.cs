using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Foldable;
using Oidc;
using Visitz.Controls;
using Visitz.Resources.Localization;
using Visitz.Services.Caseload;
using Visitz.Views.BaseClasses;
using VisitzModel.Events;
using VisitzModel.Extensions;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Views.Caseload;

public partial class CaseloadContainerViewModel : VisitzViewModel
{
    private static readonly string SortOptionIndexPref = "SortOptionIndexPref";

    static readonly SortOption<CaseloadItemViewModel> s_keyPlayerSort = new(
        LocalizedStrings.KeyPlayer,
        Comparer<CaseloadItemViewModel>.Create(
            (a, b) =>
            {
                return a.BusinessObject.DisplayName.CompareTo(b.BusinessObject.DisplayName);
            }
        )
    );

    static readonly SortOption<CaseloadItemViewModel> s_openDateSort = new(
        LocalizedStrings.OpenDate,
        Comparer<CaseloadItemViewModel>.Create(
            (a, b) => a.BusinessObject.CreatedDateBinding.CompareTo(b.BusinessObject.CreatedDateBinding)
        )
    );

    static readonly FilterOption<IBusinessObject> s_allTypesFilter = new(LocalizedStrings.AllTypes, _ => true);

    static readonly FilterOption<IBusinessObject> s_caseFilter = new(
        LocalizedStrings.Cases,
        businessObject => businessObject.EntityType == EntityType.Case
    );

    static readonly FilterOption<IBusinessObject> s_incidentFilter = new(
        LocalizedStrings.Incidents,
        businessObject => businessObject.EntityType == EntityType.Incident
    );

    static readonly FilterOption<IBusinessObject> s_allOfficeFilter = new(LocalizedStrings.All, _ => true);

    bool _disposed;

    OidcSessionInfo? SessionInfo { get; set; }

    LastUpdatedPrefs LastUpdatedPrefs { get; set; } = ServiceProvider.GetService<LastUpdatedPrefs>();

    FilterOption<IBusinessObject> _myCaseloadFilter = s_allOfficeFilter; // Default All filter to satisfy nullability

    public CaseloadListViewModel? ListViewModel { get; set; }

    public List<FilterOption<IBusinessObject>> _startingOfficeFilters = [];

    [ObservableProperty]
    public partial string SearchQuery { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool ShowSearchBar { get; set; } = false;

    [ObservableProperty]
    public partial bool ShowTitle { get; set; } = true;

    [ObservableProperty]
    public partial DateTime? LastUpdated { get; set; }

    [ObservableProperty]
    public partial List<SortOption<CaseloadItemViewModel>> SortOptions { get; set; } =
    [s_keyPlayerSort, s_openDateSort];

    [ObservableProperty]
    public partial SortOption<CaseloadItemViewModel> SelectedSort { get; set; } = s_keyPlayerSort;

    [ObservableProperty]
    public partial List<FilterOption<IBusinessObject>> FilterOptions { get; set; } =
    [s_allTypesFilter, s_caseFilter, s_incidentFilter];

    [ObservableProperty]
    public partial FilterOption<IBusinessObject> SelectedFilter { get; set; } = s_allTypesFilter;

    [ObservableProperty]
    public partial ObservableCollection<FilterOption<IBusinessObject>> OfficeOptions { get; set; } = [];

    [ObservableProperty]
    public partial FilterOption<IBusinessObject>? SelectedOffice { get; set; }

    public CaseloadContainerViewModel()
    {
        StrongReferenceMessenger.Default.RegisterAll(this);

        LastUpdated = LastUpdatedPrefs.Get(GetCaseloadService.MakeId());
        LastUpdatedPrefs.LastUpdatedChanged += LastUpdatedPrefs_LastUpdatedChanged;

        int savedSortIndex = Preferences.Default.Get(SortOptionIndexPref, 0);
        SelectedSort = SortOptions.ElementAt(ClampSortIndex(savedSortIndex));
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        SessionInfo = await OidcSessionInfo.GetAsync();

        _myCaseloadFilter = new(
            LocalizedStrings.MyCaseload,
            businessObject => businessObject.IsAssigned(SessionInfo?.Idir ?? string.Empty)
        );
        _startingOfficeFilters.Add(s_allOfficeFilter);
        _startingOfficeFilters.Add(_myCaseloadFilter);

        SetupOfficeNames();
        SessionInfo.OfficesChanged += SessionInfo_OfficesChanged;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
            LastUpdatedPrefs.LastUpdatedChanged -= LastUpdatedPrefs_LastUpdatedChanged;
            SessionInfo?.OfficesChanged -= SessionInfo_OfficesChanged;

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    void SetupOfficeNames()
    {
        OfficeOptions.Clear();
        foreach (var starter in _startingOfficeFilters)
            OfficeOptions.Add(starter);

        if (SessionInfo != null)
            foreach (var office in SessionInfo.OfficeNames.AsEnumerable().Order())
                OfficeOptions.Add(new(office, businessObject => OfficeEqual(businessObject, office)));

        SelectedOffice = _myCaseloadFilter;
    }

    void UpdateSortedOfficeNames(HashSet<string> incomingOfficeNames)
    {
        FilterOption<IBusinessObject> currentSelected = SelectedOffice ?? _myCaseloadFilter;

        // Skip to account for always-available options
        int offset = _startingOfficeFilters.Count;

        IEnumerable<FilterOption<IBusinessObject>> current = OfficeOptions.Skip(offset);

        foreach (var removeOffice in current.ExceptBy(incomingOfficeNames, option => option.Text))
            OfficeOptions.Remove(removeOffice);

        foreach (var addOffice in incomingOfficeNames.Except(current.Select(option => option.Text)))
        {
            FilterOption<IBusinessObject> newFilter = new(
                addOffice,
                businessObject => OfficeEqual(businessObject, addOffice)
            );
            OfficeOptions.InsertSorted(newFilter, startIndex: offset);
        }

        if (currentSelected != SelectedOffice)
            SelectedOffice = OfficeOptions.Contains(currentSelected) ? currentSelected : _myCaseloadFilter;
    }

    static bool OfficeEqual(IBusinessObject businessObject, string officeToCheck) =>
        businessObject.ServiceOffice.Equals(officeToCheck, StringComparison.InvariantCultureIgnoreCase);

    void SessionInfo_OfficesChanged(object? sender, HashSet<string> newOffices)
    {
        MainThread.BeginInvokeOnMainThread(() => UpdateSortedOfficeNames(newOffices));
    }

    partial void OnSearchQueryChanged(string value)
    {
        SearchByQuery();
    }

    public void SearchByQuery()
    {
        ListViewModel?.SearchQuery = SearchQuery.Trim();
    }

    partial void OnShowSearchBarChanged(bool value)
    {
        ApplyTitleVisibility();
    }

    void ApplyTitleVisibility()
    {
        bool hideTitle = ShowSearchBar;
        ShowTitle = !hideTitle;
    }

    private void LastUpdatedPrefs_LastUpdatedChanged(object? sender, LastUpdatedChangedEventArgs e)
    {
        if (e.Id.Equals(GetCaseloadService.MakeId()))
            LastUpdated = (sender as LastUpdatedPrefs)?.Get(e.Id);
    }

    int ClampSortIndex(int requestedIndex)
    {
        return Math.Clamp(requestedIndex, 0, SortOptions.Count - 1);
    }

    partial void OnSelectedOfficeChanged(FilterOption<IBusinessObject>? value)
    {
        ListViewModel?.SelectedOfficeFilter = value;
    }

    partial void OnSelectedSortChanged(SortOption<CaseloadItemViewModel> value)
    {
        if (value == null)
            return;

        ListViewModel?.SelectedSort = value;
        Preferences.Default.Set(SortOptionIndexPref, ClampSortIndex(SortOptions.IndexOf(value)));
    }

    partial void OnSelectedFilterChanged(FilterOption<IBusinessObject> value)
    {
        ListViewModel?.SelectedFilter = value ?? s_allTypesFilter;
    }
}
