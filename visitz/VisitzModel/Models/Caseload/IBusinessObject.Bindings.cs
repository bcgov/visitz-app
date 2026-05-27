using VisitzModel.Extensions;

namespace VisitzModel.Models.Caseload;

public partial interface IBusinessObject
{
    public string IdBinding
    {
        get => IsValid ? Id : string.Empty;
        set
        {
            if (!IsValid)
                return;

            this.Commit(() => Id = value);
            RaisePropertyChangedEvent(nameof(Id));
        }
    }

    public DateTimeOffset CreatedDateBinding
    {
        get => IsValid ? CreatedDate : DateTimeOffset.MinValue;
        set
        {
            if (IsValid)
            {
                this.Commit(() => CreatedDate = value);
                RaisePropertyChangedEvent(nameof(CreatedDate));
            }
        }
    }

    public string FileNumberBinding
    {
        get => IsValid ? FileNumber : string.Empty;
        set
        {
            this.Commit(() => FileNumber = value);
            RaisePropertyChangedEvent(nameof(FileNumber));
        }
    }

    public string GivenNamesBinding
    {
        get => IsValid ? GivenNames : string.Empty;
        set
        {
            this.Commit(() => GivenNames = value);
            RaisePropertyChangedEvent(nameof(GivenNames));
        }
    }

    public string LastNameBinding
    {
        get => IsValid ? LastName : string.Empty;
        set
        {
            this.Commit(() => LastName = value);
            RaisePropertyChangedEvent(nameof(LastName));
        }
    }

    public string ServiceOfficeBinding
    {
        get => IsValid ? ServiceOffice : string.Empty;
        set
        {
            this.Commit(() => ServiceOffice = value);
            RaisePropertyChangedEvent(nameof(ServiceOffice));
        }
    }
}
