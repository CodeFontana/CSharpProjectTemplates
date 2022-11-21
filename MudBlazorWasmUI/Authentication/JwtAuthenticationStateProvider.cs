using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace MudBlazorWasmUI.Authentication;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IConfiguration _config;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger;
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navMan;
    private readonly AuthenticationState _anonymous;
    private Task _authExpiryMonitor;
    private CancellationTokenSource _authExpiryMonitorTokenSource;
    private bool _isAuthenticated = false;

    public JwtAuthenticationStateProvider(IConfiguration config,
                                          ILogger<JwtAuthenticationStateProvider> logger,
                                          HttpClient httpClient,
                                          ILocalStorageService localStorage,
                                          NavigationManager navigationManager)
    {
        _config = config;
        _logger = logger;
        _http = httpClient;
        _localStorage = localStorage;
        _navMan = navigationManager;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            string localToken = await _localStorage.GetItemAsync<string>(_config["authTokenStorageKey"]);

            if (string.IsNullOrWhiteSpace(localToken))
            {
                await NotifyUserLogoutAsync();
                return _anonymous;
            }

            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken token = tokenHandler.ReadToken(localToken);
            var tokenExpiryDate = token.ValidTo;

            // If there is no valid 'exp' claim then 'ValidTo' returns DateTime.MinValue.
            if (tokenExpiryDate == DateTime.MinValue)
            {
                _logger.LogWarning("Invalid JWT [Missing 'exp' claim]");
                await NotifyUserLogoutAsync();
                return _anonymous;
            }

            // If the token is in the past then you can't use it.
            if (tokenExpiryDate < DateTime.UtcNow)
            {
                _logger.LogWarning($"Invalid JWT [Token expired on {tokenExpiryDate.ToLocalTime()}]");
                _navMan.NavigateTo("/");
                await NotifyUserLogoutAsync();
                return _anonymous;
            }

            bool isAuthenticated = await NotifyUserAuthenticationAsync(localToken);

            if (isAuthenticated == false)
            {
                await NotifyUserLogoutAsync();
                return _anonymous;
            }

            return new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity(
                        JwtParser.ParseClaimsFromJwt(localToken),
                        "jwtAuthType")));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error determining the authentication state");
            await NotifyUserLogoutAsync();
            return _anonymous;
        }
    }

    public async Task AuthenticationExpiryMonitor(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            string localToken = await _localStorage.GetItemAsync<string>(_config["authTokenStorageKey"]);

            if (string.IsNullOrWhiteSpace(localToken))
            {
                break;
            }

            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken token = tokenHandler.ReadToken(localToken);
            DateTime tokenExpiryDate = token.ValidTo;

            if (tokenExpiryDate < DateTime.UtcNow)
            {
                _logger.LogWarning($"Invalid JWT [Token expired on {tokenExpiryDate.ToLocalTime()}]");
                _navMan.NavigateTo("/sessionexpired", false);
                await NotifyUserLogoutAsync();
                break;
            }

            await Task.Delay(10000);
        }
    }

    public async Task<bool> NotifyUserAuthenticationAsync(string token)
    {
        try
        {
            ClaimsPrincipal authenticatedUser = new(
                new ClaimsIdentity(
                    JwtParser.ParseClaimsFromJwt(token),
                    "jwtAuthType"));

            Task<AuthenticationState> authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            string authTokenStorageKey = _config["authTokenStorageKey"];
            await _localStorage.SetItemAsync(authTokenStorageKey, token);

            NotifyAuthenticationStateChanged(authState);
            _isAuthenticated = true;

            if (_authExpiryMonitor == null || _authExpiryMonitor.IsCompleted)
            {
                _authExpiryMonitorTokenSource = new();
                _authExpiryMonitor = await Task.Factory.StartNew(() =>
                    AuthenticationExpiryMonitor(
                        _authExpiryMonitorTokenSource.Token),
                        TaskCreationOptions.LongRunning);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update authentication state");
            await NotifyUserLogoutAsync();
            _isAuthenticated = false;
        }

        return _isAuthenticated;
    }

    public async Task NotifyUserLogoutAsync()
    {
        _authExpiryMonitorTokenSource?.Cancel();
        string authTokenStorageKey = _config["authTokenStorageKey"];
        await _localStorage.RemoveItemAsync(authTokenStorageKey);
        Task<AuthenticationState> authState = Task.FromResult(_anonymous);
        _http.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(authState);
        _isAuthenticated = false;
    }
}
