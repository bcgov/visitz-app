namespace Visitz.Services.Messages;

public class ServiceInfoMessage
{
    public required string ServiceId { get; set; }

    public required Type ServiceType { get; set; }
}
