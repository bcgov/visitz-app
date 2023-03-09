using System;
using hestia.Routers;
using hestia.Views;

namespace hestia.ViewModels
{
	/// <summary>
	/// The logic to render the appropriate initial screen to the user after a app launch resides here.
	/// </summary>
	// To capture the query parameter sent during the Shell navigation
    [QueryProperty(nameof(BackFromPage), "navigatingBackFromPage")]
    public class LandingViewModel : BaseViewModel
    {
		bool IsDeviceAuthenticationDone;
		bool IsOpenIdAuthenticationDone;

        string page;
        public string BackFromPage
        {
			get => page;
            set
            {
				page = value;
				// Naviagtes back once the device authentication is successful.
				if (page.Equals(nameof(DeviceAuthenticationPage)))
				{
					IsDeviceAuthenticationDone = true;
				}
                // Naviagtes back once the OpenId authentication is successful.
                else if (page.Equals(nameof(OpenIdAuthenticationPage)))
				{
					IsOpenIdAuthenticationDone = true;
                }
                OnPropertyChanged(); // To trigger the property listener on the code-behind
            }
        }

        public LandingRouter.Route SolveRoute()
		{
			if (IsDeviceAuthenticationDone)
			{
				return IsOpenIdAuthenticationDone ? LandingRouter.Route.CasesAndIncidents :
					LandingRouter.Route.OpenIdAuthentication;
			}
			else
			{
				return LandingRouter.Route.DeviceAuthentication;
			}
		}
	}
}

