namespace Oidc.Exceptions;

public class SessionRefreshException : Exception
{
    public SessionRefreshException(string message) : base(message)
    {

    }
}
