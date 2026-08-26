using Microsoft.Extensions.Logging;
using Oidc;
using Oidc.Network;
using Visitz.Resources.Localization;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Views.Debugging;
using Visitz.Views.Snackbar;
using Visitz.Views.User;
using VisitzModel.Storage;
#if !WINDOWS
using Visitz.Views.AppLock;
#endif

namespace Visitz.Services.Caseload;

#nullable enable

internal class AutoRefreshService(LastUpdatedPrefs prefs, ServiceHandler serviceHandler) : VisitzService()
{
    private static readonly string Id = nameof(AutoRefreshService);
    private static readonly int CooldownDurationSeconds = 3600 * 3;

    public static readonly string CooldownTimestampUtc = "LastRefreshAttempt";

    readonly LastUpdatedPrefs LastUpdatedPrefs = prefs;

    ServiceHandler ServiceHandler { get; set; } = serviceHandler;

    static VisitzWindow? Window => Application.Current?.Windows[0] as VisitzWindow;

    public static string MakeId()
    {
        return Id;
    }

    public static StartServiceMessage MakeStartMessage(bool forceDownload = false)
    {
        return new StartServiceMessage
        {
            ServiceId = MakeId(),
            ServiceType = typeof(AutoRefreshService),
            Payload = forceDownload,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunServiceAsync()
    {
        Logger.LogTrace("Attempting auto caseload refresh");

        if (!await CanRefresh())
        {
            Logger.LogTrace("Conditions failed, auto refresh cancelled.");
            ResultCode = Result.Cancelled;
            return;
        }

        bool sessionInvalid = !await OidcSession.IsSessionValid();
        if (sessionInvalid && Window?.Page != null)
            await Window.Page.DisplayAlertAsync(
                LocalizedStrings.CaseloadRefresh,
                LocalizedStrings.AutoCaseloadRefreshDesc,
                LocalizedStrings.Ok
            );

        if (!NetworkHelper.InternetAvailable)
        {
            SnackbarHandler.ShowTextWithDetails(
                LocalizedStrings.CantRefreshNoInternet,
                LocalizedStrings.RefreshInterrupted,
                LocalizedStrings.RefreshInterruptedDesc
            );
            ResultCode = Result.Cancelled;
            return;
        }

        if (DebugOptions.Default.AutoCaseloadRefreshDisabled)
        {
            SnackbarHandler.ShowText(LocalizedStrings.AutoRefreshDebugDisabled);
            ResultCode = Result.Cancelled;
            return;
        }

        Logger.LogTrace("Auto caseload refresh proceeding");

        await RunAllDataService();
    }

    async Task RunAllDataService()
    {
        try
        {
            await ServiceHandler.TryRunServiceAsync(GetAllDataForOfflineService.MakeStartMessage());

            ResultCode = Result.Successful;
            LastUpdatedPrefs.SetUtcNow(CooldownTimestampUtc);
        }
        catch (OperationCanceledException opEx)
        {
            Logger.LogInformation($"Auto refresh cancelled: '{opEx.Message}'");
            ResultCode = Result.Cancelled;
        }
        catch (Exception)
        {
            LastUpdatedPrefs.SetUtcNow(CooldownTimestampUtc);
            throw;
        }
    }

    async Task<bool> CanRefresh()
    {
        bool elapsed = CooldownElapsed();
        bool unlocked = AppUnlockedOrFocused();
        bool sessionpageClosed = !SessionPage.IsOpen;
        bool authorized = (await OidcSession.IsAuthorizedAsync() ?? false);
        bool internetAvailable = NetworkHelper.InternetAvailable;

        string prefix = "Auto refresh: ";

        Logger.LogInformation(prefix + "cooldown " + (elapsed ? "elapsed" : "ongoing"));

        if (!unlocked)
            Logger.LogInformation(prefix + "app not unlocked/focused");

        if (!sessionpageClosed)
            Logger.LogInformation(prefix + "session page is open");

        if (!authorized)
            Logger.LogInformation(prefix + "user not authorized");

        if (!internetAvailable)
            Logger.LogInformation(prefix + "internet unavailable");

        return elapsed && unlocked && sessionpageClosed && authorized && internetAvailable;
    }

    static bool AppUnlockedOrFocused()
    {
#if WINDOWS
        return Window?.IsActivated ?? false;
#else
        return !AppLockPage.IsOpen;
#endif
    }

    bool CooldownElapsed()
    {
        if (LastUpdatedPrefs.Get(CooldownTimestampUtc) is DateTime lastAttempt)
        {
            double secondsDiff = (DateTime.UtcNow - lastAttempt).TotalSeconds;
            bool cooldownElapsed = secondsDiff > CooldownDurationSeconds;

            int remainingSeconds = CooldownDurationSeconds - (int)secondsDiff;
            double percent = secondsDiff / CooldownDurationSeconds * 100;

            Logger.LogInformation(
                "Cooldown {p}% ({sec}s/{min}m remaining)",
                percent,
                remainingSeconds,
                remainingSeconds / 60
            );

            return cooldownElapsed;
        }

        return true;
    }
}
