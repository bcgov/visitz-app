using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.SupportNetwork;

public partial class SupportNetworkListViewModel : IcmRecordViewModel
{
    private bool _disposed;

    readonly ObservableRealmQueryMap realmQuery = new();

    [ObservableProperty]
    public partial ObservableCollection<SupportNetworkItemUi> SupportNetworksList { get; set; } = [];

    [ObservableProperty]
    public partial bool ShowEmptyIcon { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;
        realmQuery.Subscribe(
            DataRealm,
            SupportNetworkItem.GetByParentIdType(DataRealm, BusinessObject.Id, BusinessObject.EntityType)
        );
    }

    private void RealmQuery_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        if (e.Type == typeof(SupportNetworkItem))
            UpdateSupportNetworkList(e.Items, e.Changes);
    }

    private void UpdateSupportNetworkList(IRealmCollection<IRealmObject> items, ChangeSet? changes)
    {
        if (changes == null)
        {
            foreach (var item in items)
                SupportNetworksList.Add(new SupportNetworkItemUi((SupportNetworkItem)item));
        }
        else
        {
            foreach (int deleted in changes.DeletedIndices.Reverse())
                SupportNetworksList.RemoveAt(deleted);

            foreach (int inserted in changes.InsertedIndices)
                SupportNetworksList.Insert(inserted, new SupportNetworkItemUi((SupportNetworkItem)items[inserted]));
        }

        ShowEmptyIcon = SupportNetworksList.Count <= 0;
    }

    [RelayCommand]
    public static void SelectedSupportNetworkItem(SupportNetworkItemUi tappedItem)
    {
        tappedItem.IsExpanded = !tappedItem.IsExpanded;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            realmQuery.ItemsChanged -= RealmQuery_ItemsChanged;
            realmQuery.Dispose();
            SupportNetworksList.Clear();
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
