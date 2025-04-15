using Visitz.Views.BaseClasses;

namespace Visitz.Views.Caseload;

public partial class CaseloadDetailView : ViewModelContentView
{
    public CaseloadDetailView() : base(ServiceProvider.GetService<CaseloadDetailViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
