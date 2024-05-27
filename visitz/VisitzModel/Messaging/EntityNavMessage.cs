using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;

namespace VisitzModel.Messaging;

public class EntityNavMessage(EntityNavItem navItem, CaseloadItem caseloadItem)
    : ValueChangedMessage<(EntityNavItem, CaseloadItem)>((navItem, caseloadItem))
{
}
