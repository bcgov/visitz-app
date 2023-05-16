using Visitz.ViewModels;

namespace Visitz.Views;

public partial class AppLockPage : VisitzPage
{
    public AppLockPage(AppLockViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
