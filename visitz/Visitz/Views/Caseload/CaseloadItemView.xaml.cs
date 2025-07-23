using Microsoft.Extensions.Logging;
using System.ComponentModel;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadItemView : BaseContentView
{
    readonly ServiceHandler serviceHandler = ServiceProvider.GetService<ServiceHandler>();

    DraftIndicatorHelper? IndicatorHelper { get; set; }

    public CaseloadItemView() : base()
    {
        InitializeComponent();

        serviceHandler.ServiceStarted += ServiceHandler_ServiceStarted;
        serviceHandler.ServiceFinished += ServiceHandler_ServiceFinished;
    }

    protected override ILogger<BaseContentView> MakeLogger()
    {
        return ServiceProvider.GetService<ILogger<CaseloadItemView>>();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is CaseloadItemViewModel vm)
        {
            vm.UpdateDraftIndicatorVisibility();
            vm.UpdateStateVisibility();
            vm.UpdateIsAssigned();

            TryAttach(vm);
        }
    }

    void TryAttach(CaseloadItemViewModel vm)
    {
        if (IndicatorHelper == null)
        {
            IndicatorHelper = vm.IndicatorHelper;
            IndicatorHelper.PropertyChanged += IndicatorHelper_PropertyChanged;
        }
    }

    void IndicatorHelper_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(DraftIndicatorHelper.DraftedItems)
                && BindingContext is CaseloadItemViewModel vm)
            {
                vm.UpdateDraftIndicatorVisibility();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    void ServiceHandler_ServiceStarted(object? sender, string e)
    {
        try
        {
            if (BindingContext is CaseloadItemViewModel vm)
                vm.UpdateStateVisibility();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    void ServiceHandler_ServiceFinished(object? sender, VisitzService service)
    {
        try
        {
            if (BindingContext is CaseloadItemViewModel vm)
            vm.UpdateStateVisibility();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            if (IndicatorHelper != null)
                IndicatorHelper.PropertyChanged -= IndicatorHelper_PropertyChanged;

            serviceHandler.ServiceFinished -= ServiceHandler_ServiceFinished;

            disposed = true;
        }
        base.Dispose(disposing);
    }
}
