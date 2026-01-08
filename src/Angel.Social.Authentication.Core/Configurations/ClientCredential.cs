namespace Angel.Social.Authentication.Core.Configurations;

public sealed record ClientCredential
{
    public const string GoogleKey = "GoogleCredentials";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
