using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Visitz.Views.BaseClasses;
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

        UpdateDownloadIconVisibility();
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

    public void UpdateDownloadIconVisibility()
    {
        ShowDownloadIcon = BusinessObject.LocalState.ShouldDownloadDuringRefresh;
    }

    [RelayCommand]
    public static void BusinessObjectSelected(IBusinessObject record)
    {
        StrongReferenceMessenger.Default.Send(new BusinessObjectSelectedMessage(record));
    }
}
