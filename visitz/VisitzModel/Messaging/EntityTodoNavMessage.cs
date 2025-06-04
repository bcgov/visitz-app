using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace VisitzModel.Messaging;

public class EntityTodoNavMessage(
    EntityNavItem navItem,
    CaseloadItem caseloadItem,
    EntitySection? subsection = null,
    PersonVisit selectedVisitItem = null)
    : ValueChangedMessage<(EntityNavItem, CaseloadItem, EntitySection?, PersonVisit)>(
        (navItem, caseloadItem, subsection, selectedVisitItem))
{
}
