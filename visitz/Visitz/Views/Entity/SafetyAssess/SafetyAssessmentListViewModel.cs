using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Models;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.SafetyAssess;

public partial class SafetyAssessmentListViewModel : IcmRecordViewModel
{
    [ObservableProperty]
    public partial string EditViewButtonText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditViewButtonGlyph { get; set; } = string.Empty;

    readonly ObservableCollection<SafetyAssessment> _queriedAssessments = [];

    [ObservableProperty]
    public partial ObservableCollection<SafetyAssessmentListItemViewModel> Assessments { get; set; } = [];

    readonly IComparer<SafetyAssessmentListItemViewModel> _insertComparer =
        Comparer<SafetyAssessmentListItemViewModel>.Create(
            (l, r) => l.SafetyAssessment.CreatedDate.CompareTo(r.SafetyAssessment.CreatedDate)
        );

    readonly ObservableRealmQueryMap _realmQueryMap = new();

    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        _realmQueryMap.ItemsChanged += RealmQueryMap_ItemsChanged;
        _queriedAssessments.CollectionChanged += QueriedAssessments_CollectionChanged;

        var draftRealm = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
        var draftQuery = AssessmentDraft.GetAllByFileNumber(draftRealm, BusinessObject.FileNumber);
        _realmQueryMap.Subscribe(draftRealm, draftQuery);

        var dataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        var dataQuery = SafetyAssessment.GetAllByFileNumber(dataRealm, BusinessObject.FileNumber);
        _realmQueryMap.Subscribe(dataRealm, dataQuery);
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            _realmQueryMap.ItemsChanged -= RealmQueryMap_ItemsChanged;
            _queriedAssessments.CollectionChanged -= QueriedAssessments_CollectionChanged;

            _realmQueryMap?.Dispose();
            _queriedAssessments.Clear();
            Assessments.Clear();

            disposed = true;
        }

        base.Dispose(disposing);
    }

    private void RealmQueryMap_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        if (e.Type == typeof(SafetyAssessment))
            UpdateQueriedAssessmentsList(e.Items, e.Changes);
        if (e.Type == typeof(AssessmentDraft))
            UpdateEditViewButtonText(e.Items.Any());
    }

    void UpdateQueriedAssessmentsList(IRealmCollection<IRealmObject> items, ChangeSet? changes)
    {
        if (changes == null)
        {
            _queriedAssessments.AddAll(items.Cast<SafetyAssessment>());
        }
        else
        {
            foreach (var i in changes.DeletedIndices.Reverse())
                _queriedAssessments.RemoveAt(i);

            foreach (var i in changes.InsertedIndices)
                _queriedAssessments.Add((SafetyAssessment)items.ElementAt(i));
        }

        IsEmpty = !Assessments.Any();
    }

    void QueriedAssessments_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (var item in e.NewItems.Cast<SafetyAssessment>())
                Assessments.InsertSorted(new(item), _insertComparer, ascending: false);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (var item in e.OldItems.Cast<SafetyAssessment>())
                TryRemove(item);
        }
    }

    void TryRemove(SafetyAssessment item)
    {
        SafetyAssessmentListItemViewModel? found = Assessments.FirstOrDefault(vm => vm.SafetyAssessment.Id == item.Id);
        if (found != null)
            Assessments.Remove(found);
    }

    void UpdateEditViewButtonText(bool draftAvailable)
    {
        EditViewButtonText = draftAvailable ? LocalizedStrings.ContinueDraft : LocalizedStrings.AddNew;
        EditViewButtonGlyph = draftAvailable ? MaterialIcons.Assignment : MaterialIcons.Assignment_add;
    }

    [RelayCommand]
    public async Task OpenSafetyAssessmentView(SafetyAssessment? assessment = null)
    {
        var view = ServiceProvider.GetService<SafetyAssessmentEditView>();

        view.BusinessObject = BusinessObject;
        view.ViewModel.RowId = RowId;
        view.ViewModel.EntityType = EntityType;

        if (assessment != null)
            view.ViewAssessment(assessment);

        await Navigator.Navigation.PushModalAsync(view);
    }
}
