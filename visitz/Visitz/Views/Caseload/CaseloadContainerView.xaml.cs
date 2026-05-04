using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using Visitz.Views.Entity;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Caseload;

#nullable enable

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

        RegisterReceivers();
    }

    static async Task<CaseloadListView> InitListView()
    {
        CaseloadListView listView = ServiceProvider.GetService<CaseloadListView>();

        await listView.StartInitAsync();
        listView.Opacity = 0.0d;

        return listView;
    }

    protected override async Task OnLoadedAsync()
    {
        await base.OnLoadedAsync();

        CaseloadListView listView = await _loadListView;
        ViewModel.ListViewModel = listView.ViewModel;
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
            StrongReferenceMessenger.Default.UnregisterAll(this);
            disposed = true;
        }

        base.Dispose(disposing);
    }

    void RegisterReceivers()
    {
        StrongReferenceMessenger.Default.Register<BusinessObjectSelectedMessage>(
            this,
            async (recipient, message) =>
            {
                await ((CaseloadContainerView)recipient).OpenBusinessObject(message);
            }
        );

        StrongReferenceMessenger.Default.Register<EntityNavBackMessage>(
            this,
            async (recipient, message) =>
            {
                await Navigator.Navigation.PopAsync();
            }
        );
    }

    async Task OpenBusinessObject(BusinessObjectSelectedMessage message)
    {
        IBusinessObject item = message.Value;

        try
        {
            var entityPage = ServiceProvider.GetService<EntityPage>();

            entityPage.Init(
                item.Id,
                item.EntityType,
                item.DisplayName,
                item.FileNumber,
                message.Section,
                message.DraftItem
            );

            await Navigator.Navigation.PushAsync(entityPage);
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    void SearchActionButton_Clicked(object sender, EventArgs e)
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
