using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace VisitzModel.Messaging;

public class CaseloadItemSelectedMessage(
	CaseloadItem value,
	EntitySection? section = null,
	IDraftItem draftItem = null)
	: ValueChangedMessage<CaseloadItem>(value)
{
	public EntitySection Section { get; set; } = section ?? EntitySection.Details;

	public IDraftItem DraftItem { get; set; } = draftItem;
}
