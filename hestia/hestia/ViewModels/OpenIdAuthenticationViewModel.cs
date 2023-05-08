using System;
using hestia.Services.Authentication;
using hestia.Services.Networking;
using IdentityModel.OidcClient;

namespace hestia.ViewModels
{
    /// <summary>
    /// The OAuth login process is managed by this class.
    /// </summary>
	public class OpenIdAuthenticationViewModel
    {
        private readonly AuthenticationClient authClient;

        public OpenIdAuthenticationViewModel(AuthenticationClient authClient)
        {
            this.authClient = authClient;
        }

        public async Task<Result> Authenticate()
        {
            try
            {
                var loginResult = await authClient.LoginAsync();

                if (!loginResult.IsError)
                    TokenHolder.AccessToken = loginResult.AccessToken;

                return new Result(isError: loginResult.IsError, loginResult.Error, errorDescription: loginResult.ErrorDescription);
            }
            catch (Exception ex)
            {
                return new Result(isError: true, error: "Exception", errorDescription: ex.ToString());
            }
        }

        public struct Result
        {
            public bool IsError;
            public string Error;
            public string ErrorDescription;

            public Result(bool isError, string error, string errorDescription)
            {
                IsError = isError;
                Error = error;
                ErrorDescription = errorDescription;
            }
        }
    }
}

