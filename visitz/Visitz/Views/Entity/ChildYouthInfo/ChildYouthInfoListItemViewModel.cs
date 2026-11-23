using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ChildYouthInfoListItemViewModel<TDraftItem, TReadItem>(TDraftItem? draftItem, TReadItem? readItem)
    : VisitzViewModel,
        IComparable<ChildYouthInfoListItemViewModel<TDraftItem, TReadItem>>
    where TDraftItem : IDraftItem
{
    [ObservableProperty]
    public partial TDraftItem? DraftItem { get; set; } = draftItem;

    [ObservableProperty]
    public partial TReadItem? ReadItem { get; set; } = readItem;

    public int CompareTo(ChildYouthInfoListItemViewModel<TDraftItem, TReadItem>? other)
    {
        if (other == null)
            return 1;

        if (DraftItem != null)
        {
            int draftCreatedCompare = DraftItem.DraftCreated.CompareTo(
                other.DraftItem?.DraftCreated ?? DateTimeOffset.MinValue
            );
            if (draftCreatedCompare != 0)
                return draftCreatedCompare;
        }
        else if (other.DraftItem != null)
            return -1;

        return Comparer<TReadItem>.Default.Compare(ReadItem, other.ReadItem);
    }
}

// TS: I wasn't able to use the above generic class directly in XAML DataTemplates, so I created the following
// concrete ViewModels instead as a workaround.

public partial class EducationListItemViewModel(ContactEducationDraft? draftItem, ContactEducation? readItem)
    : ChildYouthInfoListItemViewModel<ContactEducationDraft, ContactEducation>(draftItem, readItem) { }

public partial class MedicalBehavioralListItemViewModel(
    ContactMedicalBehavioralDraft? draftItem,
    ContactMedicalBehavioral? readItem
) : ChildYouthInfoListItemViewModel<ContactMedicalBehavioralDraft, ContactMedicalBehavioral>(draftItem, readItem) { }

public partial class LanguagesListItemViewModel(ContactLanguageDraft? draftItem, ContactLanguage? readItem)
    : ChildYouthInfoListItemViewModel<ContactLanguageDraft, ContactLanguage>(draftItem, readItem) { }
