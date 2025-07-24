using Microsoft.Extensions.Logging;
using System.ComponentModel;
using Visitz.Extensions;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadItemView : BaseContentView
{
    ServiceHandler? serviceHandler = ServiceProvider.GetService<ServiceHandler>();

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

    protected override void OnParentChanging(ParentChangingEventArgs args)
    {
        base.OnParentChanging(args);

        if (args.AttachingToParent()
            && GetParentCaseloadViewModel(args.NewParent) is CaseloadViewModel vm)
        {
            IndicatorHelper = vm.IndicatorHelper;
            IndicatorHelper.PropertyChanged += IndicatorHelper_PropertyChanged;
        }
        else if (args.DetachingFromParent() && IndicatorHelper != null)
            Dispose();
    }

    static CaseloadViewModel? GetParentCaseloadViewModel(Element parent)
    {
        if (parent == null)
            return null;
        else if (parent.BindingContext is CaseloadViewModel vm)
            return vm;
        else
            return GetParentCaseloadViewModel(parent.Parent);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is CaseloadItemViewModel vm)
        {
            vm.UpdateDraftIndicatorVisibility();
            vm.UpdateStateVisibility();
            vm.UpdateIsAssigned();
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
            {
                IndicatorHelper.PropertyChanged -= IndicatorHelper_PropertyChanged;
                IndicatorHelper = null;
            }

            if (serviceHandler != null)
            {
                serviceHandler.ServiceStarted -= ServiceHandler_ServiceStarted;
                serviceHandler.ServiceFinished -= ServiceHandler_ServiceFinished;
                serviceHandler = null;
            }

            disposed = true;
        }
        base.Dispose(disposing);
    }
}
