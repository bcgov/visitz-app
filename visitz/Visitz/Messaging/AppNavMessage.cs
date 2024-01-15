using CommunityToolkit.Mvvm.Messaging.Messages;
using Visitz.Models;

namespace Visitz.Messaging;

internal class AppNavMessage(NavItem value) : ValueChangedMessage<NavItem>(value)
{
}
