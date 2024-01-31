using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Visitz.Messaging;

internal class DraftSavedMessage<T>(T item) : ValueChangedMessage<T>(item) { }
