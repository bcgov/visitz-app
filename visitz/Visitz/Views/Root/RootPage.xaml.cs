using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Foldable;
using Visitz.Animations;
using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using Visitz.Views.Entity;
using Visitz.Views.Snackbar;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Root;

#nullable enable

public partial class RootPage : VisitzPage<RootPage, RootViewModel>, ISnackbarPresenter
{
    VisitzSnackbar? Snackbar { get; set; }

    public RootPage(RootViewModel viewModel, ILogger<RootPage> logger)
        : base(viewModel, logger)
    {
        InitializeComponent();
        BindingContext = ViewModel;

        StrongReferenceMessenger.Default.Register<AppNavMessage>(this, ReceiveAppNavMessage);
        StrongReferenceMessenger.Default.Register<GetNavPositionMessage>(this, SendNavPosition);
        StrongReferenceMessenger.Default.Register<BusinessObjectSelectedMessage>(
            this,
            async (_, message) => await BusinessObjectSelected(message)
        );
        StrongReferenceMessenger.Default.Register<EntityNavBackMessage>(
            this,
            async (_, message) => await EntityNavBack(message)
        );

        HideSoftInputOnTapped = true;
    }

    private void ReceiveAppNavMessage(object recipient, AppNavMessage message)
    {
        if (message.Value is NavItem nav && nav.ContentViewType != null)
        {
            var content =
                ServiceProvider.GetService(nav.ContentViewType) as ContentView
                ?? throw new InvalidOperationException("Requested navigation item was null");

            SetContent(content);
        }
    }

    private void SetContent(IView view)
    {
        if (view is View v)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            // StackLayout with FillAndExpand has so far been the most reliable layout mechanism in MAUI, so we'll
            // suppress compiler warnings about it.

            v.HorizontalOptions = LayoutOptions.FillAndExpand;
            v.VerticalOptions = LayoutOptions.FillAndExpand;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        ContentPane.Clear();
        ContentPane.Add(view);
    }

    public void SetSnackbar(VisitzSnackbar? snackbar)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Snackbar?.ShouldClose -= Snackbar_ShouldClose;

            Snackbar = snackbar;
            SnackbarContainer.Content = Snackbar;
            SnackbarContainer.IsVisible = Snackbar != null;

            if (Snackbar != null)
            {
                Snackbar.ShouldClose += Snackbar_ShouldClose;
                _ = new VisibilityAnimation(showView: true, 150).Animate(Snackbar);
            }
        });
    }

    public void Snackbar_ShouldClose(object? sender, EventArgs e)
    {
        _ = AnimateCloseSnackbar();
    }

    private async Task AnimateCloseSnackbar()
    {
        if (Snackbar != null)
            await new VisibilityAnimation(showView: false, 150).Animate(Snackbar);

        SetSnackbar(null);
    }

    private void TwoPaneView_ModeChanged(object? sender, EventArgs e)
    {
        if (ViewModel is RootViewModel rvm && sender is TwoPaneView paneView)
        {
            rvm.UpdateOrientationVisibility(paneView.Mode);
            StrongReferenceMessenger.Default.Send(new NavPositionMessage((int)paneView.Mode));
        }
    }

    private static void SendNavPosition(object recipient, GetNavPositionMessage message)
    {
        if (recipient is RootPage root)
            message.Reply((int)root.TwoPane.Mode);
    }

    static async Task BusinessObjectSelected(BusinessObjectSelectedMessage message)
    {
        IBusinessObject item = message.Value;

        try
        {
            var entityPage = ServiceProvider.GetService<EntityPage>();

            entityPage.Init(
                item.Id,
                item.EntityType,
                item.DisplayName,
                item.FileNumber,
                message.Section,
                message.DraftItem
            );

            await Navigator.Navigation.PushAsync(entityPage);
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    static async Task EntityNavBack(EntityNavBackMessage message)
    {
        try
        {
            await Navigator.Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }
}
