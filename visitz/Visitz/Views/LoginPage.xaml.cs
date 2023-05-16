using Visitz.ViewModels;

namespace Visitz.Views;

public partial class LoginPage : VisitzPage
{
    public LoginPage(LoginViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
