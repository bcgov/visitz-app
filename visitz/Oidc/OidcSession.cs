using System.IdentityModel.Tokens.Jwt;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;
using Oidc.Events;
using Oidc.Exceptions;
using Oidc.Network;
using Oidc.Util;

#nullable enable

namespace Oidc
{
    public class OidcSession
    {
        private static readonly string s_idirActiveKey = "idir_active_employee";
        private static readonly SemaphoreSlim s_validSession = new(1);
        private static readonly SemaphoreSlim s_canSetAuthorization = new(1);

        public static readonly double StaleThresholdMinutes = 7 * TimeSpan.MinutesPerDay;

        private static AuthenticationClient AuthClient =>
            ServicesProvider.Current.GetRequiredService<AuthenticationClient>();

        public static event EventHandler<SessionChangedEventArgs>? SessionChanged;

        public static async Task<bool> IsSessionValid()
        {
            return await TokenHolder.IsAccessTokenValid() || !await TokenHolder.IsRefreshTokenExpired();
        }

        static async Task DoAssertValidSessionAsync(
            string messageIfUnavailable,
            CancellationToken cancellationToken = default
        )
        {
            NetworkHelper.AssertInternetAvailable(messageIfUnavailable);

            if (!await TokenHolder.IsAccessTokenValid())
            {
                if (await TokenHolder.IsRefreshTokenExpired())
                    await LoginAsync(messageIfUnavailable, cancellationToken);
                else
                    await RefreshAsync(messageIfUnavailable);
            }
        }

        public static async Task AssertValidSessionAsync(
            string messageIfUnavailable,
            CancellationToken cancellationToken = default
        )
        {
            await s_validSession.WaitAsync(cancellationToken);

            try
            {
                await DoAssertValidSessionAsync(messageIfUnavailable, cancellationToken);
            }
            finally
            {
                try
                {
                    s_validSession.Release();
                }
                catch { }
            }
        }

        public static async Task LoginAsync(string messageIfUnavailable, CancellationToken cancellationToken = default)
        {
            LoginResult? loginResult = null;

            try
            {
                NetworkHelper.AssertInternetAvailable(messageIfUnavailable);

                loginResult = await AuthClient.LoginAsync(cancellationToken);

                if (loginResult == null)
                    throw new LoginException("No login result");
                else if (loginResult.IsError)
                {
                    if (loginResult.Error == BrowserResultType.UserCancel.ToString())
                        // WORKAROUND String compare to BrowserResultType is a limitation of the library
                        throw new OperationCanceledException();
                    else
                        throw new LoginException($"{loginResult.Error}: '{loginResult.ErrorDescription}'");
                }

                await TokenHolder.SaveAsync(loginResult);
            }
            finally
            {
                bool success = !loginResult?.IsError ?? false;
#if DEBUG
                ConsoleTrace.TraceMethod(
                    typeof(OidcSession),
                    $"Login success: '{success}', Error: {loginResult?.Error} -> '{loginResult?.ErrorDescription}'"
                );
#endif
                var info = await OidcSessionInfo.GetAsync();
                SessionChanged?.Invoke(info, new LoginChangedEventArgs() { Success = success });
            }
        }

        private static async Task RefreshAsync(string messageIfUnavailable)
        {
            RefreshTokenResult? refreshResult = null;

            try
            {
                NetworkHelper.AssertInternetAvailable(messageIfUnavailable);

                var refreshToken = await TokenHolder.GetRefreshTokenStringAsync();
                refreshResult = await AuthClient.RefreshAsync(refreshToken);

                if (refreshResult == null || refreshResult.IsError)
                    throw new SessionRefreshException(refreshResult?.Error ?? "No refresh result");

                await TokenHolder.SaveAsync(refreshResult);
            }
            finally
            {
#if DEBUG
                ConsoleTrace.TraceMethod(
                    typeof(OidcSession),
                    $"refreshResult.IsError: '{refreshResult?.IsError}', error: '{refreshResult?.Error}'"
                );
#endif
                var info = await OidcSessionInfo.GetAsync();
                SessionChanged?.Invoke(
                    info,
                    new RefreshChangedEventArgs() { Success = !refreshResult?.IsError ?? false }
                );
            }
        }

        public static async Task<bool> LogoutAsync()
        {
            var logoutResult = await AuthClient.LogoutAsync();
            var logoutSuccess = !logoutResult.IsError;

#if DEBUG
            ConsoleTrace.TraceMethod(
                typeof(OidcSession),
                $"logoutResult.IsError: '{logoutResult.IsError}', error: '{logoutResult.Error}'"
            );
#endif
            await InvalidateSessionAsync();

            var info = await OidcSessionInfo.GetAsync();

            if (logoutSuccess)
                info.OfficeNames = [];

            SessionChanged?.Invoke(info, new LogoutChangedEventArgs() { Success = logoutSuccess });

            return logoutSuccess;
        }

        public static async Task LocalLogoutAsync()
        {
#if DEBUG
            ConsoleTrace.TraceMethod(typeof(OidcSession), $"Local logout initiated");
#endif
            await InvalidateSessionAsync();

            var info = await OidcSessionInfo.GetAsync();
            info.OfficeNames = [];

            SessionChanged?.Invoke(info, new LogoutChangedEventArgs() { Success = true });
        }

        public static async Task<OidcSessionInfo> GetInfoAsync()
        {
            return await OidcSessionInfo.GetAsync();
        }

        public static async Task InvalidateSessionAsync()
        {
            TokenHolder.DeleteAccessToken();
            TokenHolder.DeleteRefreshToken();
            TokenHolder.DeleteIdentityToken();
            await SetAuthorization(authorized: null);

            var info = await OidcSessionInfo.GetAsync();
            SessionChanged?.Invoke(info, new SessionInvalidatedEventArgs() { Success = true });
        }

        public static async Task<bool> SessionExistsAsync()
        {
            return await TokenHolder.GetAccessTokenStringAsync() is not null
                && await TokenHolder.GetRefreshTokenStringAsync() is not null
                && await TokenHolder.GetIdentityTokenStringAsync() is not null;
        }

        public static async Task<bool?> IsSessionStale(double? minutesSinceExpiration = null)
        {
            JwtSecurityToken? access = await TokenHolder.GetAccessTokenAsync();
            TimeSpan? diff = DateTime.UtcNow - access?.ValidTo;
            return diff?.TotalMinutes >= (minutesSinceExpiration ?? StaleThresholdMinutes);
        }

        public static async Task<bool?> IsAuthorizedAsync()
        {
            var status = await SecureStorage.Default.GetAsync(s_idirActiveKey);
            return status != null ? bool.Parse(status) : null;
        }

        public static async Task SetAuthorization(bool? authorized)
        {
            await s_canSetAuthorization.WaitAsync();

            try
            {
                if (authorized is bool auth)
                    await SecureStorage.Default.SetAsync(s_idirActiveKey, auth.ToString());
                else
                    SecureStorage.Default.Remove(s_idirActiveKey);
            }
            finally
            {
                try
                {
                    s_canSetAuthorization.Release();
                }
                catch { }
            }
        }
    }
}
