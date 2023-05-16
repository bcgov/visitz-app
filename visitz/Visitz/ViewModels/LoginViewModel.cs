using Visitz.Services.Authentication;
using Visitz.Services.Networking;
using Visitz.Routers;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The OAuth login process is managed by this class.
    /// </summary>
	public class LoginViewModel : VisitzViewModel
    {
        private OpenIdAuthenticationRouter Router { get; }

        private AuthenticationClient AuthClient { get; }

        public LoginViewModel(OpenIdAuthenticationRouter router, AuthenticationClient authClient)
        {
            Router = router;
            AuthClient = authClient;
        }

        public async override void PageStarted()
        {
            Result result = await Authenticate();
            Router.routeUsing(result);
        }

        public async Task<Result> Authenticate()
        {
            try
            {
                var loginResult = await AuthClient.LoginAsync();

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

