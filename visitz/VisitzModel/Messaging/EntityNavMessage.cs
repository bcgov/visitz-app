using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace VisitzModel.Messaging;

public class EntityNavMessage(
	EntityNavItem navItem,
	CaseloadItem caseloadItem,
	EntitySection? subsection = null,
	IDraftItem selectedDraftItem = null)
    : ValueChangedMessage<(EntityNavItem, CaseloadItem, EntitySection?, IDraftItem)>(
		(navItem, caseloadItem, subsection, selectedDraftItem))
{
}
