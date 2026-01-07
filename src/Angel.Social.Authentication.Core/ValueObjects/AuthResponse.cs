namespace Angel.Social.Authentication.Core.ValueObjects;

public sealed class AuthResponse<TResponse> where TResponse : class
{
    public TResponse? Data { get; private set; }
    public Dictionary<string, string>? Errors { get; private set; }

    public bool IsSuccess => Errors == null || Errors.Count == 0;

    private AuthResponse(TResponse? data, Dictionary<string, string>? errors)
    {
        Data = data;
        Errors = errors;
    }

    public static AuthResponse<TResponse> Success(TResponse data) =>
        new AuthResponse<TResponse>(data, null);

    public static AuthResponse<TResponse> Failure(Dictionary<string, string> errors) =>
        new AuthResponse<TResponse>(null, errors);
}
