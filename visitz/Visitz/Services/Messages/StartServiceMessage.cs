namespace Visitz.Services.Messages;

#nullable enable

public class StartServiceMessage : ServiceInfoMessage
{
    public object Payload { get; set; } = new();
}
