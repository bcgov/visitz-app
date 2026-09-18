using CommunityToolkit.Mvvm.ComponentModel;
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

        var dataRealm = await VisitzRealms.GetIcmDataRealmAsync();

        Education = new(dataRealm, query: dataRealm.All<ContactEducation>());

        MedicalBehavioral = new(dataRealm, query: dataRealm.All<ContactMedicalBehavioral>());

        Languages = new(dataRealm, query: dataRealm.All<ContactLanguage>());

        await Task.WhenAll(Education.Loaded, MedicalBehavioral.Loaded, Languages.Loaded);
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
