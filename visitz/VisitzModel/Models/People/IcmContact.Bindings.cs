using VisitzModel.Extensions;

namespace VisitzModel.Models.People;

public partial class IcmContact
{
    public string PrimaryAddressBinding
    {
        get => IsValid ? PrimaryAddress : string.Empty;
        set
        {
            this.Commit(() => PrimaryAddress = value);
            RaisePropertyChanged(nameof(PrimaryAddress));
        }
    }

    public string HomePhoneBinding
    {
        get => IsValid ? HomePhone : string.Empty;
        set
        {
            this.Commit(() => HomePhone = value);
            RaisePropertyChanged(nameof(HomePhone));
        }
    }

    public string CellPhoneBinding
    {
        get => IsValid ? CellPhone : string.Empty;
        set
        {
            this.Commit(() => CellPhone = value);
            RaisePropertyChanged(nameof(CellPhone));
        }
    }
}
