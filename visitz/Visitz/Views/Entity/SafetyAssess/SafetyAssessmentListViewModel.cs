using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.SafetyAssess;

internal partial class SafetyAssessmentListViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public string editViewButtonText;

    readonly ObservableRealmQueryMap realmQueryMap = new();

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        realmQueryMap.ItemsChanged += RealmQueryMap_ItemsChanged;

        var draftRealm = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
        var query = AssessmentDraft.GetAllByFileNumber(draftRealm, CaseloadItem.CaseIncidentNumber);
        realmQueryMap.Subscribe(draftRealm, query);
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            realmQueryMap?.Dispose();

            disposed = true;
        }

        base.Dispose(disposing);
    }

    private void RealmQueryMap_ItemsChanged(
        object sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
    {
        if (e.Type == typeof(AssessmentDraft))
            UpdateEditViewButtonText(e.Items.Any());
    }

    void UpdateEditViewButtonText(bool draftAvailable)
    {
        EditViewButtonText = draftAvailable ? LocalizedStrings.ContinueDraft : LocalizedStrings.AddNew;
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
