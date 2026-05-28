using Visitz.Services.Base;

namespace Visitz.Services;

#nullable enable

public class ServiceStateMessage : ServiceInfoMessage
{
    public VisitzService.State Status { get; set; }

    public VisitzService.Result Result { get; set; }

    public string Message { get; set; } = string.Empty;

    public Exception? UncaughtException { get; set; }

    public object? ReturnPayload { get; set; }

    public bool IsRunning => Status == VisitzService.State.Running;

    public bool FinishedSuccess => Status == VisitzService.State.Stopped && Result == VisitzService.Result.Successful;

    public bool FinishedError => Status == VisitzService.State.Stopped && Result == VisitzService.Result.Error;

    public bool FinishedCancelled => Status == VisitzService.State.Stopped && Result == VisitzService.Result.Cancelled;

    public override string ToString()
    {
        string result = Status == VisitzService.State.Stopped ? Result.ToString() : "";
        string message = string.IsNullOrWhiteSpace(Message) ? "" : " " + Message;

        return $"{nameof(ServiceStateMessage)} {Status} {result}{Message}";
    }
}
