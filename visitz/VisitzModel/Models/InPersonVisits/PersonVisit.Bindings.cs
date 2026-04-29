using VisitzModel.Extensions;

namespace VisitzModel.Models.InPersonVisits;

public partial class PersonVisit
{
    public string VisitDescriptionBinding
    {
        get => IsValid ? VisitDescription : string.Empty;
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
}
