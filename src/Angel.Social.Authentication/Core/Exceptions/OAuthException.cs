namespace Angel.Social.Authentication.Core.Exceptions;

public sealed class OAuthException : Exception
{
    public OAuthException()
    {
    }

    public OAuthException(string message)
        : base(message)
    {
    }

    public OAuthException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
