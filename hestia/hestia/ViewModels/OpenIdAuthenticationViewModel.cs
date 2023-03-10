using System;
using hestia.Services.Authentication;

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
            var loginResult = await authClient.LoginAsync();
            if (loginResult.IsError)
            {
                //Console.WriteLine($"loginResult.ErrorDescription: {loginResult.ErrorDescription}");
                return new Result(isError: true, error: loginResult.Error, errorDescription: loginResult.ErrorDescription);
            }
            else
            {
                //Console.WriteLine("loginResult");
                //Console.WriteLine($"loginResult: AccessToken: {loginResult.AccessToken}");
                //Console.WriteLine($"loginResult: RefreshToken: {loginResult.RefreshToken}");
                return new Result(isError: false, error: null, errorDescription: null);
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

