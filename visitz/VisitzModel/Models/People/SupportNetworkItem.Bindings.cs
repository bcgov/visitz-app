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

    public string NameBinding
    {
        get => IsValid ? Name : string.Empty;
        set
        {
            this.Commit(() => Name = value);
            RaisePropertyChanged(nameof(Name));
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

    public string AgencyNameBinding
    {
        get => IsValid ? AgencyName : string.Empty;
        set
        {
            this.Commit(() => AgencyName = value);
            RaisePropertyChanged(nameof(AgencyName));
        }
    }

    public string AddressBinding
    {
        get => IsValid ? Address : string.Empty;
        set
        {
            this.Commit(() => Address = value);
            RaisePropertyChanged(nameof(Address));
        }
    }

    public string PhoneNumberBinding
    {
        get => IsValid ? PhoneNumber : string.Empty;
        set
        {
            this.Commit(() => PhoneNumber = value);
            RaisePropertyChanged(nameof(PhoneNumber));
        }
    }

    public string CellPhoneNumberBinding
    {
        get => IsValid ? CellPhoneNumber : string.Empty;
        set
        {
            this.Commit(() => CellPhoneNumber = value);
            RaisePropertyChanged(nameof(CellPhoneNumber));
        }
    }

    public string CommentsBinding
    {
        get => IsValid ? Comments : string.Empty;
        set
        {
            this.Commit(() => Comments = value);
            RaisePropertyChanged(nameof(Comments));
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
