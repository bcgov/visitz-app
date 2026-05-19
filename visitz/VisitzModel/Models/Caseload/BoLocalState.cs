using Realms;

namespace VisitzModel.Models.Caseload;

/// <summary>
/// Tracks local state for business objects. Info stored here shouldn't have
/// any meaning outside of this app.
/// </summary>
public partial class BoLocalState : IRealmObject
{
    [PrimaryKey]
    public string IdType { get; set; } = Guid.NewGuid().ToString();

    public bool ShouldDownloadDuringRefresh { get; set; } = false;

    public DateTimeOffset LastOpened { get; set; } = DateTimeOffset.UtcNow;

    BoLocalState() { }

    public BoLocalState(IBusinessObject businessObject)
    {
        IdType = businessObject.ToIdTypeString();
    }
}
