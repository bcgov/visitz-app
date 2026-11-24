using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Storage;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Notes;
using VisitzModel.Models.People;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Caseload;

public partial class DraftIndicatorHelper : ObservableObject, IDisposable
{
    readonly ObservableRealmQueryMap realmQueryMap = new();

    bool disposedValue;

    public Task InitTask { get; }

    [ObservableProperty]
    public partial HashSet<(string EntityId, EntityType Type)> DraftedNotes { get; set; } = [];

    [ObservableProperty]
    public partial HashSet<(string EntityId, EntityType Type)> DraftedAssessments { get; set; } = [];

    [ObservableProperty]
    public partial HashSet<(string EntityId, EntityType Type)> DraftedAttachments { get; set; } = [];

    [ObservableProperty]
    public partial HashSet<(string EntityId, EntityType Type)> DraftedVisits { get; set; } = [];

    [ObservableProperty]
    public partial HashSet<(string EntityId, EntityType Type)> DraftedContactEducations { get; set; } = [];

    [ObservableProperty]
    public partial HashSet<(string EntityId, EntityType Type)> DraftedContactMedicalBehaviorals { get; set; } = [];

    [ObservableProperty]
    public partial HashSet<(string EntityId, EntityType Type)> DraftedContactLanguages { get; set; } = [];

    [ObservableProperty]
    public partial HashSet<(string EntityId, EntityType Type)> DraftedItems { get; set; } = [];

    public DraftIndicatorHelper()
    {
        realmQueryMap.ItemsChanged += RealmQueryMap_DraftsChanged;
        PropertyChanged += DraftIndicatorHelper_PropertyChanged;
        InitTask = InitAsync();
    }

    public async Task InitAsync()
    {
        var noteDraft = await VisitzRealms.GetNoteDraftsRealmAsync();
        realmQueryMap.Subscribe(noteDraft, noteDraft.All<NoteDraft>());

        var assessmentDraft = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
        realmQueryMap.Subscribe(assessmentDraft, assessmentDraft.All<AssessmentDraft>());

        var attachmentDraft = await VisitzRealms.GetAttachmentDraftsRealmAsync();
        realmQueryMap.Subscribe(attachmentDraft, attachmentDraft.All<AttachmentDraft>());

        var visitDraft = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
        realmQueryMap.Subscribe(visitDraft, visitDraft.All<PersonVisitDraft>());

        var userData = await VisitzRealms.GetUserDataRealmAsync();
        realmQueryMap.Subscribe(userData, userData.All<ContactEducationDraft>());
        realmQueryMap.Subscribe(userData, userData.All<ContactLanguageDraft>());
        realmQueryMap.Subscribe(userData, userData.All<ContactMedicalBehavioralDraft>());
    }

    private void RealmQueryMap_DraftsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        HashSet<(string EntityId, EntityType Type)> drafted = [];

        foreach (var item in e.Items.Cast<IDraftItem>())
            drafted.Add((item.RelatedEntityId, item.RelatedEntityType));

        if (e.Type == typeof(NoteDraft))
            DraftedNotes = drafted;
        else if (e.Type == typeof(AssessmentDraft))
            DraftedAssessments = drafted;
        else if (e.Type == typeof(AttachmentDraft))
            DraftedAttachments = drafted;
        else if (e.Type == typeof(PersonVisitDraft))
            DraftedVisits = drafted;
        else if (e.Type == typeof(ContactEducationDraft))
            DraftedContactEducations = drafted;
        else if (e.Type == typeof(ContactLanguageDraft))
            DraftedContactLanguages = drafted;
        else if (e.Type == typeof(ContactMedicalBehavioralDraft))
            DraftedContactMedicalBehaviorals = drafted;
    }

    private void DraftIndicatorHelper_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Rebuild DraftedItems whenever a subset changes
        if (e.PropertyName?.StartsWith("Drafted") == true && e.PropertyName != nameof(DraftedItems))
            BuildDraftedItems();

        // TODO: Rework this implementation so we don't need to rebuild the entire DraftedItems HashSet whenever one
        // of its subsets change. Maybe some kind of Dictionary that holds a counter for each RowId/EntityType tuple?
    }

    private void BuildDraftedItems()
    {
        var newSet = new HashSet<(string EntityId, EntityType Type)>();
        newSet.UnionWith(DraftedNotes);
        newSet.UnionWith(DraftedAssessments);
        newSet.UnionWith(DraftedAttachments);
        newSet.UnionWith(DraftedVisits);
        newSet.UnionWith(DraftedContactEducations);
        newSet.UnionWith(DraftedContactLanguages);
        newSet.UnionWith(DraftedContactMedicalBehaviorals);
        DraftedItems = newSet;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                realmQueryMap?.Dispose();

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
