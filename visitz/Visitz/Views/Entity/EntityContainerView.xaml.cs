using CommunityToolkit.Mvvm.Messaging;
using Visitz.Messaging;
using Visitz.Models;

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

        base.Destroying();
    }

    private void OpenEntitySection(NavItem navItem, CaseloadItem caseloadItem)
    {
        var view = (IView)ServiceProvider.GetService(navItem.ContentViewType);
        (view as ICaseloadItemHolder).CaseloadItem = caseloadItem;
        ContainerDetails.Content = (View)view;
    }
}