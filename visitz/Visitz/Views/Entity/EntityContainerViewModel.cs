using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Resources.Styles;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity;

public partial class EntityContainerViewModel :
    VisitzViewModel,
    IBusinessObjectHolder,
    IRecipient<ServiceStateMessage>
{
    ServiceHandler ServiceHandler { get; }

    [ObservableProperty]
    public IBusinessObject businessObject;

    [ObservableProperty]
    public bool showDownloadActivity;

    [ObservableProperty]
    public Color entityTypeTextColor;

    [ObservableProperty]
    public string fullTypeCased;

    public EntityContainerViewModel() : base()
    {
        ServiceHandler = ServiceProvider.GetService<ServiceHandler>();
    }

    protected override ILogger<VisitzViewModel> MakeLogger()
    {
        return ServiceProvider.GetService<ILogger<EntityContainerViewModel>>();
    }

    protected override Task InitAsync()
    {
        var init = base.InitAsync();

        UpdateDownloadActivity();
        ServiceHandler.ServiceStarted += ServiceHandler_ServiceStarted;
        ServiceHandler.ServiceFinished += ServiceHandler_ServiceFinished;

        UpdateLocalActivityTimestamp();

        return init;
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            ServiceHandler.ServiceStarted -= ServiceHandler_ServiceStarted;
            ServiceHandler.ServiceFinished -= ServiceHandler_ServiceFinished;

            WeakReferenceMessenger.Default.UnregisterAll(this);

            disposed = true;
        }
        base.Dispose(disposing);
    }

    void UpdateDownloadActivity()
    {
        ShowDownloadActivity = BusinessObject.IsValid
            && ServiceHandler.IsAnyServiceRunning(BusinessObject.Id);
    }

    private void ServiceHandler_ServiceStarted(object sender, string e)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(UpdateDownloadActivity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    private void ServiceHandler_ServiceFinished(object sender, VisitzService e)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(UpdateDownloadActivity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    public async void Receive(ServiceStateMessage message)
    {
        ShowDownloadActivity = message.IsRunning;

        if (message.FinishedError)
        {
            string displayString = $"{BusinessObject.EntityType} {BusinessObject.DisplayName}";
            string msg = string.Format(LocalizedStrings.DownloadRecordErrorMessage, displayString);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(
                msg,
                message.UncaughtException?.ToString(),
                LocalizedStrings.DownloadError);
        }
    }

    partial void OnBusinessObjectChanged(IBusinessObject oldValue, IBusinessObject newValue)
    {
        EntityTypeTextColor = newValue?.EntityType.GetTextColor() ?? VisitzColors.BC_TextColor;
        FullTypeCased = newValue?.FullType.ToTitleCase();
    }

    private void UpdateLocalActivityTimestamp()
    {
        BusinessObject.LocalState.LastOpenedBinding = DateTimeOffset.UtcNow;
    }
}
