using Visitz.ViewModels;

namespace Visitz.Views;

public partial class DeviceAuthenticationPage : VisitzPage
{
    public DeviceAuthenticationPage(DeviceAuthenticationViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
