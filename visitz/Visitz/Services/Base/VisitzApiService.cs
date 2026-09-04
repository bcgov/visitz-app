using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using Microsoft.Extensions.Logging;
using Oidc;
using Oidc.Network;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.Snackbar;
using VisitzApi;
using VisitzApi.ErrorHandling;
using VisitzModel.Extensions;
using VisitzModel.Storage;
#if WINDOWS
using Visitz.WinUI;
#endif

namespace Visitz.Services.Base;

#nullable enable

public abstract class VisitzApiService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzService
{
    protected static ulong CurrentRefreshCallAttemptsCount = 0L;
    protected static ulong CurrentRefreshCallCompletedCount = 0L;
    protected static ulong TotalApiAttemptCount = 0L;
    protected static ulong TotalApiCompletedCount = 0L;
    protected static ConcurrentBag<(ulong RunNumber, string Id, Stopwatch)> CallTimings = [];

    protected Vpi Vpi { get; } = vpi;

    protected LastUpdatedPrefs LastUpdatedPrefs { get; } = prefs;

    protected bool ShouldWriteStats { get; set; }

    protected sealed override async Task RunServiceAsync()
    {
        if (!NetworkHelper.InternetAvailable)
        {
            SnackbarHandler.ShowTextWithDetails(
                LocalizedStrings.UnableToReachIcmDeviceOffline,
                LocalizedStrings.DeviceOffline,
                LocalizedStrings.DeviceOfflineDesc
            );

            ResultCode = Result.Cancelled;
            return;
        }

        try
        {
#if WINDOWS
            ((App)MauiWinUIApplication.Current).AuthCancelTokenSource = CancelTokenSource;
#endif
            await OidcSession.AssertValidSessionAsync(
                messageIfUnavailable: LocalizedStrings.NoInternet,
                CancelTokenSource.Token
            );

            Interlocked.Increment(ref CurrentRefreshCallAttemptsCount);
            ulong run = Interlocked.Increment(ref TotalApiAttemptCount);

            Stopwatch stopwatch = new();
            stopwatch.Start();

            try
            {
                await RunApiServiceAsync();
            }
            finally
            {
                stopwatch.Stop();

                CallTimings.Add((run, GetId(), stopwatch));

                Interlocked.Increment(ref CurrentRefreshCallCompletedCount);
                Interlocked.Increment(ref TotalApiCompletedCount);
            }

            await OidcSession.SetAuthorization(authorized: true);

            await TrySetLastUpdated();
        }
        catch (OperationCanceledException)
        {
            ResultCode = Result.Cancelled;
            throw;
        }
        catch (Exception ex)
        {
            if (FindApiException(ex) is VisitzApiException vex && IsUnauthorized(vex.HttpStatusCode))
            {
                await OidcSession.SetAuthorization(authorized: false);
                await ClearIcmDataRealm();

                throw new UnauthorizedAccessException(LocalizedStrings.UnauthorizedForApi, vex);

                // No need for different messages for 401 vs. 403, since
                // 401 would've been handled by the OAuth login.
            }

            throw;
        }
        finally
        {
            if (ShouldWriteStats)
                WriteTimingsToDesktop();
        }
    }

    static VisitzApiException? FindApiException(Exception ex)
    {
        if (ex is VisitzApiException vex)
            return vex;
        else if (ex.InnerException is AggregateException aex)
        {
            foreach (var e in aex.InnerExceptions)
                if (FindApiException(e) is VisitzApiException innerVex)
                    return innerVex;
        }
        else if (ex.InnerException != null)
            return FindApiException(ex.InnerException);

        return null;
    }

    protected abstract Task RunApiServiceAsync();

    private static bool IsUnauthorized(HttpStatusCode code)
    {
        return code == HttpStatusCode.Unauthorized || code == HttpStatusCode.Forbidden;
    }

    private async Task TrySetLastUpdated()
    {
        if (ResultCode == Result.Successful)
            await MainThread.InvokeOnMainThreadAsync(() => LastUpdatedPrefs.SetLocalNow(GetId()));
    }

    private async Task ClearIcmDataRealm()
    {
        try
        {
            using var icmData = await VisitzRealms.GetIcmDataRealmAsync();
            await icmData.WriteAsync(icmData.RemoveAll);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    static void WriteTimingsToDesktop()
    {
        (ulong runNumber, string Id, Stopwatch)[] snapshot = CallTimings.ToArray();

        string date = DateTimeOffset.Now.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filepath = Path.Combine(desktopPath, $"API call timings {date}.csv");

        using var writer = new StreamWriter(filepath);

        foreach (var (run, id, sw) in snapshot)
            writer.WriteLine($"{run},{id},{sw.ElapsedMilliseconds}");
    }
}
