namespace Oidc.Network;

public class InternetUnavailableException : Exception
{
    public InternetUnavailableException(string message) : base(message)
    {

    }
}
