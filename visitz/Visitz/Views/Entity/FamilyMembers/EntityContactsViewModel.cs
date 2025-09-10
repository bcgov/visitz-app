using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using System.Collections.ObjectModel;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.FamilyMembers;

public partial class EntityContactsViewModel : VisitzViewModel, IBusinessObjectHolder
{
    readonly ObservableRealmQueryMap realmQueryMap = new();

    [ObservableProperty]
    public IBusinessObject businessObject;

    [ObservableProperty]
    public ObservableCollection<IcmContact> contacts = [];

    [ObservableProperty]
    public bool isEmpty;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        var realm = await VisitzRealms.GetIcmDataRealmAsync();

        realmQueryMap.ItemsChanged += RealmQueryMap_ItemsChanged;
        realmQueryMap.Subscribe(realm, IcmContact.GetByParentObject(realm, BusinessObject));
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            realmQueryMap.ItemsChanged -= RealmQueryMap_ItemsChanged;
            realmQueryMap.Dispose();

            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void RealmQueryMap_ItemsChanged(object sender,
        (Type Type,
        IRealmCollection<IRealmObject> Items,
        ChangeSet Changes) e)
    {
        var items = e.Items;
        var changes = e.Changes;

        var comparer = new IcmContactRelationshipComparer();

        if (changes == null)
        {
            var ordered = items.Cast<IcmContact>()
                .ToList()
                .Order(comparer);

            foreach (var contact in ordered)
                Contacts.Add(contact);
        }
        else
        {
            foreach (var removeIndex in changes.DeletedIndices.Reverse())
                Contacts.RemoveAt(removeIndex);

            foreach (var insertIndex in changes.InsertedIndices)
            {
                var contact = (IcmContact)items.ElementAt(insertIndex);

                int index = Contacts.BinarySearch(contact, comparer);
                if (index < 0) index = ~index;

                Contacts.Insert(index, contact);
            }
        }

        IsEmpty = !Contacts.Any();
    }
}
