namespace VisitzApi.Models.Attachments;

public class CfaDetailsPayload
{
    public string RecordingType { get; set; } = string.Empty;
    public string DangerStatement { get; set; } = string.Empty;
    public string DecisionMaking { get; set; } = string.Empty;
    public string SafetyGoals { get; set; } = string.Empty;
}
