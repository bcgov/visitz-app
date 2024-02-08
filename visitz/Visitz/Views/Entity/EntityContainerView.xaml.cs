using CommunityToolkit.Mvvm.Messaging;
using Visitz.Messaging;
using Visitz.Models;
using VisitzModel.Models;

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
            var (navItem, caseloadItem) = message.Value;

            if (navItem != null)
                (recipient as EntityContainerView).OpenEntitySection(navItem, caseloadItem);
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

    private void OpenEntitySection(NavItem navItem, CaseloadItem caseloadItem)
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
        ContainerDetails.Content = (View)view;
    }
}