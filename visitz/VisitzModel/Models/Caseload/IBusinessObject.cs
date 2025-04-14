using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.Caseload;

public interface IBusinessObject
{
    public string FileNumber { get; set; }

    public string GivenNames { get; set; }

    public string LastName { get; set; }

    public EntityType EntityType { get; }
}
