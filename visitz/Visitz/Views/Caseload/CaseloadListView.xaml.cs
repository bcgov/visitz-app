using Visitz.Views.BaseClasses;

namespace Visitz.Views.Caseload;

public partial class CaseloadListView : ViewModelContentView<CaseloadListViewModel>
{
    public CaseloadListView(CaseloadListViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
