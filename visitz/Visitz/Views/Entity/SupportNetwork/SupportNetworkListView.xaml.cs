using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity.SupportNetwork;

#nullable enable

public partial class SupportNetworkListView : ViewModelContentView, IBusinessObjectHolder
{
    new SupportNetworkListViewModel? ViewModel => base.ViewModel as SupportNetworkListViewModel;

    public IBusinessObject? BusinessObject
    {
        get => ViewModel?.BusinessObject;
        set
        {
            if (ViewModel != null)
                ViewModel.BusinessObject = value;
        }
    }

    public SupportNetworkListView() : base(ServiceProvider.GetService<SupportNetworkListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
