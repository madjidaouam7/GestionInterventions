using Microsoft.JSInterop;

namespace GestionInterventions.Web.Authentication;

public class TokenProvider
{
    private const string TokenStorageKey = "gestion-interventions-token";
    private readonly IJSRuntime _jsRuntime;
    private bool _hasLoaded;

    public TokenProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string? Token { get; private set; }

    public async Task LoadTokenAsync()
    {
        if (_hasLoaded)
            return;

        try
        {
            Token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenStorageKey);
            _hasLoaded = true;
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop is unavailable during prerendering.
        }
    }

    public async Task SetTokenAsync(string? token)
    {
        Token = token;
        _hasLoaded = true;

        if (string.IsNullOrWhiteSpace(token))
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenStorageKey);
            return;
        }

        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenStorageKey, token);
    }
}
