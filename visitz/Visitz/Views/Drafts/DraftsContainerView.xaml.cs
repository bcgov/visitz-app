using Visitz.Views.BaseClasses;

namespace Visitz.Views.Drafts;

public partial class DraftsContainerView : ViewModelContentView<DraftsContainerViewModel>
{
    bool _disposed;

    readonly DraftsList _draftsList;

    public DraftsContainerView()
        : base(ServiceProvider.GetService<DraftsContainerViewModel>())
    {
        InitializeComponent();

        BindingContext = ViewModel;

        _draftsList = ServiceProvider.GetService<DraftsList>();

        MainContent.Content = _draftsList;

        ViewModel.DraftsListViewModel = _draftsList.ViewModel;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _draftsList.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
