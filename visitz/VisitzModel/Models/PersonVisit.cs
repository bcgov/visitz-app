using Realms;

namespace VisitzModel.Models;

public partial class PersonVisit : IRealmObject
{
    static readonly string _defaultType = "In Person Child Youth";

    [PrimaryKey]
    public string Id { get; set; }

    public string ParentId { get; set; }

    public string Name { get; set; }

    public string VisitDescription { get; set; }

    public string Type { get; set; } = _defaultType;

    public DateTimeOffset DateOfVisit { get; set; }

    public string VisitDetailsValue { get; set; }

    public string LoginName { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset Updated { get; set; }

    public string CreatedBy { get; set; }

    public string UpdatedBy { get; set; }
}
