using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.SafetyAssess;

internal partial class SafetyAssessmentListViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

    protected override async Task InitAsync()
    {
        await base.InitAsync();
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            // TODO: diposal

            disposed = true;
        }

        base.Dispose(disposing);
    }

    [RelayCommand]
    public async Task OpenSafetyAssessmentView(SafetyAssessment assessment = null)
    {
        var view = ServiceProvider.GetService<SafetyAssessmentEditView>();

        view.CaseloadItem = CaseloadItem;
        view.Assessment = assessment;

        await Navigator.Navigation.PushModalAsync(view);
    }
}
