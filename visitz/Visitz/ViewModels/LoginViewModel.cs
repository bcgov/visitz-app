using Visitz.Services.Authentication;
using Visitz.Services.Networking;
using Visitz.Views;
using Visitz.Resources.Localization;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The OAuth login process is managed by this class.
    /// </summary>
	public class LoginViewModel : VisitzViewModel
    {
        private AuthenticationClient AuthClient { get; }

        public LoginViewModel(AuthenticationClient authClient)
        {
            AuthClient = authClient;
        }

        public async override void PageStarted()
        {
            Result result = await Authenticate();
            await RouteUsing(result);
        }

        public async Task RouteUsing(Result result)
        {
            if (result.IsError)
                // TODO: Show this error using UI instead of an alert
                await Shell.Current.DisplayAlert(
                    LocalizedStrings.LoginError,
                    result.ErrorDescription,
                    LocalizedStrings.Ok
                );
            else
                await NavigateTo(typeof(CaseloadPage));
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

