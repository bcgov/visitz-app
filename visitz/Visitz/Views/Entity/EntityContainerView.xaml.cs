using CommunityToolkit.Mvvm.Messaging;
using Visitz.Views.BaseClasses;
using Visitz.Views.Entity.Details;
using VisitzModel.Interfaces;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity;

public partial class EntityContainerView : ViewModelContentView, ICaseloadItemHolder
{
    public CaseloadItem CaseloadItem
    {
        get => (ViewModel as EntityContainerViewModel).CaseloadItem;
        set => (ViewModel as EntityContainerViewModel).CaseloadItem = value;
    }

    public EntityContainerView() : base(ServiceProvider.GetService<EntityContainerViewModel>())
    {
        InitializeComponent();

        BindingContext = ViewModel;

        ContainerDetails.Content = ServiceProvider.GetService<EntityDetailsView>();
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        StrongReferenceMessenger.Default.Register<EntityNavMessage>(this, (recipient, message) =>
        {
            var (navItem, caseloadItem, subsection, draftItem) = message.Value;

            if (navItem != null)
                (recipient as EntityContainerView).OpenEntitySection(navItem, caseloadItem, subsection, draftItem);
        });
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);

            if (ContainerDetails.Content is BaseContentView baseView)
            {
                baseView.Dispose();
                ContainerDetails.Content = null;
            }

            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void OpenEntitySection(
        EntityNavItem navItem,
        CaseloadItem caseloadItem,
        EntitySection? subsection,
        IDraftItem focusedDraftItem)
    {
        if (ContainerDetails.Content is BaseContentView baseView)
        {
            if (baseView.GetType().Equals(navItem.ContentViewType.GetType()))
                return;

            baseView.Dispose();
            ContainerDetails.Content = null;
        }

        var view = (IView)ServiceProvider.GetService(navItem.ContentViewType);

        if (view is ICaseloadItemHolder itemHolder)
            itemHolder.CaseloadItem = caseloadItem;

        if (view is IRequestedEntitySection sectionView)
            sectionView.RequestedSection = subsection ?? navItem.Section;

        if (view is IFocusDraftItem focusDraftView)
            focusDraftView.FocusedDraftItem = focusedDraftItem;

        ContainerDetails.Content = (View)view;
    }
}
