using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Visitz.Extensions;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
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

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        IsLoading = true;

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
