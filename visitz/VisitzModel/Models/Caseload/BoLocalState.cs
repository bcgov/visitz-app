using Realms;
using VisitzModel.Extensions;

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

    public bool ShouldDownloadDuringRefresh { get; set; } = false;

    public bool ShouldDownloadDuringRefreshBinding
    {
        get => IsValid && ShouldDownloadDuringRefresh;
        set
        {
            this.Commit(() => ShouldDownloadDuringRefresh = value);
            RaisePropertyChanged(nameof(ShouldDownloadDuringRefresh));
        }
    }

    BoLocalState() { }

    public BoLocalState(IBusinessObject businessObject)
    {
        IdType = businessObject.ToIdTypeString();
    }
}
