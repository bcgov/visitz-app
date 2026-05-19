using Realms;
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

    public string DisplayDate { get; }

    public string DisplayName { get; }

    public string FullType { get; }

    public IQueryable<IcmContact> Contacts { get; }

    public bool IsAssigned(string username);

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
    );

    void RaisePropertyChangedEvent(string propertyName);
}
