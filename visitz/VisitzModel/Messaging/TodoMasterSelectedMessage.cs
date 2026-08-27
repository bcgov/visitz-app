using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models.Navigation;

namespace Visitz.Messaging;

public class TodoMasterSelectedMessage(NavItem value) : ValueChangedMessage<NavItem>(value) { }
