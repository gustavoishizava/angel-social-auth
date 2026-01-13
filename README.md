# Angel Social Authentication

A lightweight and easy-to-use library for OAuth 2.0 authentication with social providers like Google and Facebook in .NET 10 applications. The library provides opinionated abstractions and helpers to simplify integration with social authentication providers, allowing you to focus on business logic instead of OAuth implementation details.

## Overview

Angel Social Authentication is built on top of .NET's HttpClient and provides a consistent, strongly-typed API for working with different social authentication providers. The library handles authorization URL generation, code-to-token exchange, and user information retrieval.

**Key goals:**

- **Ease of use**: Simple, intuitive API for social provider integration
- **Extensibility**: Architecture that allows adding new providers easily
- **Strongly typed**: Leverages .NET 10 and C# features for a type-safe experience
- **DI integration**: Ready-to-use extensions for ASP.NET Core's dependency injection container

## Features

- **Credential management**: Centralized configuration of Client ID and Client Secret per provider
- **Authorization URL generation**: Automatic creation of authorization URLs with all necessary parameters
- **Code-to-token exchange**: Transparent exchange of authorization code for access token
- **Refresh token**: Support for token renewal (Google)
- **User data retrieval**: Methods to fetch authenticated user profile information
- **Supported providers**: Google and Facebook (with extensible architecture for new providers)

## Getting started

### Requirements

- .NET SDK 10.0
- OAuth credentials configured in the providers (Google Cloud Console, Facebook Developers)

### Install via NuGet

Install the library from NuGet into your project:

```bash
dotnet add package Angel.Social.Authentication --version 1.0.0
```

Or add a `PackageReference` to your project file:

```xml
<PackageReference Include="Angel.Social.Authentication" Version="1.0.0" />
```

### Use from your project

Add a project reference to `Angel.Social.Authentication`, or build a NuGet package if you prefer to publish and consume it that way.

## Quick usage (examples)

The examples below show common patterns and the library's DI extensions. Use them as a starting point to integrate Angel Social Authentication into your application.

### 1. Configure providers

Use the `AddGoogleProvider` and `AddFacebookProvider` extensions to configure credentials during service registration.

**Configuration via appsettings.json:**

```json
{
  "GoogleCredentials": {
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret"
  },
  "FacebookCredentials": {
    "ClientId": "your-facebook-app-id",
    "ClientSecret": "your-facebook-app-secret"
  }
}
```

**Registration in Program.cs:**

```csharp
using Angel.Social.Authentication.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register Google provider
builder.Services.AddGoogleProvider(builder.Configuration, "GoogleCredentials");

// Register Facebook provider
builder.Services.AddFacebookProvider(builder.Configuration, "FacebookCredentials");

var app = builder.Build();
```

**Configuration via code:**

```csharp
builder.Services.AddGoogleProvider(options =>
{
    options.ClientId = "your-google-client-id";
    options.ClientSecret = "your-google-client-secret";
});

builder.Services.AddFacebookProvider(options =>
{
    options.ClientId = "your-facebook-app-id";
    options.ClientSecret = "your-facebook-app-secret";
});
```

### 2. Generate authorization URL

To redirect the user to the provider's consent screen, you need to generate an authorization URL.

**Example with Google:**

```csharp
using Angel.Social.Authentication.Google.Services;
using Angel.Social.Authentication.Google.ValueObjects;

public class AuthController : ControllerBase
{
    private readonly IGoogleProvider _googleProvider;

    public AuthController(IGoogleProvider googleProvider)
    {
        _googleProvider = googleProvider;
    }

    [HttpGet("google/login")]
    public IActionResult GoogleLogin()
    {
        var request = new GoogleOAuthRequestUrl
        {
            RedirectUri = "https://yourapp.com/auth/google/callback",
            Scopes = new[] { "openid", "profile", "email" },
            State = "random-state-string",
            AccessType = "offline", // To obtain refresh token
            Prompt = "consent",
            ResponseType = "code"
        };

        var authUrl = _googleProvider.GetUri(request);
        return Redirect(authUrl.ToString());
    }
}
```

**Example with Facebook:**

```csharp
using Angel.Social.Authentication.Facebook.Services;
using Angel.Social.Authentication.Facebook.ValueObjects;

public class AuthController : ControllerBase
{
    private readonly IFacebookProvider _facebookProvider;

    public AuthController(IFacebookProvider facebookProvider)
    {
        _facebookProvider = facebookProvider;
    }

    [HttpGet("facebook/login")]
    public IActionResult FacebookLogin()
    {
        var request = new FacebookOAuthRequestUrl
        {
            RedirectUri = "https://yourapp.com/auth/facebook/callback",
            Scopes = new[] { "email", "public_profile" },
            State = "random-state-string",
            ResponseType = "code"
        };

        var authUrl = _facebookProvider.GetUri(request);
        return Redirect(authUrl.ToString());
    }
}
```

### 3. Exchange authorization code for access token

After the user consents, the provider redirects back to your application with an authorization code. Use this code to obtain the access token.

**Example with Google:**

```csharp
[HttpGet("google/callback")]
public async Task<IActionResult> GoogleCallback([FromQuery] string code)
{
    var signInCode = new SignInCode
    {
        Code = code,
        RedirectUri = "https://yourapp.com/auth/google/callback",
        State = "random-state-string"
    };

    var tokenResponse = await _googleProvider.GetAccessTokenAsync(signInCode);
    
    // tokenResponse contains: access_token, refresh_token, expires_in, token_type, id_token
    // Store tokens securely and use them to access Google APIs
    
    return Ok(new 
    { 
        AccessToken = tokenResponse.AccessToken,
        RefreshToken = tokenResponse.RefreshToken,
        ExpiresIn = tokenResponse.ExpiresIn
    });
}
```

**Example with Facebook:**

```csharp
[HttpGet("facebook/callback")]
public async Task<IActionResult> FacebookCallback([FromQuery] string code)
{
    var signInCode = new SignInCode
    {
        Code = code,
        RedirectUri = "https://yourapp.com/auth/facebook/callback",
        State = "random-state-string"
    };

    var tokenResponse = await _facebookProvider.GetAccessTokenAsync(signInCode);
    
    // tokenResponse contains: access_token, token_type, expires_in
    
    return Ok(new 
    { 
        AccessToken = tokenResponse.AccessToken,
        ExpiresIn = tokenResponse.ExpiresIn
    });
}
```

### 4. Refresh access token (Google)

Google allows you to renew the access token using the refresh token, avoiding the need for the user to authorize again.

```csharp
[HttpPost("google/refresh")]
public async Task<IActionResult> RefreshGoogleToken([FromBody] string refreshToken)
{
    var tokenResponse = await _googleProvider.RefreshAccessTokenAsync(refreshToken);
    
    return Ok(new 
    { 
        AccessToken = tokenResponse.AccessToken,
        ExpiresIn = tokenResponse.ExpiresIn
    });
}
```

### 5. Get user information

After obtaining the access token, you can fetch the user's profile information.

**Example with Facebook:**

```csharp
public class UserInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

[HttpGet("facebook/user")]
public async Task<IActionResult> GetFacebookUser([FromQuery] string accessToken)
{
    var fields = new[] { "id", "name", "email" };
    var userInfo = await _facebookProvider.GetUserAsync<UserInfo>(
        accessToken, 
        fields);
    
    return Ok(userInfo);
}
```

### 6. Complete authentication flow example

```csharp
using Angel.Social.Authentication.Facebook.Services;
using Angel.Social.Authentication.Facebook.ValueObjects;
using Angel.Social.Authentication.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IFacebookProvider _facebookProvider;

    public AuthController(IFacebookProvider facebookProvider)
    {
        _facebookProvider = facebookProvider;
    }

    // Step 1: Generate authorization URL
    [HttpGet("facebook/login")]
    public IActionResult InitiateLogin()
    {
        var request = new FacebookOAuthRequestUrl
        {
            RedirectUri = "https://yourapp.com/auth/facebook/callback",
            Scopes = new[] { "email", "public_profile" },
            State = Guid.NewGuid().ToString(), // Use a secure generator in production
            ResponseType = "code"
        };

        var authUrl = _facebookProvider.GetUri(request);
        return Redirect(authUrl.ToString());
    }

    // Step 2: Receive callback and exchange code for token
    [HttpGet("facebook/callback")]
    public async Task<IActionResult> HandleCallback(
        [FromQuery] string code, 
        [FromQuery] string state)
    {
        // Validate state to prevent CSRF
        
        var signInCode = new SignInCode
        {
            Code = code,
            RedirectUri = "https://yourapp.com/auth/facebook/callback",
            State = state
        };

        var tokenResponse = await _facebookProvider.GetAccessTokenAsync(signInCode);

        // Step 3: Fetch user information
        var fields = new[] { "id", "name", "email" };
        var userInfo = await _facebookProvider.GetUserAsync<dynamic>(
            tokenResponse.AccessToken, 
            fields);

        // Here you can:
        // - Create or update user in your database
        // - Generate your own JWT tokens
        // - Establish user session
        
        return Ok(new 
        { 
            User = userInfo,
            AccessToken = tokenResponse.AccessToken 
        });
    }
}
```

## Architecture and extensibility

The library is built with an extensible architecture that allows adding new providers easily. Each provider implements the `IExternalProvider<TOAuthRequest, TOAuthResponse>` interface.

### Project structure

- **Angel.Social.Authentication.Core**: Contains shared interfaces, value objects, and exceptions
- **Angel.Social.Authentication.Google**: Google OAuth 2.0 specific implementation
- **Angel.Social.Authentication.Facebook**: Facebook Login specific implementation
- **Angel.Social.Authentication**: DI extensions to facilitate provider registration
- **Angel.Social.Authentication.WebApi.Sample**: Sample application demonstrating usage

### Adding a new provider

To add support for a new OAuth provider:

1. Create a new project following the pattern `Angel.Social.Authentication.{Provider}`
2. Implement the `IExternalProvider<TOAuthRequest, TOAuthResponse>` interface
3. Create provider-specific value objects (request/response)
4. Define URL and parameter constants
5. Add extension methods for DI registration

## Error handling

The library throws `OAuthException` when errors occur during the authentication process, such as:

- Failure to exchange code for token
- Invalid response from provider
- Network issues or timeouts

```csharp
try
{
    var tokenResponse = await _googleProvider.GetAccessTokenAsync(signInCode);
}
catch (OAuthException ex)
{
    _logger.LogError(ex, "Error authenticating with Google");
    return BadRequest("Authentication failed. Please try again.");
}
```

## Contributing

- Create issues for bugs and feature requests
- Fork the repository, implement your changes, and open a pull request with tests
- Keep changes focused and add unit tests

## License

This project is licensed under the MIT License — see the `LICENSE` file for details.

Copyright (c) 2026 Gustavo Ishizava

## Questions / Contact

For questions or feedback, open an issue in this repository or contact the maintainer.
