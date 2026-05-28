using VisitzModel.Extensions;

namespace VisitzModel.Models.People;

public partial class SupportNetworkItem
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

    public string ActiveBinding
    {
        get => IsValid ? Active : string.Empty;
        set
        {
            this.Commit(() => Active = value);
            RaisePropertyChanged(nameof(Active));
        }
    }
}
