using CommunityToolkit.Mvvm.Messaging;
using Visitz.Views.Entity;
using Visitz.Views.Entity.Navigation;
using Visitz.Views.SplitView;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Caseload;

public partial class CaseloadContainerView : SplitLayoutView
{
    static IView CaseloadView;
    static IView CaseloadDetailView;

    public CaseloadContainerView()
    {
        InitializeComponent();
    }

    protected override void Creating()
    {
        base.Creating();

        StartPaneColumnWidth = SplitLayoutDimensions.StartPaneCaseloadViewLength;
        StartPane.MinimumWidthRequest = SplitLayoutDimensions.MinimumStartPaneWidth;

        RegisterReceivers();

        CaseloadView ??= ServiceProvider.GetService<CaseloadView>();
        CaseloadDetailView ??= ServiceProvider.GetService<CaseloadDetailView>();

        NavigateBack();
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
            (recipient as CaseloadContainerView).OpenCaseloadItem(message);
        });

        StrongReferenceMessenger.Default.Register<EntityNavBackMessage>(this, (recipient, message) =>
        {
            (recipient as CaseloadContainerView).NavigateBack();
        });
    }

    private void OpenCaseloadItem(CaseloadItemSelectedMessage message)
    {
        CaseloadItem item = message.Value;
        EntitySection section = message.Section;
        IDraftItem draftItem = message.DraftItem;

        var containerView = ServiceProvider.GetService<EntityContainerView>();
        containerView.CaseloadItem = item;
        SetEndPane(containerView);

        var entityNav = ServiceProvider.GetService<EntityNavView>();
        entityNav.CaseloadItem = item;
        entityNav.SetRequestedSection(section, draftItem);
        SetStartPane(entityNav);

        StartPaneColumnWidth = GridLength.Auto;
    }

    private void OpenTodoCaseloadItem(CaseloadItemSelectedMessage message)
    {
        CaseloadItem item = message.Value;
        EntitySection section = message.Section;
        IDraftItem draftItem = message.DraftItem;

        var containerView = ServiceProvider.GetService<EntityContainerView>();
        containerView.CaseloadItem = item;
        SetEndPane(containerView);

        var entityNav = ServiceProvider.GetService<EntityNavView>();
        entityNav.CaseloadItem = item;
        entityNav.SetRequestedSection(section);
        SetStartPane(entityNav);

        StartPaneColumnWidth = GridLength.Auto;
    }

    private void NavigateBack()
    {
        SetStartPane(CaseloadView);
        SetEndPane(CaseloadDetailView);

        StartPaneColumnWidth = SplitLayoutDimensions.StartPaneCaseloadViewLength;
    }
}
