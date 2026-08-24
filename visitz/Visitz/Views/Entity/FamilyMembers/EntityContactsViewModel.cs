using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Models;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.FamilyMembers;

public partial class EntityContactsViewModel : IcmRecordViewModel
{
    readonly ObservableRealmQueryMap _realmQueryMap = new();

    static readonly IcmContactRelationshipComparer _contactComparer = new();

    readonly IComparer<ContactItemViewModel> _itemComparer = Comparer<ContactItemViewModel>.Create(
        (l, r) => _contactComparer.Compare(l.Contact, r.Contact)
    );

    [ObservableProperty]
    public partial ObservableCollection<IcmContact> Contacts { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<ContactItemViewModel> ContactViewModels { get; set; } = [];

    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        Contacts.CollectionChanged += Contacts_CollectionChanged;
        _realmQueryMap.ItemsChanged += RealmQueryMap_ItemsChanged;
        _realmQueryMap.Subscribe(DataRealm, IcmContact.GetByParentIdType(DataRealm, RowId, EntityType));
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            Contacts.CollectionChanged -= Contacts_CollectionChanged;
            _realmQueryMap.ItemsChanged -= RealmQueryMap_ItemsChanged;
            _realmQueryMap.Dispose();
            Contacts.Clear();
            ContactViewModels.Clear();

            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void RealmQueryMap_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        if (e.Changes == null)
        {
            Contacts.AddAll(e.Items.Cast<IcmContact>());
        }
        else
        {
            foreach (var removeIndex in e.Changes.DeletedIndices.Reverse())
                Contacts.RemoveAt(removeIndex);

            foreach (var insertIndex in e.Changes.InsertedIndices)
                Contacts.Add((IcmContact)e.Items[insertIndex]);
        }

        IsEmpty = !ContactViewModels.Any();
    }

    private void Contacts_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (var item in e.NewItems.Cast<IcmContact>())
                ContactViewModels.InsertSorted(new(item), _itemComparer);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (var item in e.OldItems.Cast<IcmContact>())
                if (ContactViewModels.FirstOrDefault(vm => vm.Contact == item) is ContactItemViewModel found)
                    ContactViewModels.Remove(found);
        }
    }
}
