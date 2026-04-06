using Visitz.Views.BaseClasses;

namespace Visitz.Views.Drafts;

#nullable enable

public partial class DraftsMasterList : ViewModelContentView
{
    public DraftsMasterList()
        : base(ServiceProvider.GetService<DraftsMasterListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
