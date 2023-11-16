using CommunityToolkit.Mvvm.Messaging;
using Visitz.Messaging;
using Visitz.Models;
using Visitz.Views.Entity;
using Visitz.Views.SplitView;

namespace Visitz.Views.Caseload;

public partial class CaseloadContainerView : SplitLayoutView
{
    private static readonly double MinimumStartPaneWidth = 250.0f;
    private static readonly GridLength StartPaneCaseloadViewLength = new(0.5, GridUnitType.Star);

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

        StrongReferenceMessenger.Default.Register<EntityNavMessage>(this, (recipient, message) =>
        {
            var (navItem, caseloadItem) = message.Value;
            (recipient as CaseloadContainerView).OpenEntitySection(navItem, caseloadItem);
        });
    }

    private void OpenCaseloadItem(CaseloadItem item)
    {
        var entityNav = ServiceProvider.GetService<EntityNavView>();
        entityNav.CaseloadItem = item;
        SetStartPane(entityNav);

        StartPaneColumnWidth = GridLength.Auto;
    }

    private void NavigateBack()
    {
        SetStartPane(ServiceProvider.GetService<CaseloadView>());
        SetEndPane(null);

        StartPaneColumnWidth = StartPaneCaseloadViewLength;
    }

    private void OpenEntitySection(NavItem navItem, CaseloadItem caseloadItem)
    {
        if (navItem == null)
            return;

        var view = (IView)ServiceProvider.GetService(navItem.ContentViewType);
        (view as ICaseloadItemHolder).CaseloadItem = caseloadItem;
        SetEndPane(view);
    }
}