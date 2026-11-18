using System.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactLanguageEditViewModel : DraftEditViewModel<ContactLanguageDraft>
{
    public override Func<ContactLanguageDraft, INotifyPropertyChanged?> ObjectToObserveChanges =>
        static draft => draft.ContactLanguage;

    public override string DiscardWarningMessage =>
        string.Format(
            LocalizedStrings.DiscardThisDraftDescription,
            LocalizedStrings.ContactLanguages.ToLowerInvariant()
        );

    public override string DiscardedMessage =>
        string.Format(LocalizedStrings.DiscardedThisDraft, LocalizedStrings.ContactLanguages.ToLowerInvariant());


    protected override Task DoPublishDraftAsync()
    {
        // TODO: wire up publishing
        return Task.CompletedTask;
    }

    protected override bool EnablePublish(ContactLanguageDraft draft)
    {
        // TODO: wire up validation for publishing
        return false;
    }
}
