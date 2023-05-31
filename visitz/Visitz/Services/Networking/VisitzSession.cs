using IdentityModel.OidcClient;
using Visitz.Services.Authentication;

namespace Visitz.Services.Networking
{
    public class VisitzSession
    {
        public static async Task<bool> GetValidSessionAsync()
        {
            if (await TokenHolder.IsAccessTokenValid())
                return true;
            else
            {
                var result = await DoLoginAsync();

                return !result.IsError;
            }
        }

        private static async Task<Result> DoLoginAsync()
        {
            var authClient = Application.Current
                .Handler
                .MauiContext
                .Services
                .GetRequiredService<AuthenticationClient>();

            return await HandleReponse(await authClient.LoginAsync());
        }

        private async static Task<Result> HandleReponse(LoginResult loginResult)
        {
            if (!loginResult.IsError)
                await TokenHolder.SaveAsync(loginResult);

            return new Result(loginResult.IsError, loginResult.Error, loginResult.ErrorDescription);
        }

        public static async Task<VisitzSessionInfo> GetInfoAsync()
        {
            return await VisitzSessionInfo.GetAsync();
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
