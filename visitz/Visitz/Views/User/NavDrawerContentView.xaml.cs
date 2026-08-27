using Visitz.Views.BaseClasses;

namespace Visitz.Views.User;

public partial class NavDrawerContentView : ViewModelContentView<NavDrawerContentViewModel>
{
    public NavDrawerContentView()
        : base(ServiceProvider.GetService<NavDrawerContentViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        CollectionNoticeItem.TappedCommand = ViewModel.UserViewModel.OpenCollectionNoticeCommand;
        FeedbackItem.TappedCommand = ViewModel.UserViewModel.OpenFeedbackUrlCommand;
        LogoutItem.TappedCommand = ViewModel.UserViewModel.TryLogoutCommand;
    }
}
