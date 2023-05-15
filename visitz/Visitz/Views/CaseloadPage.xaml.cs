using Visitz.ViewModels;

namespace Visitz.Views;

public partial class CaseloadPage : VisitzPage
{
    public CaseloadPage(CaseloadViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
