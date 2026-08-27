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

    public int AgeBinding
    {
        get => IsValid ? Age : int.MinValue;
        set
        {
            this.Commit(() => Age = value);
            RaisePropertyChanged(nameof(Age));
        }
    }

    public string IndigenousBinding
    {
        get => IsValid ? Indigenous : string.Empty;
        set
        {
            this.Commit(() => Indigenous = value);
            RaisePropertyChanged(nameof(Indigenous));
        }
    }

    public string CoordinationAgtCaBinding
    {
        get => IsValid ? CoordinationAgtCa : string.Empty;
        set
        {
            this.Commit(() => CoordinationAgtCa = value);
            RaisePropertyChanged(nameof(CoordinationAgtCa));
        }
    }

    public string _921AgtBinding
    {
        get => IsValid ? _921Agt : string.Empty;
        set
        {
            this.Commit(() => _921Agt = value);
            RaisePropertyChanged(nameof(_921Agt));
        }
    }

    public string DeceasedBinding
    {
        get => IsValid ? Deceased : string.Empty;
        set
        {
            this.Commit(() => Deceased = value);
            RaisePropertyChanged(nameof(Deceased));
        }
    }

    public string RelationshipBinding
    {
        get => IsValid ? Relationship : string.Empty;
        set
        {
            this.Commit(() => Relationship = value);
            RaisePropertyChanged(nameof(Relationship));
        }
    }
}
