using System;
using hestia.Services;

namespace hestia.ViewModels
{
    /// <summary>
    /// The business logic for the local device authentication(Biometrics, PIN, Pattern etc) goes here.
    /// </summary>
	public class DeviceAuthenticationViewModel
	{
        DeviceAuthenticator authenticator;
        public DeviceAuthenticationViewModel(DeviceAuthenticator authenticator)
        {
            this.authenticator = authenticator;
        }

        public async Task<DeviceAuthenticator.Result> Authenticate()
        {
            return await authenticator.Authenticate();
        }
    }
}

