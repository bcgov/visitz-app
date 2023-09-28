using System.Net;
using Visitz.Authentication.Keycloak;
using Visitz.Resources.Localization;
using VisitzApi;
using VisitzApi.ErrorHandling;

namespace Visitz.Services
{
    public abstract class VisitzApiService : VisitzService
    {
        protected Vpi Vpi { get; set; }

        public VisitzApiService(Vpi vpi)
        {
            Vpi = vpi;
        }

        protected override sealed async Task RunServiceAsync()
        {
            await VisitzSession.AssertValidSessionAsync();

            try
            {
                await RunApiServiceAsync();
            }
            catch (VisitzApiException ex)
            {
#if DEBUG
                Console.WriteLine(nameof(VisitzApiException) 
                    + $" {ex.HttpStatusCode} -> {ex.Message}:\n{ex.StackTrace}");
#endif
                if (IsSessionException(ex.HttpStatusCode))
                {
                    await VisitzSession.InvalidateSessionAsync();
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
