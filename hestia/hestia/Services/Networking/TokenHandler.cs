using System.Net.Http.Headers;

namespace visitz.Services.Networking
{
    /// <summary>
    /// Injects the access token into each HTTP request to the API
    /// </summary>
	public class TokenHandler : DelegatingHandler
    {
        private static readonly string Bearer = "Bearer";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(Bearer, TokenHolder.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}

