using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models;

namespace VisitzModel.Messaging;

public class CaseloadItemSelectedMessage(CaseloadItem value) : ValueChangedMessage<CaseloadItem>(value)
{
}
