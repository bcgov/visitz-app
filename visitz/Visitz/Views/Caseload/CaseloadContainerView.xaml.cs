using CommunityToolkit.Mvvm.Messaging;
using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using Visitz.Views.Entity;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Caseload;

public partial class CaseloadContainerView : BaseContentView
{
    readonly Task<CaseloadListView> _loadListView;

    public CaseloadContainerView()
    {
        InitializeComponent();

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
        MainGrid.Add(listView);

        await Task.WhenAll(
            listView.FadeToAsync(1.0d, easing: Easing.Linear),
            LoadingShimmer.FadeToAsync(0.0d, easing: Easing.Linear)
        );
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

    private void RegisterReceivers()
    {
        StrongReferenceMessenger.Default.Register<BusinessObjectSelectedMessage>(
            this,
            async (recipient, message) =>
            {
                await (recipient as CaseloadContainerView).OpenBusinessObject(message);
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

    private async Task OpenBusinessObject(BusinessObjectSelectedMessage message)
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
}
