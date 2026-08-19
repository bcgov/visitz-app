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

    public string LastNameBinding
    {
        get => IsValid ? LastName : string.Empty;
        set
        {
            this.Commit(() => LastName = value);
            RaisePropertyChanged(nameof(LastName));
        }
    }

    public string FirstNameBinding
    {
        get => IsValid ? FirstName : string.Empty;
        set
        {
            this.Commit(() => FirstName = value);
            RaisePropertyChanged(nameof(FirstName));
        }
    }

    public string GenderBinding
    {
        get => IsValid ? Gender : string.Empty;
        set
        {
            this.Commit(() => Gender = value);
            RaisePropertyChanged(nameof(Gender));
        }
    }

    public DateTimeOffset? DateOfBirthBinding
    {
        get => IsValid ? DateOfBirth : null;
        set
        {
            this.Commit(() => DateOfBirth = value);
            RaisePropertyChanged(nameof(DateOfBirth));
        }
    }
}
