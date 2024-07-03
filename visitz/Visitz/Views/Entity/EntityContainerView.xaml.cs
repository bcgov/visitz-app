using CommunityToolkit.Mvvm.Messaging;
using Visitz.Views.BaseClasses;
using Visitz.Views.Entity.Details;
using VisitzModel.Messaging;
using VisitzModel.Models;
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

    protected override void Creating()
    {
        base.Creating();

        StrongReferenceMessenger.Default.Register<EntityNavMessage>(this, (recipient, message) =>
        {
            var (navItem, caseloadItem, subsection) = message.Value;

            if (navItem != null)
                (recipient as EntityContainerView).OpenEntitySection(navItem, caseloadItem, subsection);
        });
    }

    protected override void Destroying()
    {
        StrongReferenceMessenger.Default.UnregisterAll(this);

        if (ContainerDetails.Content is BaseContentView baseView)
        {
            baseView.Destroy();
            ContainerDetails.Content = null;
        }

        base.Destroying();
    }

    private void OpenEntitySection(EntityNavItem navItem, CaseloadItem caseloadItem, EntitySection? subsection)
    {
        if (ContainerDetails.Content is BaseContentView baseView)
        {
            if (baseView.GetType().Equals(navItem.ContentViewType.GetType()))
                return;

            baseView.Destroy();
            ContainerDetails.Content = null;
        }

        var view = (IView)ServiceProvider.GetService(navItem.ContentViewType);

		(view as ICaseloadItemHolder).CaseloadItem = caseloadItem;

		if (view is IRequestedEntitySection sectionView)
			sectionView.RequestedSection = subsection ?? navItem.Section;

        ContainerDetails.Content = (View)view;
    }
}
