namespace Oidc.Events;

public class SessionChangedEventArgs : EventArgs
{
    public bool Success { get; set; }
}
