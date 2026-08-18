using System.Globalization;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Oidc;
using Visitz.Resources.Localization;
using Visitz.Storage;
using VisitzModel.Storage;
#if IOS
using CommunityToolkit.Maui.Behaviors;
#endif

namespace Visitz.Views;

#nullable enable

public partial class LastUpdatedBanner : ContentView
{
    [BindableProperty(PropertyChangedMethodName = nameof(SetUpdatedText))]
    public partial DateTime? LastUpdated { get; set; }

    [BindableProperty(PropertyChangedMethodName = nameof(SetUpdatedText))]
    public partial string FallbackText { get; set; } = LocalizedStrings.NA;

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

    private static void SetUpdatedText(object boundObj, object _, object newVal)
    {
        var thiz = (LastUpdatedBanner)boundObj;

        thiz.SetLastUpdated(newVal as DateTime?);
    }

    private void SetLastUpdated(DateTime? lastUpdated)
    {
        LastUpdatedSpan.Text = lastUpdated is DateTime last
            ? last.ToString("yyyy-MMM-dd h:mm tt", CultureInfo.InvariantCulture)
            : LastUpdatedSpan.Text = FallbackText;
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
