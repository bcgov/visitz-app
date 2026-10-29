using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Visitz.Controls.ActionButton;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Models;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ChildYouthInfoListViewModel : IcmRecordViewModel
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

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        IsLoading = true;

        FabOptions =
        [
            new ActionButtonOption()
            {
                Text = LocalizedStrings.Education,
                Command = new Command(AddEducationDraft),
                Icon = MaterialIcons.School.GetUnfilledMaterialIcon(),
            },
            new ActionButtonOption()
            {
                Text = LocalizedStrings.MedicalBehavioural,
                Command = new Command(AddMedicalBehavioralDraft),
                Icon = MaterialIcons.Monitor_heart.GetUnfilledMaterialIcon(),
            },
            new ActionButtonOption()
            {
                Text = LocalizedStrings.Languages,
                Command = new Command(AddLanguageDraft),
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
    }

    [RelayCommand]
    public void AddEducationDraft()
    {
        SnackbarHandler.ShowText("AddEducationDraft");
    }

    [RelayCommand]
    public void AddMedicalBehavioralDraft()
    {
        SnackbarHandler.ShowText("AddMedicalBehavioralDraft");
    }

    [RelayCommand]
    public void AddLanguageDraft()
    {
        SnackbarHandler.ShowText("AddLanguageDraft");
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
}
