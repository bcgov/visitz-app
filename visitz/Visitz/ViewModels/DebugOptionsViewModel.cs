using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Services;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    public partial class DebugOptionsViewModel : VisitzViewModel
    {
        [ObservableProperty]
        public string idirOverride;

        [ObservableProperty]
        public bool alwaysExpireAccessToken;

        [ObservableProperty]
        public bool alwaysExpireRefreshToken;

        public override void PageStarted()
        {
            IdirOverride = DebugOptions.IdirOverride;
            AlwaysExpireAccessToken = DebugOptions.AlwaysExpireAccessToken;
            AlwaysExpireRefreshToken = DebugOptions.AlwaysExpireRefreshToken;
        }

        partial void OnIdirOverrideChanged(string value)
        {
            DebugOptions.IdirOverride = value;
        }

        partial void OnAlwaysExpireAccessTokenChanged(bool value)
        {
            DebugOptions.AlwaysExpireAccessToken = value;
        }

        partial void OnAlwaysExpireRefreshTokenChanged(bool value)
        {
            DebugOptions.AlwaysExpireRefreshToken = value;
        }

        [RelayCommand]
        public async void ClearRealmData()
        {
            await DebugOptions.ClearRealmData();
        }
    }
}
