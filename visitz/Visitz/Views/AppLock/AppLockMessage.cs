using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Visitz.Views.AppLock;

public class AppLockMessage(AppLockStatus value) : ValueChangedMessage<AppLockStatus>(value) { }
