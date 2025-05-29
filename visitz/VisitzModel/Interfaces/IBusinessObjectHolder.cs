using VisitzModel.Models.Caseload;

namespace VisitzModel.Interfaces;

public interface IBusinessObjectHolder
{
    public IBusinessObject BusinessObject { get; set; }
}
