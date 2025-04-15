using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;

namespace Visitz.Views.Entity.SupportNetwork;

public partial class SupportNetworkListView : ViewModelContentView, ICaseloadItemHolder
{
    new SupportNetworkListViewModel ViewModel => base.ViewModel as SupportNetworkListViewModel;
    public CaseloadItem CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public SupportNetworkListView() : base(ServiceProvider.GetService<SupportNetworkListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
