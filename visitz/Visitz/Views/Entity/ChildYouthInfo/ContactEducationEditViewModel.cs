using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.ListOfValues;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactEducationEditViewModel : DraftEditViewModel<ContactEducationDraft>
{
    bool _disposed;

    public override Func<ContactEducationDraft, INotifyPropertyChanged?> ObjectToObserveChanges =>
        static draft => draft.ContactEducation;

    public override string DiscardWarningMessage =>
        string.Format(
            LocalizedStrings.DiscardThisDraftDescription,
            LocalizedStrings.ContactEducation.ToLowerInvariant()
        );

    public override string DiscardedMessage =>
        string.Format(LocalizedStrings.DiscardedThisDraft, LocalizedStrings.ContactEducation.ToLowerInvariant());

    [ObservableProperty]
    public partial ObservableQuery<ListedValue>? DegreeQuery { get; set; }

    [ObservableProperty]
    public partial ListedValue? SelectedDegree { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        DegreeQuery = new(DataRealm, ListedValue.GetAllByType(DataRealm, ContactEducation.DegreeLookupLovKey));

        await DegreeQuery.Loaded;
        SelectedDegree = DegreeQuery.Items.GetListedValue(Draft?.ContactEducation?.Degree);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            DegreeQuery?.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    protected override Task DoPublishDraftAsync()
    {
        // TODO: wire up publishing
        return Task.CompletedTask;
    }

    protected override bool EnablePublish(ContactEducationDraft draft)
    {
        // TODO: wire up validation for publishing
        return false;
    }

    partial void OnSelectedDegreeChanged(ListedValue? value)
    {
        if (IsInitialized && value?.Value is string degree)
            Draft?.ContactEducation?.DegreeBinding = degree;
    }
}
