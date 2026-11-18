using System.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactMedicalBehavioralEditViewModel : DraftEditViewModel<ContactMedicalBehavioralDraft>
{
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


    protected override Task DoPublishDraftAsync()
    {
        // TODO: wire up publishing
        return Task.CompletedTask;
    }

    protected override bool EnablePublish(ContactMedicalBehavioralDraft draft)
    {
        // TODO: wire up validation for publishing
        return false;
    }
}
