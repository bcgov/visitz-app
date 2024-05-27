using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;

namespace VisitzModel.Messaging;

public class EntityNavMessage(EntityNavItem navItem, CaseloadItem caseloadItem, EntitySection? subsection = null)
    : ValueChangedMessage<(EntityNavItem, CaseloadItem, EntitySection?)>((navItem, caseloadItem, subsection))
{
}
