using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Oidc.Events;
using Visitz.Resources.Styles;
using Visitz.Views.Entity;
using Visitz.VisitzConfig;
using VisitzModel.Extensions;
using VisitzModel.Messaging;

namespace Visitz.Controls;

internal partial class VisitzAvatarView : AvatarView, IAsyncInitialize
{
    OidcSessionInfo? SessionInfo { get; set; }

    public Task? InitTask { get; }

    public VisitzAvatarView()
        : base()
    {
        OidcSession.SessionChanged += OidcSession_SessionChanged;

        TapGestureRecognizer tap = new();
        tap.Tapped += Tap_Tapped;
        GestureRecognizers.Add(tap);

        BackgroundColor = VisitzColors.BuildBarBackgroundColor;
        BorderWidth = 0;
        FontFamily = VisitzFonts.BcSansBoldAlias;
        HeightRequest = 30;
        WidthRequest = 30;
        TextColor = Colors.White;

        InitTask = InitAsync();
    }

    async Task InitAsync()
    {
        SessionInfo = await OidcSessionInfo.GetAsync();
        Text = SessionInfo.UserInitials;
    }

    private void Tap_Tapped(object? sender, TappedEventArgs e)
    {
        StrongReferenceMessenger.Default.Send(new NavDrawerMessage(isOpen: true));
    }

    private async void OidcSession_SessionChanged(object? sender, SessionChangedEventArgs e)
    {
        if (InitTask == null)
            return;

        try
        {
            await InitTask;
            Text = SessionInfo?.UserInitials ?? string.Empty;
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILogger<VisitzAvatarView>>().LogException(ex);
            Text = "??";
        }
    }
}
