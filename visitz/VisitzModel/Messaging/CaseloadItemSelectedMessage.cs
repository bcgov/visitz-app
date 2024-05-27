using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Messaging;

public class CaseloadItemSelectedMessage(CaseloadItem value, EntitySection? section = null)
	: ValueChangedMessage<CaseloadItem>(value)
{
	public EntitySection Section { get; set; } = section ?? EntitySection.Details;
}
