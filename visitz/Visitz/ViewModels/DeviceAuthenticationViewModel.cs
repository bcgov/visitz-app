using System;
using Visitz.Services;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the local device authentication(Biometrics, PIN, Pattern etc) goes here.
    /// </summary>
	public class DeviceAuthenticationViewModel : VisitzViewModel
    {
        DeviceAuthenticator authenticator;
        public DeviceAuthenticationViewModel(DeviceAuthenticator authenticator)
        {
            this.authenticator = authenticator;
        }

        public override async void PageStarted()
        {
            return await authenticator.Authenticate();
        }
    }
}

