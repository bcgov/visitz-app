using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models;

namespace Visitz.Messaging;

public class CaseloadItemSelectedMessage(CaseloadItem value) : ValueChangedMessage<CaseloadItem>(value)
{
}
