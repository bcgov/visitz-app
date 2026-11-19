using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Visitz.Controls.ActionButton;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Drafts;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ChildYouthInfoListViewModel : IcmRecordViewModel, IOpenFocusDraftItem
{
    bool _disposed;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial ObservableQuery<ContactEducation>? Education { get; private set; }

    [ObservableProperty]
    public partial ObservableQuery<ContactMedicalBehavioral>? MedicalBehavioral { get; private set; }

    [ObservableProperty]
    public partial ObservableQuery<ContactLanguage>? Languages { get; private set; }

    [ObservableProperty]
    public partial List<ActionButtonOption> FabOptions { get; private set; } = [];

    public IDraftItem? FocusedDraftItem { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        IsLoading = true;

        FabOptions =
        [
            new ActionButtonOption()
            {
                Text = LocalizedStrings.Education,
                Command = OpenEducationDraftCommand,
                Icon = MaterialIcons.School.GetUnfilledMaterialIcon(),
            },
            new ActionButtonOption()
            {
                Text = LocalizedStrings.MedicalBehavioural,
                Command = OpenMedicalBehavioralDraftCommand,
                Icon = MaterialIcons.Monitor_heart.GetUnfilledMaterialIcon(),
            },
            new ActionButtonOption()
            {
                Text = LocalizedStrings.Languages,
                Command = OpenLanguageDraftCommand,
                Icon = MaterialIcons.Globe.GetUnfilledMaterialIcon(),
            },
        ];

        if (BusinessObject.GetKeyPlayer() is IcmContact keyPlayer)
        {
            var dataRealm = await VisitzRealms.GetIcmDataRealmAsync();

            Education = new(dataRealm, query: ContactEducation.GetAllByParent(dataRealm, keyPlayer.Id));

            MedicalBehavioral = new(dataRealm, query: ContactMedicalBehavioral.GetAllByParent(dataRealm, keyPlayer.Id));

            Languages = new(dataRealm, query: ContactLanguage.GetAllByParent(dataRealm, keyPlayer.Id));

            await Task.WhenAll(Education.Loaded, MedicalBehavioral.Loaded, Languages.Loaded);
        }
        else
        {
            string error = $"Unable to find key player for {BusinessObject.DisplayName}";
            Logger.LogError(error);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(error);
        }

        IsLoading = false;

        await OpenDraftAsync();
    }

    [RelayCommand]
    public async Task OpenEducationDraft(ContactEducationDraft? draft)
    {
        ContactEducationEditView view = ServiceProvider.GetService<ContactEducationEditView>();
        view.Init(RowId, EntityType, draft);

        await Navigator.Navigation.PushModalAsync(view.WrapPageForModal());
    }

    [RelayCommand]
    public async Task OpenMedicalBehavioralDraft(ContactMedicalBehavioralDraft? draft)
    {
        ContactMedicalBehavioralEditView view = ServiceProvider.GetService<ContactMedicalBehavioralEditView>();
        view.Init(RowId, EntityType, draft);

        await Navigator.Navigation.PushModalAsync(view.WrapPageForModal());
    }

    [RelayCommand]
    public async Task OpenLanguageDraft(ContactLanguageDraft? draft)
    {
        ContactLanguageEditView view = ServiceProvider.GetService<ContactLanguageEditView>();
        view.Init(RowId, EntityType, draft);

        await Navigator.Navigation.PushModalAsync(view.WrapPageForModal());
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            Education?.Dispose();
            MedicalBehavioral?.Dispose();
            Languages?.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    public async Task OpenDraftAsync()
    {
        if (FocusedDraftItem is ContactEducationDraft education)
            await OpenEducationDraft(education);
        else if (FocusedDraftItem is ContactMedicalBehavioralDraft medical)
            await OpenMedicalBehavioralDraft(medical);
        else if (FocusedDraftItem is ContactLanguageDraft language)
            await OpenLanguageDraft(language);
    }
}
