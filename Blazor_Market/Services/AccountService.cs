using Blazor_Market.API.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blazor_Market.Services
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;
        public AccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> RegisterAsync(RegisterModel registerModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/account/register", registerModel);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LoginUserAsync(LoginModel loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/account/login", loginModel);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
