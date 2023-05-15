using Visitz.ViewModels;

namespace Visitz.Views;

public partial class OpenIdAuthenticationPage : VisitzPage
{
    public OpenIdAuthenticationPage(OpenIdAuthenticationViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
