using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.Interfaces;

public interface IRecordInfo
{
    string RelatedEntityId { get; set; }

    EntityType RelatedEntityType { get; set; }

    EntitySubtype RelatedEntitySubtype { get; set; }
}

public static class IRecordInfoExtensions
{
    public static IRecordInfo InitWith(this IRecordInfo item, IBusinessObject businessObject)
    {
        item.RelatedEntityId = businessObject.FileNumber;
        item.RelatedEntityType = businessObject.EntityType;
        item.RelatedEntitySubtype = businessObject.EntitySubtype;

        return item;
    }
}
