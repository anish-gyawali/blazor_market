using Blazor_Market.API.Model;
using Blazor_Market.Services;
using Microsoft.AspNetCore.Components;

namespace Blazor_Market.Pages.Account
{
    public partial class Login
    {
        private bool isLoading = false;
        private bool loginSuccessful = false;
        private LoginModel loginModel = new LoginModel();

        [Inject]
        private AccountService? AccountService { get; set; }
        [Inject]
        private NavigationManager? NavigationManager { get; set; }
        private async void HandleValidSubmit()
        {
            isLoading = true;
            bool loginResult = await AccountService!.LoginUserAsync(loginModel);
            loginSuccessful = loginResult;
            isLoading = false;
            if(loginResult)
            {
                loginModel=new LoginModel();
                NavigationManager!.NavigateTo("/");
            }
            else
            {
                loginModel = new LoginModel();
                NavigationManager!.NavigateTo("/login");
            }
        }
    }
}
