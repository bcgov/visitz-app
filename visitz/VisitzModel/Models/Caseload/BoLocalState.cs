using Realms;

namespace VisitzModel.Models.Caseload;

#nullable enable

/// <summary>
/// Tracks local state for business objects. Info stored here shouldn't have
/// any meaning outside of this app.
/// </summary>
public partial class BoLocalState : IRealmObject
{
    [PrimaryKey]
    public string IdType { get; set; } = Guid.NewGuid().ToString();

    public CaseRecord? Case { get; set; }

    public IncidentRecord? Incident { get; set; }

    public MemoRecord? Memo { get; set; }

    public ServiceRequestRecord? ServiceRequest { get; set; }

    public bool ShouldDownloadDuringRefresh { get; set; } = false;

    public BoLocalState() { }

    public BoLocalState(IBusinessObject businessObject)
    {
        SetBusinessObject(businessObject);
    }

    public void SetBusinessObject(IBusinessObject businessObject)
    {
        IdType = businessObject.ToIdTypeString();

        if (businessObject is CaseRecord @case)
            Case = @case;
        else if (businessObject is IncidentRecord incident)
            Incident = incident;
        else if (businessObject is MemoRecord memo)
            Memo = memo;
        else if (businessObject is ServiceRequestRecord serviceRequest)
            ServiceRequest = serviceRequest;
        else
            throw new InvalidOperationException($"'{businessObject.GetType()}' not supported");
    }

    public BoLocalState ShallowCopy()
    {
        return new()
        {
            ShouldDownloadDuringRefresh = ShouldDownloadDuringRefresh,
        };
    }
}
