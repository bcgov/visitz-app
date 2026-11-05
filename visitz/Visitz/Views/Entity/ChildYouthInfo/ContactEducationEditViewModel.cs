using System.ComponentModel;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactEducationEditViewModel : DraftEditViewModel<ContactEducationDraft>
{
    public override Func<ContactEducationDraft, INotifyPropertyChanged?> ObjectToObserveChanges =>
        static draft => draft.ContactEducation;

    public override string DiscardWarningMessage => LocalizedStrings.DiscardContactEducationDraftDescription;

    public override string DiscardedMessage => LocalizedStrings.DiscardedContactEducationDraft;

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
}
