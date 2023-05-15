using Visitz.ViewModels;

namespace Visitz.Views;

public partial class LandingPage : VisitzPage
{
    public LandingPage(LandingViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
