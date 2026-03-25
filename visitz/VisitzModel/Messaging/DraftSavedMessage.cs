using CommunityToolkit.Mvvm.Messaging.Messages;

namespace VisitzModel.Messaging;

public class DraftSavedMessage<T>(T item) : ValueChangedMessage<T>(item) { }
