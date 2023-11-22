using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Authentication.Keycloak;
using Visitz.Settings;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    public partial class DebugOptionsViewModel : VisitzViewModel
    {
        [ObservableProperty]
        public string idirOverride;

        [ObservableProperty]
        public bool dryFireSubmitNotes;

        [ObservableProperty]
        public bool dryFireSubmitNotesSimulateSuccess;

        [ObservableProperty]
        public string appId;

        [ObservableProperty]
        public string dotnetVersion;

        [ObservableProperty]
        public string apiDomain;

        [ObservableProperty]
        public string authenticationDomain;

        public override void PageCreated()
        {
            base.PageCreated();

            IdirOverride = DebugOptions.IdirOverride;
            DryFireSubmitNotes = DebugOptions.DryFireSubmitNotes;
            DryFireSubmitNotesSimulateSuccess = DebugOptions.DryFireSubmitNotesSimulateSuccess;

            AppId = AppInfo.Current.PackageName;
            DotnetVersion = Environment.Version.ToString();

            var settings = new AppSettings();

            ApiDomain = settings.Api.ApiDomain;
            AuthenticationDomain = settings.Oidc.AuthenticationDomain;
        }

        partial void OnIdirOverrideChanged(string value)
        {
            DebugOptions.IdirOverride = value;
        }

        partial void OnDryFireSubmitNotesChanged(bool value)
        {
            DebugOptions.DryFireSubmitNotes = value;
        }

        partial void OnDryFireSubmitNotesSimulateSuccessChanged(bool value)
        {
            DebugOptions.DryFireSubmitNotesSimulateSuccess = value;
        }

        [RelayCommand]
        public void DeleteAccessToken()
        {
            if (DebugOptions.Enabled)
                TokenHolder.DeleteAccessToken();
        }

        [RelayCommand]
        public void DeleteRefreshToken()
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
