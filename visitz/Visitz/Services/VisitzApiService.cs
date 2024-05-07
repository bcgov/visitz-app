using IdentityModel.OidcClient.Browser;
using Oidc;
using Oidc.Exceptions;
using System.Net;
using Visitz.Resources.Localization;
using VisitzApi;
using VisitzApi.ErrorHandling;

namespace Visitz.Services
{
    public abstract class VisitzApiService(Vpi vpi) : VisitzService
    {
        protected Vpi Vpi { get; set; } = vpi;

        protected override sealed async Task RunServiceAsync()
        {
			if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
			{
				SnackbarHandler.ShowTextWithDetails(
					LocalizedStrings.UnableToReachIcmDeviceOffline,
					LocalizedStrings.DeviceOffline,
					LocalizedStrings.DeviceOfflineDesc);

				ResultCode = Result.Cancelled;
				return;
			}

            try
            {
				var cancelTokenSource = new CancellationTokenSource();
#if WINDOWS
				(Application.Current as VisitzApp).AuthCancelTokenSource = cancelTokenSource;
#endif
				await OidcSession.AssertValidSessionAsync(
					messageIfUnavailable: LocalizedStrings.NoInternet,
					cancelTokenSource.Token);

                await RunApiServiceAsync();
            }
            catch (LoginException ex)
            {
                if (ex.Message.Equals(BrowserResultType.UserCancel.ToString()))
                {
                    ResultCode = Result.Cancelled;
                    throw new OperationCanceledException(BrowserResultType.UserCancel.ToString(), ex);
                }

                throw;
            }
            catch (VisitzApiException ex)
            {
#if DEBUG
                Console.WriteLine(nameof(VisitzApiException) 
                    + $" {ex.HttpStatusCode} -> {ex.Message}:\n{ex.StackTrace}");
#endif
                if (IsSessionException(ex.HttpStatusCode))
                {
                    await OidcSession.InvalidateSessionAsync();
                    throw new UnauthorizedAccessException(LocalizedStrings.UnauthorizedForApi, ex);

                    // No need for different messages for 401 vs. 403, since 401 would've been handled by the
                    // OAuth login.
                }

                throw;
            }
        }

        protected abstract Task RunApiServiceAsync();

        private static bool IsSessionException(HttpStatusCode code)
        {
            return code == HttpStatusCode.Unauthorized || code == HttpStatusCode.Forbidden;
        }
    }
}
