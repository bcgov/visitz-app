using CommunityToolkit.Mvvm.Messaging.Messages;

namespace VisitzModel.Messaging;

public class NavPositionMessage(int twoPaneModeInt) : ValueChangedMessage<int>(twoPaneModeInt) { }
