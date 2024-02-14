using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Device;
using Visitz.Pages;
using Visitz.Resources.Localization;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    public partial class AppLockViewModel : VisitzViewModel
    {
        private DeviceAuthenticator Authenticator { get; }

        [ObservableProperty]
        public string backgroundImageUri;

        public AppLockViewModel(DeviceAuthenticator authenticator)
        {
            Authenticator = authenticator;
        }

        public override async void Create()
        {
            base.Create();

            BackgroundImageUri = await BcGovAlbum.GetFeaturedPictureUri();
        }

        public override async void Start()
        {
            base.Start();

            await PromptAuthentication();
        }

        public async Task PromptAuthentication()
        {
            DeviceAuthenticator.Result result = await Authenticator.Authenticate();
            await RouteUsing(result);
        }

        public async Task RouteUsing(DeviceAuthenticator.Result result)
        {
            switch (result)
            {
                case DeviceAuthenticator.Result.NotConfigured:
                    await Navigator.CurrentOpenPage.DisplayAlert(
                        LocalizedStrings.EnableDeviceSecurity,
                        LocalizedStrings.SecureDeviceAndTryAgain,
                        LocalizedStrings.Ok
                    );
                    break;
                case DeviceAuthenticator.Result.Successful:
                    await Navigator.Navigation.PopModalAsync();
                    break;
            }
        }
    }
}

