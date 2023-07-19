using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Authentication.Keycloak;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    public partial class DebugOptionsViewModel : VisitzViewModel
    {
        [ObservableProperty]
        public string idirOverride;

        public override void PageStarted()
        {
            IdirOverride = DebugOptions.IdirOverride;
        }

        partial void OnIdirOverrideChanged(string value)
        {
            DebugOptions.IdirOverride = value;
        }

        [RelayCommand]
        public void ExpireAccessToken()
        {
            if (DebugOptions.Enabled)
                TokenHolder.DeleteAccessToken();
        }

        [RelayCommand]
        public void ExpireRefreshToken()
        {
            if (DebugOptions.Enabled)
                TokenHolder.DeleteRefreshToken();
        }

        [RelayCommand]
        public async void ClearRealmData()
        {
            await DebugOptions.ClearRealmData();
        }

        [RelayCommand]
        public void DeleteEncryptionKey()
        {
            DebugOptions.DeleteEncryptionKey();
        }

        [RelayCommand]
        public async void Logout()
        {
            if (DebugOptions.Enabled)
                await VisitzSession.LogoutAsync();
        }
    }
}
