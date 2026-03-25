using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models.Navigation;

namespace VisitzModel.Messaging;

public class AppNavMessage(NavItem value) : ValueChangedMessage<NavItem>(value) { }
