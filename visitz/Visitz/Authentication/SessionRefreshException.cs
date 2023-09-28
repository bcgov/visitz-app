namespace Visitz.Authentication;

public class SessionRefreshException : Exception
{
    public SessionRefreshException(string message) : base(message)
    {

    }
}
