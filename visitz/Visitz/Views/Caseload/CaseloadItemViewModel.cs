using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Views.BaseClasses;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadItemViewModel : VisitzViewModel
{
    public static readonly FontImageSource RemoveImageSource = new()
    {
        FontFamily = FluentIcons.FontConfig.FontFamily,
        Glyph = FluentIcons.Subtract_circle_20_regular,
    };

    readonly ServiceHandler serviceHandler = ServiceProvider.GetService<ServiceHandler>();

    OidcSessionInfo SessionInfo { get; }

    [ObservableProperty]
    public IBusinessObject businessObject;

    [ObservableProperty]
    public DraftIndicatorHelper indicatorHelper;

    [ObservableProperty]
    public bool showDate;

    [ObservableProperty]
    public bool showDraftIndicator;

    [ObservableProperty]
    public bool showDownloadIcon;

    [ObservableProperty]
    public bool showProgressIndicator;

    [ObservableProperty]
    public bool canRemoveFromDevice;

    public CaseloadItemViewModel(
        DraftIndicatorHelper indicatorHelper,
        IBusinessObject businessObject,
        OidcSessionInfo sessionInfo) : base()
    {
        IndicatorHelper = indicatorHelper;
        BusinessObject = businessObject;
        SessionInfo = sessionInfo;

        UpdateStateVisibility();
        StartInitAsync();
    }

    protected override ILogger<VisitzViewModel> MakeLogger()
    {
        return ServiceProvider.GetService<ILogger<CaseloadItemViewModel>>();
    }

    public void UpdateDraftIndicatorVisibility()
    {
        var draftItems = IndicatorHelper.DraftedItems;

        if (draftItems != null)
        {
            var idType = (BusinessObject.Id, BusinessObject.EntityType);
            var numType = (BusinessObject.FileNumber, BusinessObject.EntityType);

            ShowDraftIndicator = draftItems.Contains(idType) || draftItems.Contains(numType);
        }
        else
            ShowDraftIndicator = false;
    }

    public void UpdateStateVisibility()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var state = serviceHandler.GetAnyServiceStateByIdSubstring(BusinessObject.Id);
            bool isntMarkedForDownload = !BusinessObject.LocalState.ShouldDownloadDuringRefresh;

            if (state == VisitzService.State.Running)
            {
                ShowProgressIndicator = true;
                ShowDownloadIcon = !ShowProgressIndicator;
            }
            else if (isntMarkedForDownload)
            {
                ShowProgressIndicator = false;
                ShowDownloadIcon = !ShowProgressIndicator;
            }
            else
            {
                ShowProgressIndicator = false;
                ShowDownloadIcon = false;
            }

            ShowDate = !ShowDownloadIcon && !ShowProgressIndicator;
        });
    }

    public void UpdateIsAssigned()
    {
        CanRemoveFromDevice = !BusinessObject.IsAssigned(SessionInfo.Idir)
            && BusinessObject.LocalState.ShouldDownloadDuringRefresh;
    }

    [RelayCommand]
    public void BusinessObjectSelected(IBusinessObject record)
    {
        StrongReferenceMessenger.Default.Send(new BusinessObjectSelectedMessage(record));

        bool markForDownload = !record.LocalState.ShouldDownloadDuringRefresh;
        if (markForDownload)
        {
            record.LocalState.ShouldDownloadDuringRefreshBinding = true;

            var msg = GetAllDataForRecordService.MakeStartMessage(record);
            WeakReferenceMessenger.Default.Send(msg);

            UpdateStateVisibility();
            UpdateIsAssigned();
        }
    }

    [RelayCommand]
    public async Task UnloadDependentData(IBusinessObject record)
    {
        if (record.IsAssigned(SessionInfo.Idir))
        {
            string assignedMsg = string.Format(
                LocalizedStrings.RemoveFromDeviceErrorAssigned,
                BusinessObject.EntityType,
                BusinessObject.DisplayName);

            await Navigator.CurrentOpenPage.DisplayAlert(
                LocalizedStrings.UnableToRemove,
                assignedMsg,
                LocalizedStrings.Cancel);
            return;
        }

        string message = string.Format(LocalizedStrings.RemoveFromDeviceMessage,
            BusinessObject.EntityType,
            BusinessObject.DisplayName);

        bool shouldRemove = await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.RemoveFromDevice,
            message,
            LocalizedStrings.RemoveFromDevice,
            LocalizedStrings.Cancel);

        if (shouldRemove)
        {
            record.LocalState.ShouldDownloadDuringRefreshBinding = false;
            UpdateStateVisibility();
            UpdateIsAssigned();
        }
    }
}
