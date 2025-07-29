using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Resources.Styles;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
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
    [ObservableProperty]
    public IBusinessObject businessObject;

    [ObservableProperty]
    public bool showDownloadActivity;

    [ObservableProperty]
    public Color entityTypeTextColor;

    [ObservableProperty]
    public string fullTypeCased;

    protected override Task InitAsync()
    {
        var init = base.InitAsync();

        string id = GetAllDataForRecordService.MakeId(BusinessObject);

        ServiceHandler services = ServiceProvider.GetService<ServiceHandler>();
        ShowDownloadActivity = services.GetServiceState(id) == VisitzService.State.Running;

        WeakReferenceMessenger.Default.Register(this, id);

        return init;
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            disposed = true;
        }
        base.Dispose(disposing);
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
}
