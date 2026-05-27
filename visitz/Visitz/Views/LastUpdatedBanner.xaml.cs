using System.Globalization;
using CommunityToolkit.Maui.Core;
using Oidc;
using Visitz.Resources.Localization;
using Visitz.Storage;
using VisitzModel.Formats;
using VisitzModel.Storage;
#if IOS
using CommunityToolkit.Maui.Behaviors;
#endif

namespace Visitz.Views;

#nullable enable

public partial class LastUpdatedBanner : ContentView
{
    public static readonly BindableProperty LastUpdatedProperty = BindableProperty.Create(
        nameof(LastUpdated),
        typeof(DateTime?),
        typeof(LastUpdatedBanner),
        propertyChanged: SetUpdatedText
    );

    public static readonly BindableProperty FallbackTextProperty = BindableProperty.Create(
        nameof(FallbackText),
        typeof(string),
        typeof(LastUpdatedBanner),
        propertyChanged: SetUpdatedText,
        defaultValue: LocalizedStrings.NA
    );

    private static void SetUpdatedText(object boundObj, object _, object newVal)
    {
        var thiz = (LastUpdatedBanner)boundObj;

        thiz.SetLastUpdated(newVal as DateTime?);
    }

    public DateTime? LastUpdated
    {
        get => (DateTime?)GetValue(LastUpdatedProperty);
        set => SetValue(LastUpdatedProperty, value);
    }

    public string FallbackText
    {
        get => (string)GetValue(FallbackTextProperty);
        set => SetValue(FallbackTextProperty, value);
    }

    public LastUpdatedBanner()
    {
        InitializeComponent();
        SetLastUpdated(null);
#if IOS
        var touch = new TouchBehavior()
        {
            DefaultAnimationEasing = Easing.CubicInOut,
            LongPressDuration = 600,
            PressedScale = 1.01d,
        };
        touch.LongPressCompleted += TouchBehavior_LongPressCompleted;
        Behaviors.Add(touch);
#endif
    }

    private void SetLastUpdated(DateTime? lastUpdated)
    {
        LastUpdatedLabel.Text = lastUpdated is DateTime last
            ? last.ToString(IcmDateFormats.BasicTimestamp, CultureInfo.InvariantCulture)
            : LastUpdatedLabel.Text = FallbackText;
    }

    async void MenuFlyoutItem_Clicked(object? sender, EventArgs e)
    {
        await ShowStats();
    }

    async void TouchBehavior_LongPressCompleted(object? sender, LongPressCompletedEventArgs e)
    {
        await ShowStats();
    }

    static async Task ShowStats()
    {
        var realm = await VisitzRealms.GetIcmDataRealmAsync();
        var info = await OidcSessionInfo.GetAsync();
        await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.LocalCaseloadStats,
            await IcmData.GetStats(realm, info.Idir),
            LocalizedStrings.Ok
        );
    }
}
