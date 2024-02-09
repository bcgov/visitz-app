using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models;

namespace VisitzModel.Messaging;

public class EntityNavMessage(NavItem navItem, CaseloadItem caseloadItem)
    : ValueChangedMessage<(NavItem, CaseloadItem)>((navItem, caseloadItem))
{
}
