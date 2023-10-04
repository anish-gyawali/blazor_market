using Blazor_Market.API.Model.AccountModel;
using Blazor_Market.Services.Authentication;
using Microsoft.AspNetCore.Components;

namespace Blazor_Market.Pages.Account
{
    public partial class Login
    {
        private bool isLoading = false;
        private bool loginSuccessful = false;
        private LoginModel loginModel = new LoginModel();

        [Inject]
        private IAuthenticationService? AuthenticationService { get; set; }
        [Inject]
        private NavigationManager? NavigationManager { get; set; }
        private async void HandleValidSubmit()
        {
            isLoading = true;
            bool loginResult = await AuthenticationService!.LoginAsync(loginModel);
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
