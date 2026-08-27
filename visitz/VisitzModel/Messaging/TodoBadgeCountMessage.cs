using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Visitz.Messaging;

public class TodoBadgeCountMessage(int count) : ValueChangedMessage<int>(count) { }
