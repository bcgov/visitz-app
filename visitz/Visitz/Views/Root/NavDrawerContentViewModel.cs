using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.User;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Root;

public partial class NavDrawerContentViewModel : VisitzViewModel
{
    [ObservableProperty]
    public partial UserViewModel UserViewModel { get; set; } = ServiceProvider.GetService<UserViewModel>();

    [ObservableProperty]
    public partial NavItem LogoutNavItem { get; set; } =
        new()
        {
            Text = LocalizedStrings.Logout,
            Color = Colors.White,
            IconSize = 24,
            SelectedImageSource = MaterialIcons.Logout.GetFilledMaterialIcon(Colors.White),
            UnselectedImageSource = MaterialIcons.Logout.GetUnfilledMaterialIcon(Colors.White),
        };

    [ObservableProperty]
    public partial NavItem CollectionNoticeNavItem { get; set; } =
        new()
        {
            Text = LocalizedStrings.CollectionNotice,
            Color = Colors.White,
            IconSize = 24,
            SelectedImageSource = MaterialIcons.Balance.GetFilledMaterialIcon(Colors.White),
            UnselectedImageSource = MaterialIcons.Balance.GetUnfilledMaterialIcon(Colors.White),
        };

    [ObservableProperty]
    public partial NavItem FeedbackNavItem { get; set; } =
        new()
        {
            Text = LocalizedStrings.FeedbackUrlPrompt,
            Color = Colors.White,
            IconSize = 24,
            SelectedImageSource = MaterialIcons.Feedback.GetFilledMaterialIcon(Colors.White),
            UnselectedImageSource = MaterialIcons.Feedback.GetUnfilledMaterialIcon(Colors.White),
        };

    protected override async Task InitAsync()
    {
        await UserViewModel.StartInitAsync();
    }
}
