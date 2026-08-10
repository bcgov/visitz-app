using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
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

    readonly ObservableCollection<IncidentConcerns> _queriedConcerns = [];

    [ObservableProperty]
    public partial ObservableCollection<ConcernListItemViewModel> Concerns { get; set; } = [];

    [ObservableProperty]
    public partial bool ShowConcerns { get; set; }

    [ObservableProperty]
    public partial bool HasCallInfo { get; set; }

    partial void OnCallInfoChanged(CallInformation value)
    {
        HasCallInfo = value.InformationBinding.Trim().Length > 0;
    }

    [ObservableProperty]
    public partial bool HasAdditionalInfo { get; set; }

    partial void OnAdditionalInfoChanged(AdditionalInformation value)
    {
        HasAdditionalInfo = value.AdditionalInformationsBinding.Trim().Length > 0;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        _queriedConcerns.CollectionChanged += QueriedConcerns_CollectionChanged;
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
            _queriedConcerns.CollectionChanged -= QueriedConcerns_CollectionChanged;
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
            if (e.Changes == null)
                _queriedConcerns.AddAll(e.Items.Cast<IncidentConcerns>());
            else
            {
                foreach (var delete in e.Changes.DeletedIndices.Reverse())
                    _queriedConcerns.RemoveAt(delete);

                foreach (var insert in e.Changes.InsertedIndices)
                    _queriedConcerns.Add((IncidentConcerns)e.Items[insert]);
            }
        }
    }

    private void QueriedConcerns_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (var newItem in e.NewItems.Cast<IncidentConcerns>())
                Concerns.InsertSorted(new() { Concerns = newItem });
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (var removeItem in e.OldItems.Cast<IncidentConcerns>())
                if (Concerns.FirstOrDefault(vm => vm.Concerns.Id == removeItem.Id) is ConcernListItemViewModel found)
                    Concerns.Remove(found);
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
