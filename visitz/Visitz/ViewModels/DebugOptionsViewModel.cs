using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oidc;
using Visitz.Services;
using Visitz.Settings;
using Visitz.Storage;
using VisitzModel.Extensions;
using VisitzModel.Storage;

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

		readonly LastUpdatedPrefs lastUpdatedPrefs = ServiceProvider.GetService<LastUpdatedPrefs>();

		[ObservableProperty]
		public DateTime caseloadLastUpdated;

		[ObservableProperty]
		public DateTime maxDate = DateTimeExtensions.LocalNow;

		public override void Create()
        {
            base.Create();

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

			CaseloadLastUpdated = lastUpdatedPrefs.Get(GetCaseloadService.MakeId(), DateTimeExtensions.LocalNow);
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
        public static void DeleteAccessToken()
        {
            if (DebugOptions.Enabled)
                TokenHolder.DeleteAccessToken();
        }

        [RelayCommand]
        public static void DeleteRefreshToken()
        {
            if (DebugOptions.Enabled)
                TokenHolder.DeleteRefreshToken();
        }

        [RelayCommand]
        public static async Task ClearRealmData()
        {
            await DebugOptions.ClearRealmData();
        }

        [RelayCommand]
        public static async Task ClearSafetyAssessmentDraft()
        {
            await DebugOptions.ClearSafetyAssessmentDraftsRealm();
        }

        [RelayCommand]
        public static async Task Load620bData()
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
        public static async Task Logout()
        {
            if (DebugOptions.Enabled)
                await OidcSession.LogoutAsync();
        }

		[RelayCommand]
		public static void ClearFeedbackSurveyPrefs()
		{
			new SurveyFeedbackTracker(Preferences.Default).ClearAll();
		}

		[RelayCommand]
		public void ApplyCaseloadLastUpdated()
		{
			lastUpdatedPrefs.Set(GetCaseloadService.MakeId(), CaseloadLastUpdated);
		}
	}
}
