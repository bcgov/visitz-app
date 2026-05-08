using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Foldable;
using Visitz.Controls;
using Visitz.Resources.Localization;
using Visitz.Services.Caseload;
using Visitz.Views.BaseClasses;
using VisitzModel.Events;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;
using VisitzModel.Storage;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadContainerViewModel : VisitzViewModel, IRecipient<NavPositionMessage>
{
    private static readonly string SortOptionIndexPref = "SortOptionIndexPref";

    bool _disposed;

    public CaseloadListViewModel? ListViewModel { get; set; }

    [ObservableProperty]
    public string searchQuery = string.Empty;

    [ObservableProperty]
    public bool showSearchBar = false;

    [ObservableProperty]
    public bool showTitle = true;

    [ObservableProperty]
    public LayoutOptions searchBarHorizontalOptions;

    [ObservableProperty]
    public ObservableCollection<string> officeNames = [];

    [ObservableProperty]
    public string? selectedOffice;

    LastUpdatedPrefs LastUpdatedPrefs { get; set; } = ServiceProvider.GetService<LastUpdatedPrefs>();

    [ObservableProperty]
    public DateTime? lastUpdated;

    static readonly SortOption<CaseloadItemViewModel> _keyPlayerSort = new(
        LocalizedStrings.KeyPlayer,
        Comparer<CaseloadItemViewModel>.Create(
            (a, b) =>
            {
                return a.BusinessObject.DisplayName.CompareTo(b.BusinessObject.DisplayName);
            }
        )
    );

    static readonly SortOption<CaseloadItemViewModel> _openDateSort = new(
        LocalizedStrings.OpenDate,
        Comparer<CaseloadItemViewModel>.Create(
            (a, b) => a.BusinessObject.CreatedDateBinding.CompareTo(b.BusinessObject.CreatedDateBinding)
        )
    );

    [ObservableProperty]
    public List<SortOption<CaseloadItemViewModel>> sortOptions = [_keyPlayerSort, _openDateSort];

    [ObservableProperty]
    public SortOption<CaseloadItemViewModel> selectedSort = _keyPlayerSort;

    public CaseloadContainerViewModel()
    {
        SetSearchBarHorizontalOptions(
            (TwoPaneViewMode)StrongReferenceMessenger.Default.Send(new GetNavPositionMessage()).Response
        );
        StrongReferenceMessenger.Default.RegisterAll(this);

        LastUpdated = LastUpdatedPrefs.Get(GetCaseloadService.MakeId());
        LastUpdatedPrefs.LastUpdatedChanged += LastUpdatedPrefs_LastUpdatedChanged;

        int savedSortIndex = Preferences.Default.Get(SortOptionIndexPref, 0);
        SelectedSort = SortOptions.ElementAt(ClampSortIndex(savedSortIndex));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
            LastUpdatedPrefs.LastUpdatedChanged -= LastUpdatedPrefs_LastUpdatedChanged;

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    public void Receive(NavPositionMessage message)
    {
        SetSearchBarHorizontalOptions((TwoPaneViewMode)message.Value);
    }

    void SetSearchBarHorizontalOptions(TwoPaneViewMode mode)
    {
        SearchBarHorizontalOptions = mode switch
        {
            TwoPaneViewMode.Tall => LayoutOptions.Fill,
            TwoPaneViewMode.Wide => LayoutOptions.End,
            _ => throw new InvalidOperationException($"{nameof(TwoPaneViewMode)} '{mode}' not supported"),
        };
    }

    partial void OnSearchQueryChanged(string value)
    {
        SearchByQuery();
    }

    public void SearchByQuery()
    {
        ListViewModel?.SearchQuery = SearchQuery.Trim();
    }

    partial void OnSearchBarHorizontalOptionsChanged(LayoutOptions value)
    {
        ApplyTitleVisibility();
    }

    partial void OnShowSearchBarChanged(bool value)
    {
        ApplyTitleVisibility();
    }

    void ApplyTitleVisibility()
    {
        bool hideTitle = SearchBarHorizontalOptions == LayoutOptions.Fill && ShowSearchBar;
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

    async partial void OnSelectedSortChanged(SortOption<CaseloadItemViewModel> value)
    {
        if (value == null)
            return;

        ListViewModel?.SelectedSort = value;
        Preferences.Default.Set(SortOptionIndexPref, ClampSortIndex(SortOptions.IndexOf(value)));
    }
}
