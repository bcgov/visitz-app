using System.Net.Http.Headers;

namespace hestia.Services.Networking
{
    /// <summary>
    /// Injects the access token into each HTTP request to the API
    /// </summary>
	public class TokenHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string hardCodedToken = "<Token>";
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", hardCodedToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}

