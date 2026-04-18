using Visitz.Views.BaseClasses;

namespace Visitz.Views.Drafts;

#nullable enable

public partial class DraftsContainerView : ViewModelContentView
{
    public DraftsContainerView()
        : base(ServiceProvider.GetService<DraftsContainerViewModel>())
    {
        InitializeComponent();

        BindingContext = ViewModel;

        MainContent.Content = ServiceProvider.GetService<DraftsList>();
    }
}
