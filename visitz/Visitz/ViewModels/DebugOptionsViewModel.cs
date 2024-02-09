using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oidc;
using Visitz.Settings;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    public partial class DebugOptionsViewModel : VisitzViewModel
    {
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

        [ObservableProperty]
        public bool buildingInDebug;

        [ObservableProperty]
        public bool skipLocalAuth;

        public override void PageCreated()
        {
            base.PageCreated();

            DryFireSubmitNotes = DebugOptions.DryFireSubmitNotes;
            DryFireSubmitNotesSimulateSuccess = DebugOptions.DryFireSubmitNotesSimulateSuccess;

            AppId = AppInfo.Current.PackageName;
            DotnetVersion = Environment.Version.ToString();

#if DEBUG
            BuildingInDebug = true;
#else
            BuildingInDebug = false;
#endif
            SkipLocalAuth = BuildingInDebug && DebugOptions.SkipLocalAuth;

            var settings = new AppSettings();

            ApiDomain = settings.Api.ApiDomain;
            AuthenticationDomain = settings.Oidc.AuthenticationDomain;
        }

        partial void OnDryFireSubmitNotesChanged(bool value)
        {
            DebugOptions.DryFireSubmitNotes = value;
        }

        partial void OnDryFireSubmitNotesSimulateSuccessChanged(bool value)
        {
            DebugOptions.DryFireSubmitNotesSimulateSuccess = value;
        }

        partial void OnSkipLocalAuthChanged(bool value)
        {
            DebugOptions.SkipLocalAuth = value;
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
        public async void ClearSafetyAssessmentDraft()
        {
            await DebugOptions.ClearSafetyAssessmentDraftsRealm();
        }

        [RelayCommand]
        public async void Load620bData()
        {
            try
            {
                await DebugOptions.Load620bTestingRecords();
            }
            catch (Exception ex)
            {
                await Navigator.CurrentOpenPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        public async void Logout()
        {
            if (DebugOptions.Enabled)
                await OidcSession.LogoutAsync();
        }
    }
}
