using VisitzModel.Extensions;

namespace VisitzModel.Models.InPersonVisits;

public partial class PersonVisit
{
    public string IdBinding
    {
        get => IsValid ? Id : string.Empty;
        set
        {
            this.Commit(() => Id = value);
            RaisePropertyChanged(nameof(Id));
        }
    }

    public string ParentIdBinding
    {
        get => IsValid ? ParentId : string.Empty;
        set
        {
            this.Commit(() => ParentId = value);
            RaisePropertyChanged(nameof(ParentId));
        }
    }

    public string VisitDescriptionBinding
    {
        get => IsValid ? VisitDescription : string.Empty;
        set
        {
            this.Commit(() => VisitDescription = value);
            RaisePropertyChanged(nameof(VisitDescription));
        }
    }

    public string CreatedByBinding
    {
        get => IsValid ? CreatedBy : string.Empty;
        set
        {
            this.Commit(() => CreatedBy = value);
            RaisePropertyChanged(nameof(CreatedBy));
        }
    }

    public IList<string> VisitDetailsBinding
    {
        get => IsValid ? VisitDetails : [];
    }

    public DateTimeOffset DateOfVisitBinding
    {
        get => IsValid ? DateOfVisit : default;
        set
        {
            this.Commit(() => DateOfVisit = value);
            RaisePropertyChanged(nameof(DateOfVisit));
        }
    }
}
