using Visitz.Views.BaseClasses;

namespace Visitz.Views.User;

public partial class UserView : ViewModelContentView
{
    public UserView() : base(ServiceProvider.GetService<UserViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
