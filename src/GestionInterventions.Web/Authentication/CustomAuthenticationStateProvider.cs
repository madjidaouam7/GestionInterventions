using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace GestionInterventions.Web.Authentication;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly TokenProvider _tokenProvider;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(TokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = _tokenProvider.Token;

        if (string.IsNullOrWhiteSpace(token))
            return Task.FromResult(new AuthenticationState(_anonymous));

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    public void MarkUserAsAuthenticated(string token)
    {
        _tokenProvider.SetToken(token);

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void MarkUserAsLoggedOut()
    {
        _tokenProvider.SetToken(null);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}