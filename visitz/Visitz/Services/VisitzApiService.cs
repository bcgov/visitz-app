using System.Net;
using Visitz.Authentication.Keycloak;
using VisitzApi.ErrorHandling;

namespace Visitz.Services
{
    public abstract class VisitzApiService : VisitzService
    {
        public override async Task OnRunAsync()
        {
            try
            {
                await base.OnRunAsync();
            }
            catch (VisitzApiException ex)
            {
                if (ex.HttpStatusCode == HttpStatusCode.Unauthorized
                    || ex.HttpStatusCode == HttpStatusCode.Forbidden)
                {
                    VisitzSession.InvalidateSession();

                    // TODO: Properly notify user their session is invalid and
                    // prompt them to login again
#if DEBUG
                    Console.WriteLine(nameof(VisitzApiException) + $" {ex.HttpStatusCode} error");
                    Console.WriteLine(ex.StackTrace);
#endif
                }
            }
        }
    }
}
