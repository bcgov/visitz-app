using Microsoft.Extensions.Logging;
using System.ComponentModel;
using Visitz.Services;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadItemView : BaseContentView
{
    DraftIndicatorHelper? IndicatorHelper { get; set; }

    CaseloadItemViewModel? Previous { get; set; }

    public CaseloadItemView() : base()
    {
        InitializeComponent();
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
            TryDetachBusinessObject();

            vm.UpdateDraftIndicatorVisibility();
            vm.UpdateStateVisibility();

            Attach(vm);
        }
    }

    void Attach(CaseloadItemViewModel vm)
    {
        vm.BusinessObject.LocalState.PropertyChanged += LocalState_PropertyChanged;
        Previous = vm;

        if (IndicatorHelper == null)
        {
            IndicatorHelper = vm.IndicatorHelper;
            IndicatorHelper.PropertyChanged += IndicatorHelper_PropertyChanged;
        }
    }

    void TryDetachBusinessObject()
    {
        if (Previous != null)
            Previous.BusinessObject.LocalState.PropertyChanged -= LocalState_PropertyChanged;
    }

    void IndicatorHelper_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    { 
        if (e.PropertyName == nameof(DraftIndicatorHelper.DraftedItems)
            && BindingContext is CaseloadItemViewModel vm)
        {
            vm.UpdateDraftIndicatorVisibility();
        }
    }

    private void LocalState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BoLocalState.ShouldDownloadDuringRefresh)
            && BindingContext is CaseloadItemViewModel vm)
        {
            vm.UpdateStateVisibility();
        }
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            if (IndicatorHelper != null)
                IndicatorHelper.PropertyChanged -= IndicatorHelper_PropertyChanged;

            TryDetachBusinessObject();

            disposed = true;
        }
        base.Dispose(disposing);
    }
}
