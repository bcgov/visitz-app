using VisitzModel.Extensions;

namespace VisitzModel.Models.CallDetails;

public partial class IncidentConcerns
{
    public string ConcernBinding
    {
        get => IsValid ? Concern : string.Empty;
        set
        {
            this.Commit(() => Concern = value);
            RaisePropertyChanged(nameof(Concern));
        }
    }

    public DateTimeOffset? StartDateBinding
    {
        get => IsValid ? StartDate : default;
        set
        {
            this.Commit(() => StartDate = value);
            RaisePropertyChanged(nameof(StartDate));
        }
    }

    public DateTimeOffset? EndDateBinding
    {
        get => IsValid ? EndDate : default;
        set
        {
            this.Commit(() => EndDate = value);
            RaisePropertyChanged(nameof(EndDate));
        }
    }

    public DateTimeOffset CreatedBinding
    {
        get => IsValid ? Created : DateTimeOffset.MinValue;
        set
        {
            this.Commit(() => Created = value);
            RaisePropertyChanged(nameof(Created));
        }
    }
}
