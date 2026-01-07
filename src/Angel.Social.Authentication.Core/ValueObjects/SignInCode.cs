namespace Angel.Social.Authentication.Core.ValueObjects;

public sealed record SignInCode
{
    public string Code { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
}
