using VisitzModel.Extensions;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.InPersonVisits;

public partial class PersonVisit
{
    public string VisitDescriptionBinding
    {
        get => IsValid ? VisitDescription : default;
        set
        {
            this.Commit(() => VisitDescription = value);
            RaisePropertyChanged(nameof(VisitDescription));
        }
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

    public string VisitDetailsValueBinding
    {
        get => IsValid ? VisitDetailsValue : default;
        set
        {
            this.Commit(() => VisitDetailsValue = value);
            RaisePropertyChanged(nameof(VisitDetailsValue));
        }
    }

    public string VisitDetailsGroupBinding
    {
        get => IsValid ? VisitDetailsGroup : default;
        set
        {
            this.Commit(() => VisitDetailsGroup = value);
            RaisePropertyChanged(nameof(VisitDetailsGroup));
        }
    }
}
