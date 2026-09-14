namespace GestionInterventions.Web.Authentication;

public class TokenProvider
{
    public string? Token { get; private set; }

    public void SetToken(string? token)
    {
        Token = token;
    }
}
