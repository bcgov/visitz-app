using Microsoft.Extensions.Logging;
using Realms;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Interfaces;

namespace Visitz.Views.Drafts;

#nullable enable

public partial class DraftsListItem : BaseContentView
{
    readonly ServiceHandler serviceHandler = ServiceProvider.GetService<ServiceHandler>();

    public DraftsListItem()
    {
        InitializeComponent();

        serviceHandler.ServiceStarted += ServiceHandler_ServiceStarted;
        serviceHandler.ServiceFinished += ServiceHandler_ServiceFinished;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        UpdateDownloadActivityIndicator();
    }

    private void ServiceHandler_ServiceFinished(object? sender, VisitzService e)
    {
        MainThread.BeginInvokeOnMainThread(UpdateDownloadActivityIndicator);
    }

    private void ServiceHandler_ServiceStarted(object? sender, string e)
    {
        MainThread.BeginInvokeOnMainThread(UpdateDownloadActivityIndicator);
    }

    private void UpdateDownloadActivityIndicator()
    {
        try
        {
            bool isRunning =
                BindingContext is IRealmObject realmObj
                && realmObj.IsValid
                && BindingContext is IRecordInfo info
                && serviceHandler.IsAnyServiceRunning(info.RelatedEntityId);

            DownloadActivity.IsRunning = isRunning;
            DownloadActivity.IsVisible = isRunning;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Couldn't update network activity UI for {nameof(DraftsListItem)}");
        }
    }
}
