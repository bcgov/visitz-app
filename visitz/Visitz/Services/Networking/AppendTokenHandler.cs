using System.Net.Http.Headers;

namespace Visitz.Services.Networking
{
    /// <summary>
    /// Injects the access token into each HTTP request to the API
    /// </summary>
	public class AppendTokenHandler : DelegatingHandler
    {
        private static readonly string Bearer = "Bearer";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var jwtToken = await TokenHolder.GetAccessTokenStringAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue(Bearer, jwtToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}

