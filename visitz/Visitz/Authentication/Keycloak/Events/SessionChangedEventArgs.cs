namespace Visitz.Authentication.Keycloak.Events;

public class SessionChangedEventArgs : EventArgs
{
    public bool Success { get; set; }
}
