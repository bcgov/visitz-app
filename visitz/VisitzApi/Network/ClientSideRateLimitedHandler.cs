using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

namespace VisitzApi.Network;

public class ClientSideRateLimitedHandler : DelegatingHandler
{
    RateLimiter RateLimiter { get; } =
        new TokenBucketRateLimiter(
            new()
            {
                TokenLimit = 40, // arbitrarily chosen
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = short.MaxValue,
                ReplenishmentPeriod = TimeSpan.FromMilliseconds(10), // arbitrarily chosen
                TokensPerPeriod = 1, // arbitrarily chosen
                AutoReplenishment = true,
            }
        );

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        using RateLimitLease lease = await RateLimiter.AcquireAsync(permitCount: 1, cancellationToken);

        if (lease.IsAcquired)
            return await base.SendAsync(request, cancellationToken);

        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);

        if (lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
            response.Headers.Add(
                "Retry-After",
                ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo)
            );

        return response;
    }

    public async ValueTask DisposeAsync()
    {
        await RateLimiter.DisposeAsync().ConfigureAwait(false);

        Dispose(disposing: false);

        GC.SuppressFinalize(this);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            RateLimiter.Dispose();
        }
    }
}
