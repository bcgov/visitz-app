using CommunityToolkit.Mvvm.Messaging;
using Visitz.Views.Entity;
using Visitz.Views.Entity.Navigation;
using Visitz.Views.SplitView;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;
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

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        StartPaneColumnWidth = SplitLayoutDimensions.StartPaneCaseloadViewLength;
        StartPane.MinimumWidthRequest = SplitLayoutDimensions.MinimumStartPaneWidth;

        RegisterReceivers();

        CaseloadView ??= ServiceProvider.GetService<CaseloadView>();
        CaseloadDetailView ??= ServiceProvider.GetService<CaseloadDetailView>();

        NavigateBack();
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
            (recipient, message) =>
            {
                (recipient as CaseloadContainerView).OpenBusinessObject(message);
            }
        );

        StrongReferenceMessenger.Default.Register<EntityNavBackMessage>(
            this,
            (recipient, message) =>
            {
                (recipient as CaseloadContainerView).NavigateBack();
            }
        );
    }

    private void OpenBusinessObject(BusinessObjectSelectedMessage message)
    {
        IBusinessObject item = message.Value;
        EntitySection section = message.Section;
        IDraftItem draftItem = message.DraftItem;

        var containerView = ServiceProvider.GetService<EntityContainerView>();
        containerView.BusinessObject = item;
        SetEndPane(containerView);

        var entityNav = ServiceProvider.GetService<EntityNavView>();
        entityNav.BusinessObject = item;
        entityNav.SetRequestedSection(section, draftItem);
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
