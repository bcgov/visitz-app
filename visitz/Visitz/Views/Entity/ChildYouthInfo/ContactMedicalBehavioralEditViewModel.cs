using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Models;
using VisitzModel.Models.ListOfValues;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactMedicalBehavioralEditViewModel : DraftEditViewModel<ContactMedicalBehavioralDraft>
{
    bool _disposed;

    public override Func<ContactMedicalBehavioralDraft, INotifyPropertyChanged?> ObjectToObserveChanges =>
        static draft => draft.ContactMedicalBehavioral;

    public override string DiscardWarningMessage =>
        string.Format(
            LocalizedStrings.DiscardThisDraftDescription,
            LocalizedStrings.ContactMedicalBehavioral.ToLowerInvariant()
        );

    public override string DiscardedMessage =>
        string.Format(
            LocalizedStrings.DiscardedThisDraft,
            LocalizedStrings.ContactMedicalBehavioral.ToLowerInvariant()
        );

    [ObservableProperty]
    public partial ObservableQuery<ListedValue> CategoriesQuery { get; set; }

    [ObservableProperty]
    public partial ObservableQuery<ListedValue> ConditionsQuery { get; set; }

    [ObservableProperty]
    public partial ListedValue? SelectedCategory { get; set; }

    [ObservableProperty]
    public partial ListedValue? SelectedCondition { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        CategoriesQuery = new(
            DataRealm,
            ListedValue.GetAllByType(DataRealm, ContactMedicalBehavioral.CategoriesLovKey),
            // Upstream does some funky stuff with the explicit sort order, so we'll override and sort only by name.
            addItemToTargetList: (item, targetList) => targetList.InsertSorted(item, ListedValue.NameOnlyComparer)
        );
        await CategoriesQuery.Loaded;
        SelectedCategory = CategoriesQuery.Items.GetListedValue(Draft?.ContactMedicalBehavioral?.Category);

        ConditionsQuery = new(
            realm: DataRealm,
            query: ListedValue.GetAllByType(DataRealm, ContactMedicalBehavioral.ConditionNamesLovKey),
            addItemToTargetList: AddCondition
        );
        await ConditionsQuery.Loaded;
        SelectedCondition = ConditionsQuery.Items.GetListedValue(Draft?.ContactMedicalBehavioral?.Condition);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            CategoriesQuery?.Dispose();
            ConditionsQuery?.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    void AddCondition(ListedValue item, ObservableCollection<ListedValue> targetList)
    {
        if (item.Parent == SelectedCategory?.Value)
            targetList.InsertSorted(item);
    }

    protected override Task DoPublishDraftAsync()
    {
        // TODO: wire up publishing
        return Task.CompletedTask;
    }

    partial void OnSelectedCategoryChanged(ListedValue? value)
    {
        if (IsInitialized && value?.Value is string category)
        {
            Draft?.ContactMedicalBehavioral?.CategoryBinding = category;
            ConditionsQuery.RefreshList();
        }
    }

    partial void OnSelectedConditionChanged(ListedValue? value)
    {
        if (IsInitialized && value?.Value is string condition)
            Draft?.ContactMedicalBehavioral?.ConditionBinding = condition;
    }
}
