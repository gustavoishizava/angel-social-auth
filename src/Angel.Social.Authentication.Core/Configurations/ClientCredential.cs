namespace Angel.Social.Authentication.Core.Configurations;

public sealed record ClientCredential
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
