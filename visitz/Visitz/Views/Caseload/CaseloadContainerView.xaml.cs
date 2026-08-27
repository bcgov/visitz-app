using CommunityToolkit.Maui.Core.Platform;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Caseload;

public partial class CaseloadContainerView : ViewModelContentView<CaseloadContainerViewModel>
{
    readonly Task<CaseloadListView> _loadListView;

    public CaseloadContainerView(CaseloadContainerViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
        BindingContext = ViewModel;

        _loadListView = InitListView();

        for (int i = 0; i < 25; i++)
            CustomShimmerContainer.Add(new CaseloadItemShimmerStencil());
    }

    static async Task<CaseloadListView> InitListView()
    {
        CaseloadListView listView = ServiceProvider.GetService<CaseloadListView>();

        await listView.StartInitAsync();
        listView.Opacity = 0.0d;

        return listView;
    }

    protected override async Task OnFirstLoadAsync()
    {
        await base.OnFirstLoadAsync();

        CaseloadListView listView = await _loadListView;
        ViewModel.ListViewModel = listView.ViewModel;
        listView.ViewModel.SelectedOfficeFilter = ViewModel.SelectedOffice;
        listView.ViewModel.SelectedFilter = ViewModel.SelectedFilter;
        listView.ViewModel.SelectedSort = ViewModel.SelectedSort;

        MainGrid.Add(listView, 0, 1);

        await Task.WhenAll(
            listView.FadeToAsync(1.0d, easing: Easing.Linear),
            LoadingShimmer.FadeToAsync(0.0d, easing: Easing.Linear)
        );
        LoadingShimmer.IsVisible = false;
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            disposed = true;
        }

        base.Dispose(disposing);
    }

    void SearchActionButton_Clicked(object? sender, EventArgs e)
    {
        ViewModel.ShowSearchBar = true;
        CaseloadSearchBar.Focus();
    }

#if MACCATALYST
    void CaseloadSearchBar_SearchButtonPressed(object? sender, EventArgs e)
#else
    async void CaseloadSearchBar_SearchButtonPressed(object? sender, EventArgs e)
#endif
    {
        ViewModel.SearchByQuery();

#if !MACCATALYST
        await CaseloadSearchBar.HideKeyboardAsync(CancellationToken.None);
#endif
    }

    void CaseloadSearchBar_TextChanged(object? sender, TextChangedEventArgs e)
    {
        ViewModel.SearchByQuery();
    }

    void CaseloadSearchBar_Unfocused(object? sender, FocusEventArgs e)
    {
        if (ViewModel.SearchQuery.Length <= 0)
            ViewModel.ShowSearchBar = false;
    }
}
