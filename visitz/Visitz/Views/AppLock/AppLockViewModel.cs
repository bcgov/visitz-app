using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.Fingerprint.Abstractions;
using Visitz.Device;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Surveys;
using VisitzModel.Storage;

namespace Visitz.Views.AppLock;

public partial class AppLockViewModel() : VisitzViewModel
{
    static readonly RowDefinitionCollection s_heroRows =
    [
        new() { Height = new GridLength(3, GridUnitType.Star) },
        new() { Height = GridLength.Star },
        new() { Height = GridLength.Star },
    ];
    static readonly RowDefinitionCollection s_nonHeroRows =
    [
        new() { Height = GridLength.Star },
        new() { Height = GridLength.Star },
        new() { Height = GridLength.Star },
    ];

    [ObservableProperty]
    public partial bool ShowHeroImage { get; set; } = true;

    [ObservableProperty]
    public partial RowDefinitionCollection RowDefinitions { get; set; } = s_heroRows;

    public static async Task PromptAuthentication()
    {
        (bool available, _) = await DeviceAuthenticator.GetAvailabilityAsync();

        if (available)
            await PromptBiometricAuth();
    }

    static async Task PromptBiometricAuth()
    {
        var result = await DeviceAuthenticator.Authenticate(
            LocalizedStrings.UnlockApp,
            LocalizedStrings.DeviceAuthReason
        );

        await HandleAuthResult(result);
    }

    public static async Task HandleAuthResult(FingerprintAuthenticationResult result)
    {
        switch (result.Status)
        {
            case FingerprintAuthenticationResultStatus.Succeeded:
                await HandleSuccessfulAuth();
                break;
            case FingerprintAuthenticationResultStatus.NotAvailable:
                await Navigator.CurrentOpenPage.DisplayAlertAsync(
                    LocalizedStrings.EnableDeviceSecurity,
                    LocalizedStrings.SecureDeviceAndTryAgain,
                    LocalizedStrings.Ok
                );
                break;
            case FingerprintAuthenticationResultStatus.Canceled:
                /* No-op */
                break;
            case FingerprintAuthenticationResultStatus.FallbackRequested:
            case FingerprintAuthenticationResultStatus.TooManyAttempts:
            case FingerprintAuthenticationResultStatus.Failed:
            case FingerprintAuthenticationResultStatus.Denied:
            case FingerprintAuthenticationResultStatus.Unknown:
            case FingerprintAuthenticationResultStatus.UnknownError:
                await Navigator.CurrentOpenPage.DisplayErrorAlert(result.ErrorMessage);
                break;
            default:
                throw new ArgumentException($"Unsupported {nameof(FingerprintAuthenticationResult)}");
        }
    }

    static async Task HandleSuccessfulAuth()
    {
        await Navigator.Navigation.PopModalAsync();

        new SurveyFeedbackTracker(Preferences.Default).IncrementTimesAppUnlocked();
        await FeedbackSurveyPage.TryOpen();

        StrongReferenceMessenger.Default.Send(new AppLockMessage(AppLockStatus.Closed));
    }

    partial void OnShowHeroImageChanged(bool value)
    {
        RowDefinitions = value ? s_heroRows : s_nonHeroRows;
    }
}
