namespace Visitz.Services.Messages;

public class StartServiceMessage : ServiceInfoMessage
{
    public object Payload { get; set; } = new();
}
