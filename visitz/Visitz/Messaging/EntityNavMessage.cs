using CommunityToolkit.Mvvm.Messaging.Messages;
using Visitz.Models;
using VisitzModel.Models;

namespace Visitz.Messaging;

public class EntityNavMessage(NavItem navItem, CaseloadItem caseloadItem) 
    : ValueChangedMessage<(NavItem, CaseloadItem)>((navItem, caseloadItem))
{
}
