using Realms;
using VisitzModel.Models.Interfaces;

namespace VisitzModel.Models.Drafts;

public interface IDraftItem : IRealmObject, IRecordInfo
{
	DateTimeOffset DraftCreated { get; set; }

	DateTimeOffset LastUpdated { get; set; }

	string Preview { get; }

	string DraftLocation { get; set; }	
}

public static class IDraftItemExtensions
{
	public static IDraftItem InitDraftWith(this IDraftItem item, CaseloadItem caseloadItem)
	{
		item.DraftLocation = caseloadItem.DisplayName;
		(item as IRecordInfo).InitWith(caseloadItem);
		return item;
	}
}
