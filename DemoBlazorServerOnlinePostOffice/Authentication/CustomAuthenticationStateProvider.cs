using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DemoBlazorServerOnlinePostOffice.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly ILocalStorageService localStorageService;

        public CustomAuthenticationStateProvider(ILocalStorageService localStorageService)
        {
            this.localStorageService = localStorageService;
        }

        // Override Default Method to authenticate and deauthenticate user during page navigations.
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await localStorageService.GetItemAsStringAsync("token");
                if (string.IsNullOrEmpty(token)) return await Task.FromResult(new AuthenticationState(anonymous));

                var (id, email, role) = await GetClaimsFromJWT(token);
                return await Task.FromResult(new AuthenticationState(SetClaimsPrincipal(id, email, role)));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(anonymous));
            }

        }

        // Update Token by setting up or deleting from local storage
        public async Task UpdateAuthenticationState(string token)
        {

            ClaimsPrincipal claimsPrincipal;
            if (!string.IsNullOrEmpty(token))
            {
                await localStorageService.SetItemAsStringAsync("token", token);
                var (id, email, role) = await GetClaimsFromJWT(token);
                claimsPrincipal = SetClaimsPrincipal(id, email, role!);
            }
            else
            {
                claimsPrincipal = anonymous;
                await localStorageService.RemoveItemAsync("token");
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        // General methods
        private static ClaimsPrincipal SetClaimsPrincipal(int id, string email, string role)
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                     new(ClaimTypes.NameIdentifier, id.ToString()),
                    new(ClaimTypes.Name, email),
                    new(ClaimTypes.Role, role)
                }, "JwtAuth"));
            return claimsPrincipal;
        }

        public async Task<(int id, string email, string role)> GetClaimsFromJWT(string jwt)
        {

            if (jwt is null) return (-1, null!, null!);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var claims = token.Claims;

            var role = claims.First(_ => _.Type == ClaimTypes.Role).Value;
            var email = claims.First(_ => _.Type == ClaimTypes.Name).Value;
            var id = claims.First(_ => _.Type == ClaimTypes.NameIdentifier).Value;

            return (int.Parse(id), email, role);

        }
    }
}
