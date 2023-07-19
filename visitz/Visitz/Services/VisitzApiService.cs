using System.Net;
using Visitz.Authentication.Keycloak;
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

        public override async Task OnRunAsync()
        {
            try
            {
                if (await VisitzSession.GetValidSessionAsync())
                    await base.OnRunAsync();
            }
            catch (VisitzApiException ex)
            {
#if DEBUG
                Console.WriteLine(nameof(VisitzApiException) 
                    + $" {ex.HttpStatusCode} -> {ex.Message}:\n{ex.StackTrace}");
#endif

                if (ex.HttpStatusCode == HttpStatusCode.Unauthorized
                    || ex.HttpStatusCode == HttpStatusCode.Forbidden)
                {
                    VisitzSession.InvalidateSession();

                    // TODO: Properly notify user their session is invalid and
                    // prompt them to login again
                }
            }
        }
    }
}
