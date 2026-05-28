namespace Visitz.Services.Messages;

#nullable enable

public class ServiceInfoMessage
{
    public required string ServiceId { get; set; }

    public required Type ServiceType { get; set; }
}
