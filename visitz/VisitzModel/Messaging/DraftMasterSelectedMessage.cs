using CommunityToolkit.Mvvm.Messaging.Messages;
using Realms;

namespace VisitzModel.Messaging;

public class DraftMasterSelectedMessage(Type draftType, Realm realm)
    : ValueChangedMessage<(Type, Realm)>((draftType, realm)) { }
