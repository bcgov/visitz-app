using CommunityToolkit.Mvvm.Messaging;
using Visitz.Views.Entity;
using Visitz.Views.SplitView;
using VisitzModel.Messaging;
using VisitzModel.Models;

namespace Visitz.Views.Caseload;

public partial class CaseloadContainerView : SplitLayoutView
{
    private static readonly double MinimumStartPaneWidth = 300.0f;
    private static readonly GridLength StartPaneCaseloadViewLength = new(0.6, GridUnitType.Star);

    public CaseloadContainerView()
    {
		InitializeComponent();
    }

    protected override void Creating()
    {
        base.Creating();

        StartPaneColumnWidth = StartPaneCaseloadViewLength;
        StartPane.MinimumWidthRequest = MinimumStartPaneWidth;

        RegisterReceivers();

        SetStartPane(ServiceProvider.GetService<CaseloadView>());
        SetEndPane(ServiceProvider.GetService<CaseloadDetailView>());
    }

    protected override void Destroying()
    {
        StrongReferenceMessenger.Default.UnregisterAll(this);

        base.Destroying();
    }

    private void RegisterReceivers()
    {
        StrongReferenceMessenger.Default.Register<CaseloadItemSelectedMessage>(this, (recipient, message) =>
        {
            (recipient as CaseloadContainerView).OpenCaseloadItem(message.Value);
        });

        StrongReferenceMessenger.Default.Register<EntityNavBackMessage>(this, (recipient, message) =>
        {
            (recipient as CaseloadContainerView).NavigateBack();
        });
    }

    private void OpenCaseloadItem(CaseloadItem item)
    {
        var containerView = ServiceProvider.GetService<EntityContainerView>();
        containerView.CaseloadItem = item;
        SetEndPane(containerView);

        var entityNav = ServiceProvider.GetService<EntityNavView>();
        entityNav.CaseloadItem = item;
        SetStartPane(entityNav);

        StartPaneColumnWidth = GridLength.Auto;
    }

    private void NavigateBack()
    {
        SetStartPane(ServiceProvider.GetService<CaseloadView>());
        SetEndPane(ServiceProvider.GetService<CaseloadDetailView>());

        StartPaneColumnWidth = StartPaneCaseloadViewLength;
    }
}
