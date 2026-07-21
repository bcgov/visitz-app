using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.CallDetails;

namespace Visitz.Views.Entity.CallDetails;

#nullable enable

public partial class CallDetailsViewModel : IcmRecordViewModel
{
    bool _disposed;

    readonly ObservableRealmQueryMap _realmQueryMap = new();

    [ObservableProperty]
    public partial CallInformation CallInfo { get; set; } = new();

    [ObservableProperty]
    public partial AdditionalInformation AdditionalInfo { get; set; } = new();

    [ObservableProperty]
    public partial IEnumerable<IncidentConcerns> Concerns { get; set; } = [];

    [ObservableProperty]
    public partial bool ShowConcerns { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        _realmQueryMap.ItemsChanged += RealmQueryMap_ItemsChanged;

        _realmQueryMap.Subscribe(DataRealm, CallInformation.GetByParent(DataRealm, EntityType, RowId));
        _realmQueryMap.Subscribe(DataRealm, AdditionalInformation.GetByParent(DataRealm, EntityType, RowId));

        if (EntityType == VisitzModel.Models.EntityTypes.EntityType.Incident)
        {
            _realmQueryMap.Subscribe(DataRealm, IncidentConcerns.GetByParent(DataRealm, RowId));
            ShowConcerns = true;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _realmQueryMap.ItemsChanged -= RealmQueryMap_ItemsChanged;
            _realmQueryMap.Dispose();

            CallInfo = new();
            AdditionalInfo = new();
            Concerns = [];

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void RealmQueryMap_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        if (e.Type == typeof(CallInformation))
        {
            if (e.Changes == null && e.Items.Count > 0)
                CallInfo = (CallInformation)e.Items[0];
        }
        else if (e.Type == typeof(AdditionalInformation))
        {
            if (e.Changes == null && e.Items.Count > 0)
                AdditionalInfo = (AdditionalInformation)e.Items[0];
        }
        else if (e.Type == typeof(IncidentConcerns))
        {
            Concerns = e.Items.Cast<IncidentConcerns>();
        }
    }

    [RelayCommand]
    public static async Task OpenFullInfoDetails(string text)
    {
        FullDetailsView full = ServiceProvider.GetService<FullDetailsView>();
        full.Text = text;
        await Navigator.Navigation.PushModalAsync(full.WrapPageForModal());
    }
}
