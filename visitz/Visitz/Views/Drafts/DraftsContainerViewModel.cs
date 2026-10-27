using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Controls;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Notes;
using VisitzModel.Models.People;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Drafts;

public partial class DraftsContainerViewModel : VisitzViewModel
{
    static readonly FilterOption<IDraftItem> s_allDrafts = new(LocalizedStrings.AllTypes, _ => true);

    readonly FilterOption<IDraftItem> _attachmentsFilter = new(
        LocalizedStrings.Attachments,
        draft => draft is AttachmentDraft
    );

    readonly FilterOption<IDraftItem> _notesFilter = new(LocalizedStrings.Notes, draft => draft is NoteDraft);

    readonly FilterOption<IDraftItem> _safetyAssessmentFilter = new(
        LocalizedStrings.SafetyAssessments,
        draft => draft is AssessmentDraft
    );

    readonly FilterOption<IDraftItem> _visitsFilter = new(
        LocalizedStrings.ChildYouthVisits,
        draft => draft is PersonVisitDraft
    );

    readonly FilterOption<IDraftItem> _contactEducationFilter = new(
        LocalizedStrings.ContactEducation,
        draft => draft is ContactEducationDraft
    );

    readonly FilterOption<IDraftItem> _contactLanguageFilter = new(
        LocalizedStrings.ContactLanguages,
        draft => draft is ContactLanguageDraft
    );

    readonly FilterOption<IDraftItem> _contactMedicalBehavioralFilter = new(
        LocalizedStrings.ContactMedicalBehavioral,
        draft => draft is ContactMedicalBehavioralDraft
    );

    readonly ObservableRealmCount _realmCount = new();

    public DraftsListViewModel? DraftsListViewModel { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<FilterOption<IDraftItem>> FilterOptions { get; set; } = [s_allDrafts];

    [ObservableProperty]
    public partial FilterOption<IDraftItem> SelectedFilter { get; set; } = s_allDrafts;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        _realmCount.CountChanged += RealmCount_CountChanged;

        _realmCount.Subscribe<AttachmentDraft>(await VisitzRealms.GetAttachmentDraftsRealmAsync());
        _realmCount.Subscribe<NoteDraft>(await VisitzRealms.GetNoteDraftsRealmAsync());
        _realmCount.Subscribe<AssessmentDraft>(await VisitzRealms.GetSafetyAssessmentDraftRealmAsync());
        _realmCount.Subscribe<PersonVisitDraft>(await VisitzRealms.GetPersonVisitDraftsRealmAsync());
        _realmCount.Subscribe<ContactEducationDraft>(await VisitzRealms.GetUserDataRealmAsync());
        _realmCount.Subscribe<ContactLanguageDraft>(await VisitzRealms.GetUserDataRealmAsync());
        _realmCount.Subscribe<ContactMedicalBehavioralDraft>(await VisitzRealms.GetUserDataRealmAsync());
    }

    bool _disposed;

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
            _realmCount.CountChanged -= RealmCount_CountChanged;
            _realmCount.Dispose();

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    void RealmCount_CountChanged(object? sender, (Type Kind, int Count) e)
    {
        if (e.Kind == typeof(AttachmentDraft))
            UpdateItem(_attachmentsFilter, e.Count);
        else if (e.Kind == typeof(NoteDraft))
            UpdateItem(_notesFilter, e.Count);
        else if (e.Kind == typeof(AssessmentDraft))
            UpdateItem(_safetyAssessmentFilter, e.Count);
        else if (e.Kind == typeof(PersonVisitDraft))
            UpdateItem(_visitsFilter, e.Count);
        else if (e.Kind == typeof(ContactEducationDraft))
            UpdateItem(_contactEducationFilter, e.Count);
        else if (e.Kind == typeof(ContactLanguageDraft))
            UpdateItem(_contactLanguageFilter, e.Count);
        else if (e.Kind == typeof(ContactMedicalBehavioralDraft))
            UpdateItem(_contactMedicalBehavioralFilter, e.Count);
    }

    void UpdateItem(FilterOption<IDraftItem> filter, int count)
    {
        if (count <= 0)
            FilterOptions.Remove(filter);
        else if (!FilterOptions.Contains(filter))
            FilterOptions.InsertSorted(filter, startIndex: 1);
    }

    partial void OnSelectedFilterChanged(FilterOption<IDraftItem> value)
    {
        DraftsListViewModel?.SelectedFilter = value;
    }
}
