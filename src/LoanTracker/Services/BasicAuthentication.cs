using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private static readonly List<UserAuth> users = new List<UserAuth>
    {
        new UserAuth { Username = "user1", Password = "password1" },
        new UserAuth { Username = "user2", Password = "password2" }
    };

    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, System.Text.Encodings.Web.UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    /// <summary>
    /// Обрабатывает аутентификацию с использованием Basic Authentication.
    /// </summary>
    /// <returns>Результат аутентификации.</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
        }

        var authorizationHeader = Request.Headers["Authorization"].ToString();
        if (authorizationHeader.StartsWith("Basic ", StringComparison.InvariantCultureIgnoreCase))
        {
            var encodedCredentials = authorizationHeader.Substring("Basic ".Length).Trim();
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var credentials = decodedCredentials.Split(':');

            if (credentials.Length == 2)
            {
                var user = users.FirstOrDefault(u => u.Username == credentials[0] && u.Password == credentials[1]);

                if (user != null)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user.Username)
                    };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }
        }

        return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
    }
}

public class UserAuth
{
    public string Username { get; set; }
    public string Password { get; set; }
}
