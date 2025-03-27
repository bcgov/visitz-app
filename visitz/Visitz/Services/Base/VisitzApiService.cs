using Oidc;
using Oidc.Network;
using System.Net;
using Visitz.Resources.Localization;
using Visitz.Views.Snackbar;
using VisitzApi;
using VisitzApi.ErrorHandling;
using VisitzModel.Storage;

namespace Visitz.Services.Base
{
    public abstract class VisitzApiService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzService
    {
        protected Vpi Vpi { get; } = vpi;

        protected LastUpdatedPrefs LastUpdatedPrefs { get; } = prefs;

        protected override sealed async Task RunServiceAsync()
        {
            if (!NetworkHelper.InternetAvailable)
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

                await TrySetLastUpdated();
            }
            catch (OperationCanceledException)
            {
                ResultCode = Result.Cancelled;
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

        private async Task TrySetLastUpdated()
        {
            if (ResultCode == Result.Successful)
                await MainThread.InvokeOnMainThreadAsync(
                    () => LastUpdatedPrefs.SetLocalNow(GetId()));
        }
    }
}
