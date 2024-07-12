using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models;

public interface IRecordInfo
{
	string RelatedEntityId { get; set; }

	EntityType RelatedEntityType { get; set; }

	EntitySubtype RelatedEntitySubtype { get; set; }
}

public static class IRecordInfoExtensions
{
	public static IRecordInfo InitWith(this IRecordInfo item, CaseloadItem caseloadItem)
	{
		item.RelatedEntityId = caseloadItem.CaseIncidentNumber;
		item.RelatedEntityType = caseloadItem.EntityType.ParseEntityType();
		item.RelatedEntitySubtype = caseloadItem.CaseIncidentType.ParseEntitySubtype();

		return item;
	}
}
