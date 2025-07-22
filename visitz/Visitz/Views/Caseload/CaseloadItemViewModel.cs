using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Visitz.Services.Caseload;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadItemViewModel : VisitzViewModel
{
    [ObservableProperty]
    public IBusinessObject businessObject;

    [ObservableProperty]
    public DraftIndicatorHelper indicatorHelper;

    [ObservableProperty]
    public bool showDraftIndicator;

    [ObservableProperty]
    public bool showDownloadIcon;

    public CaseloadItemViewModel(
        DraftIndicatorHelper indicatorHelper,
        IBusinessObject businessObject) : base()
    {
        IndicatorHelper = indicatorHelper;
        BusinessObject = businessObject;

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
        bool isntMarkedForDownload = !BusinessObject.LocalState.ShouldDownloadDuringRefresh;
        ShowDownloadIcon = isntMarkedForDownload;
    }

    [RelayCommand]
    public void BusinessObjectSelected(IBusinessObject record)
    {
        StrongReferenceMessenger.Default.Send(new BusinessObjectSelectedMessage(record));

        bool markForDownload = !record.LocalState.ShouldDownloadDuringRefresh;
        if (markForDownload)
        {
            record.Commit(() =>
                record.LocalState.ShouldDownloadDuringRefresh = true);

            var msg = GetAllDataForRecordService.MakeStartMessage(BusinessObject);
            WeakReferenceMessenger.Default.Send(msg);
        }
    }
}
