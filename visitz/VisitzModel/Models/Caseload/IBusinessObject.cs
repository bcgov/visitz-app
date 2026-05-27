using System.ComponentModel;
using System.Globalization;
using Realms;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Formats;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;
using VisitzModel.Storage;

namespace VisitzModel.Models.Caseload;

public partial interface IBusinessObject : IRealmObject
{
    public static readonly string DisplayDateFormat = IcmDateFormats.BasicTimestampShort;

    public string Id { get; set; }

    public string FileNumber { get; set; }

    public string GivenNames { get; set; }

    public string LastName { get; set; }

    public string AssignedTo { get; set; }

    public string AssignedToId { get; set; }

    public string DisplayAssignees { get; }

    public EntityType EntityType { get; }

    public EntitySubtype EntitySubtype { get; set; }

    public EntitySubtype EntitySubtypeBinding { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset UpdatedDate { get; set; }

    public string EntitySubtypeInitials { get; }

    public string ServiceOffice { get; set; }

    public BoLocalState? LocalState { get; set; }

    public string DisplayDate => CreatedDateBinding.ToString(DisplayDateFormat, CultureInfo.InvariantCulture);

    public string DisplayName => $"{LastNameBinding}, {GivenNamesBinding}";

    public string FullType => $"{EntitySubtype.GetDisplayString()} {EntityType.GetDisplayString()}";

    public IQueryable<IcmContact> Contacts => GetContacts(Realm);

    public bool IsAssigned(string username)
    {
        return AssignedTo == username;
    }

    /// <summary>
    /// Deletes most of the dependent data for a BusinessObject.
    /// </summary>
    /// <param name="userIgnoredPrefs"></param>
    /// <param name="fromRealm">A Realm reference to delete from or leave null to use the private reference.</param>
    /// <param name="deleteLocalState">true to delete LocalState as well. false to keep it.</param>
    public void DeleteDependentData(
        UserIgnoredContentPrefs userIgnoredPrefs,
        Realm? fromRealm = null,
        bool deleteLocalState = false
    );

    /// <summary>
    /// Deletes a BusinessObject and all its dependent data.
    /// </summary>
    /// <param name="userIgnoredPrefs"></param>
    /// <param name="fromRealm">A Realm reference to delete from or leave null
    /// to use the private reference.</param>
    /// <param name="cascade">Delete all dependent data for this BusinessObject.
    /// Defaults to true.</param>
    /// <param name="deleteLocalState">Delete LocalState for this BusinessObject.
    /// Defaults to true. Independent from cascade.</param>
    public void Delete(
        UserIgnoredContentPrefs userIgnoredPrefs,
        Realm? fromRealm = null,
        bool cascade = true,
        bool deleteLocalState = true
    )
    {
        fromRealm ??= Realm;
        ArgumentNullException.ThrowIfNull(fromRealm);

        if (cascade)
            DeleteDependentData(userIgnoredPrefs, fromRealm, deleteLocalState);

        fromRealm.Remove(this);
    }

    void RaisePropertyChangedEvent(string propertyName);

    string ToIdTypeString()
    {
        return $"{Id}||{(int)EntityType}";
    }

    IcmContact? GetKeyPlayer(Realm? realm = null)
    {
        realm ??= Realm ?? throw new InvalidOperationException("Attached Realm is null");
        return IcmContact.GetKeyPlayerFor(realm, this);
    }

    IQueryable<IcmContact> GetContacts(Realm? realm = null)
    {
        realm ??= Realm;
        ArgumentNullException.ThrowIfNull(realm);
        return IcmContact.GetByParentObject(realm, this);
    }

    public void SubscribePropertyChanged(PropertyChangedEventHandler handler)
    {
        (this as INotifyPropertyChanged)?.PropertyChanged += handler;
    }

    public void UnsubscribePropertyChanged(PropertyChangedEventHandler handler)
    {
        (this as INotifyPropertyChanged)?.PropertyChanged -= handler;
    }

    public bool Equals(IBusinessObject? other)
    {
        return ReferenceEquals(this, other)
            || (other != null && IdBinding == other.IdBinding && EntityType == other.EntityType);
    }

    public int MakeHashCode()
    {
#pragma warning disable SS008 // GetHashCode() refers to mutable or static member
        // Id is not meant to change
        return EntityType.GetHashCode() * IdBinding.GetHashCode();
#pragma warning restore SS008 // GetHashCode() refers to mutable or static member
    }

    void UpsertLocalState(Realm realm, bool? markForDownload = null)
    {
        if (realm.Find<BoLocalState>(ToIdTypeString()) is BoLocalState local)
        {
            if (!local.ShouldDownloadDuringRefresh && markForDownload is bool mark)
                local.ShouldDownloadDuringRefresh = mark;

            LocalState = local;
            realm.Add(local, update: true);
        }
        else
        {
            LocalState = new(this) { ShouldDownloadDuringRefresh = markForDownload ?? false };
            realm.Add(LocalState);
        }
    }

    public static IQueryable<IBusinessObject> GetQueryableByRelaxedIdType(Realm realm, string id, EntityType type)
    {
        return type switch
        {
            EntityType.Case => realm.All<CaseRecord>().Where(rec => rec.Id == id || rec.FileNumber == id),
            EntityType.Incident => realm.All<IncidentRecord>().Where(rec => rec.Id == id || rec.FileNumber == id),
            EntityType.Memo => realm.All<MemoRecord>().Where(rec => rec.Id == id || rec.FileNumber == id),
            EntityType.ServiceRequest => realm
                .All<ServiceRequestRecord>()
                .Where(rec => rec.Id == id || rec.FileNumber == id),
            _ => throw new InvalidOperationException($"'{type}' not supported"),
        };
    }

    public static IBusinessObject? GetByIdType(Realm realm, string id, EntityType type)
    {
        return type switch
        {
            EntityType.Case => realm.Find<CaseRecord>(id),
            EntityType.Incident => realm.Find<IncidentRecord>(id),
            EntityType.Memo => realm.Find<MemoRecord>(id),
            EntityType.ServiceRequest => realm.Find<ServiceRequestRecord>(id),
            _ => throw new InvalidOperationException($"'{type}' not supported"),
        };
    }

    static IEnumerable<TItem> FilterUnsupportedSubtypes<TItem>(IEnumerable<TItem> businessObjects)
        where TItem : IBusinessObject
    {
        return businessObjects;
    }

    static IEnumerable<TItem> GetAllByAssignee<TItem>(Realm realm, string username, bool invert = false)
        where TItem : IBusinessObject
    {
        Func<TItem, bool> predicate = invert
            ? item => item.AssignedTo != username
            : item => item.AssignedTo == username;
        return realm.All<TItem>().Where(predicate);
    }

    static void CascadeDelete<TItem>(
        Realm realm,
        IEnumerable<TItem> unassigned,
        UserIgnoredContentPrefs userIgnoredPrefs
    )
        where TItem : IBusinessObject
    {
        foreach (var item in unassigned)
            item.Delete(userIgnoredPrefs, realm);
    }

    public static async Task SynchronizeAsync<TItem>(
        Realm realm,
        IEnumerable<TItem> incomingItems,
        UserIgnoredContentPrefs userIgnoredPrefs,
        string currentUsername,
        bool isPersonalCaseload
    )
        where TItem : IBusinessObject
    {
        bool isOfficeCaseload = !isPersonalCaseload;

        var filteredUpsertItems = FilterUnsupportedSubtypes(incomingItems);
        var currentAssigned = GetAllByAssignee<TItem>(realm, currentUsername, isOfficeCaseload).ToList();
        var unassigned = currentAssigned.Except(filteredUpsertItems);

        await realm.CommitAsync(() =>
        {
            CascadeDelete(realm, unassigned, userIgnoredPrefs);
            foreach (var item in filteredUpsertItems)
            {
                realm.Add(item, update: true);
                item.UpsertLocalState(realm, markForDownload: isPersonalCaseload);
            }
        });
    }
}
