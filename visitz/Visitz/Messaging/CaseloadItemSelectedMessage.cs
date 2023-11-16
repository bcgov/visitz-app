using CommunityToolkit.Mvvm.Messaging.Messages;
using Visitz.Models;

namespace Visitz.Messaging;

public class CaseloadItemSelectedMessage(CaseloadItem value) : ValueChangedMessage<CaseloadItem>(value)
{
}
