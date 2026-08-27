using CommunityToolkit.Mvvm.Messaging.Messages;

namespace VisitzModel.Messaging;

public class NavDrawerMessage(bool isOpen) : ValueChangedMessage<bool>(isOpen) { }
