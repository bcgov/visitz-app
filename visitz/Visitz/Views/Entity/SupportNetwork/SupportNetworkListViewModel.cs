using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.SupportNetwork;

internal partial class SupportNetworkListViewModel : VisitzViewModel, ICaseloadItemHolder, IRequestedEntitySection
{
    private bool _disposed;

    readonly ObservableRealmQueryMap realmQuery = new();
    
    public EntitySection RequestedSection { get; set; }

    [ObservableProperty]
    public ObservableCollection<SupportNetworkItemUi> supportNetworksList = [];

    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public string name;

    [ObservableProperty]
    public string relationship;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        Realm icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;

        realmQuery.Subscribe(icmDataRealm, SupportNetworkItem.GetSupportNetworkByCaseId(icmDataRealm, CaseloadItem.RowId));
    }

    private void RealmQuery_ItemsChanged(object sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
    {
        if (e.Type == typeof(SupportNetworkItem))
            UpdateSupportNetworkList(e.Items, e.Changes);
    }

    private void UpdateSupportNetworkList(IRealmCollection<IRealmObject> items, ChangeSet changes)
    {
        if (changes == null)
        {
            foreach (var item in items)
                SupportNetworksList.Add(new SupportNetworkItemUi(item as SupportNetworkItem));
        }
        else
        {
            foreach (int deleted in changes.DeletedIndices)
                SupportNetworksList.RemoveAt(deleted);

            foreach (int inserted in changes.InsertedIndices)
                SupportNetworksList.Insert(inserted, new SupportNetworkItemUi(items[inserted] as SupportNetworkItem));
        }
    }

    [RelayCommand]
     public void SelectedSupportNetworkItem(SupportNetworkItemUi tappedItem)
    {
        // Toggle the visibility of the tapped item
        tappedItem.IsExpanded = !tappedItem.IsExpanded;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            realmQuery.ItemsChanged -= RealmQuery_ItemsChanged;
            realmQuery.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
