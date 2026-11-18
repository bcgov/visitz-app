using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.ListOfValues;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactLanguageEditViewModel : DraftEditViewModel<ContactLanguageDraft>
{
    bool _disposed;

    public override Func<ContactLanguageDraft, INotifyPropertyChanged?> ObjectToObserveChanges =>
        static draft => draft.ContactLanguage;

    public override string DiscardWarningMessage =>
        string.Format(
            LocalizedStrings.DiscardThisDraftDescription,
            LocalizedStrings.ContactLanguages.ToLowerInvariant()
        );

    public override string DiscardedMessage =>
        string.Format(LocalizedStrings.DiscardedThisDraft, LocalizedStrings.ContactLanguages.ToLowerInvariant());

    [ObservableProperty]
    public partial ObservableQuery<ListedValue>? LanguageNamesQuery { get; set; }

    [ObservableProperty]
    public partial ObservableQuery<ListedValue>? TypesQuery { get; set; }

    [ObservableProperty]
    public partial ListedValue? SelectedLanguageName { get; set; }

    [ObservableProperty]
    public partial ListedValue? SelectedType { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        LanguageNamesQuery = new(DataRealm, ListedValue.GetAllByType(DataRealm, ContactLanguage.LanguageNamesLovKey));
        TypesQuery = new(DataRealm, ListedValue.GetAllByType(DataRealm, ContactLanguage.TypeLovKey));

        await LanguageNamesQuery.Loaded;
        SelectedLanguageName = LanguageNamesQuery.Items.GetListedValue(Draft?.ContactLanguage?.LanguageName);

        await TypesQuery.Loaded;
        SelectedType = TypesQuery.Items.GetListedValue(Draft?.ContactLanguage?.Type);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            LanguageNamesQuery?.Dispose();
            TypesQuery?.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }

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

    partial void OnSelectedLanguageNameChanged(ListedValue? value)
    {
        if (IsInitialized && value?.Value is string name)
            Draft?.ContactLanguage?.LanguageNameBinding = name;
    }

    partial void OnSelectedTypeChanged(ListedValue? value)
    {
        if (IsInitialized && value?.Value is string type)
            Draft?.ContactLanguage?.TypeBinding = type;
    }
}
