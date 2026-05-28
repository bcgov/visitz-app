using System.Net.Http.Headers;

namespace Oidc;

/// <summary>
/// Forces a workaround Accept-Language header. Some environments don't accept the automatic header provided by iOS
/// (a mix of en-CA and en-US in our case) and will fail when parsing the expected DateTime structure
/// (MMM YYYY instead of MMM. YYYY).
/// </summary>
public class AppendAcceptLanguageHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        request.Headers.AcceptLanguage.Clear();
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
        return await base.SendAsync(request, cancellationToken);
    }
}
