using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.SupportNetwork;

public partial class SupportNetworkListView : ViewModelContentView, ICaseloadItemHolder, IRequestedEntitySection
{
    new SupportNetworkListViewModel ViewModel => base.ViewModel as SupportNetworkListViewModel;
    public CaseloadItem CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public EntitySection RequestedSection
    {
        get => ViewModel.RequestedSection;
        set => ViewModel.RequestedSection = value;
    }
	public SupportNetworkListView() : base(ServiceProvider.GetService<SupportNetworkListViewModel>())
	{
		InitializeComponent();
        BindingContext = ViewModel;
	}
}
