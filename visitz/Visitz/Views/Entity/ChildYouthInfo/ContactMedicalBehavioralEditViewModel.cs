using System.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactMedicalBehavioralEditViewModel : DraftEditViewModel<ContactMedicalBehavioralDraft>
{
    public override Func<ContactMedicalBehavioralDraft, INotifyPropertyChanged?> ObjectToObserveChanges =>
        static draft => draft.ContactMedicalBehavioral;

    public override string DiscardWarningMessage => throw new NotImplementedException();

    public override string DiscardedMessage => throw new NotImplementedException();

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
