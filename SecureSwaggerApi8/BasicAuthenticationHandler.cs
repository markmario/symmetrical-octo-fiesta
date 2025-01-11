// Program.cs
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
// BasicAuthenticationHandler.cs
public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TimeProvider clock,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            Response.Headers.Append("WWW-Authenticate", "Basic");
            return Task.FromResult(AuthenticateResult.Fail("Authorization header not found."));
        }

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(
                Request.Headers.Authorization.ToString());
            if (authHeader.Scheme != "Basic")
                return Task.FromResult(AuthenticateResult.Fail("Authentication scheme not supported"));

            var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            
            if (credentials.Length != 2)
                return Task.FromResult(AuthenticateResult.Fail("Invalid authentication credentials"));

            var username = credentials[0];
            var password = credentials[1];

            if (!IsValidUser(username, password))
                return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "ApiUser")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));
        }
    }

    private bool IsValidUser(string username, string password)
    {
        // In production, use proper secure credential storage
        var validUsername = _configuration["SwaggerCredentials:Username"];
        var validPassword = _configuration["SwaggerCredentials:Password"];

        return username == validUsername && password == validPassword;
    }
}