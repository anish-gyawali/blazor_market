using Blazor_Market.API.Model.AccountModel;
using Blazor_Market.Provider;
using Blazored.LocalStorage;
using System.Text.Json;

namespace Blazor_Market.Services.AuthenticationService
{
    public class AuthenticationService:IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ApiAuthenticationStateProvider _authenticationStateProvider;
        public AuthenticationService(HttpClient httpClient, ILocalStorageService localStorage, ApiAuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> LoginAsync(LoginModel loginModel)
        {
            try
            {
                var loginAsJson = JsonSerializer.Serialize(loginModel);
                var response = await _httpClient.PostAsJsonAsync("/api/account/login", loginModel);
                var loginResult = JsonSerializer.Deserialize<LoginResponse>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (!response.IsSuccessStatusCode)
                {

                    return false;
                }
                await _localStorage.SetItemAsync("authToken", loginResult?.Token);

                //change authsate of app
                await _authenticationStateProvider.LoggedIn();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> RegisterAsync(RegisterModel registerModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/account/register", registerModel);

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task Logout()
        {
            await _authenticationStateProvider.LoggedOut();
        }
    }
}
