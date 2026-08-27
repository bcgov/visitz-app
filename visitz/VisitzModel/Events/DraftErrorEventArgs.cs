namespace VisitzModel.Events;

public class DraftErrorEventArgs(string errorMessage) : EventArgs()
{
    public string ErrorMessage { get; set; } = errorMessage;
}
