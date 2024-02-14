using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models;

namespace VisitzModel.Messaging;

public class AppNavMessage(NavItem value) : ValueChangedMessage<NavItem>(value)
{
}
