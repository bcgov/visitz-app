using CommunityToolkit.Mvvm.Messaging;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity;

public partial class EntityContainerView : ViewModelContentView, IBusinessObjectHolder
{
    public IBusinessObject BusinessObject
    {
        get => (ViewModel as EntityContainerViewModel).BusinessObject;
        set => (ViewModel as EntityContainerViewModel).BusinessObject = value;
    }

    public EntityContainerView() : base(ServiceProvider.GetService<EntityContainerViewModel>())
    {
        InitializeComponent();

        BindingContext = ViewModel;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        StrongReferenceMessenger.Default.Register<EntityNavMessage>(this, Receive);
    }

    void Receive(object recipient, EntityNavMessage message)
    {
        var (navItem, businessObject, subsection, draftItem) = message.Value;

        if (navItem != null)
        {
            (recipient as EntityContainerView).OpenEntitySection(
                navItem,
                businessObject,
                subsection,
                draftItem);
        }
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
        IBusinessObject businessObject,
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

        if (view is IBusinessObjectHolder objectHolder)
            objectHolder.BusinessObject = businessObject;

        if (view is IRequestedEntitySection sectionView)
            sectionView.RequestedSection = subsection ?? navItem.Section;

        if (view is IFocusDraftItem focusDraftView)
            focusDraftView.FocusedDraftItem = focusedDraftItem;

        ContainerDetails.Content = (View)view;
    }
}
