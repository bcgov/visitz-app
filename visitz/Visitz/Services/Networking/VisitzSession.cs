using IdentityModel.OidcClient;
using Visitz.Services.Authentication;

namespace Visitz.Services.Networking
{
    public class VisitzSession
    {
        // TODO: Temporary naive access token handling until it's properly implemented
        public static bool IsValid => TokenHolder.AccessToken?.Length > 0;

        public static async Task<Result> PromptForLogin()
        {
            var authClient = Application.Current
                .Handler
                .MauiContext
                .Services
                .GetRequiredService<AuthenticationClient>();

            return HandleReponse(await authClient.LoginAsync());
        }

        private static Result HandleReponse(LoginResult loginResult)
        {
            if (!loginResult.IsError)
                // TODO: Incoming JWT token should be processed and stored in SecureStorage within this
                // class instead of using TokenHolder.
                TokenHolder.AccessToken = loginResult.AccessToken;

            return new Result(loginResult.IsError, loginResult.Error, loginResult.ErrorDescription);
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
