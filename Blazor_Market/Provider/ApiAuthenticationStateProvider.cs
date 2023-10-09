using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Blazor_Market.Provider
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        public ApiAuthenticationStateProvider( ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            var savedToken=await _localStorage.GetItemAsync<string>("authToken");
            if (savedToken == null) 
            {
                return new AuthenticationState(user);
            }
            var tokenContent= _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
            if (tokenContent.ValidTo < DateTime.Now)
            {
                return new AuthenticationState(user);
            }
            var claims=await GetClaims();

             user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            return new AuthenticationState(user);
        }

        public async Task LoggedIn()
        {
            var claims = await GetClaims();
            var user= new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            var authState=Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);

        }

        public async Task LoggedOut()
        {
            await _localStorage.RemoveItemAsync("authToken");
            var noUser= new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(noUser));
            NotifyAuthenticationStateChanged(authState);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
            var claims = tokenContent.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            var uidClaim = tokenContent.Claims.FirstOrDefault(claim => claim.Type == "uid");
            if (uidClaim != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, uidClaim.Value));
            }
            return claims;
        }
    }
}
