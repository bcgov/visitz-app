using Visitz.Views.BaseClasses;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadListView : ViewModelContentView<CaseloadListViewModel>
{
    public CaseloadListView(CaseloadListViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
