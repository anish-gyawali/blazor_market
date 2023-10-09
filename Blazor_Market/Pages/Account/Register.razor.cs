using Blazor_Market.API.Model.AccountModel;
using Blazor_Market.Services.AuthenticationService;
using Microsoft.AspNetCore.Components;

namespace Blazor_Market.Pages.Account
{
    public partial class Register
    {
        
        private bool isRegistering = false;
        private bool registrationSuccessful = false;
        private RegisterModel registerModel = new RegisterModel();
        

        [Inject]
        private IAuthenticationService? AuthenticationService { get; set; }
        [Inject]
        private NavigationManager? NavigationManager { get; set; }
        private async Task HandleValidSubmit()
        {
            // Start the registration process
            isRegistering = true;

            //send the registration request to the server
            bool registrationResult = await AuthenticationService!.RegisterAsync(registerModel); 

            // Update the registration state
            registrationSuccessful = registrationResult;

            // Stop the registration process
            isRegistering = false;

            if (registrationResult)
            {
                // Registration was successful, clear the form and redirect to the home page
                registerModel = new RegisterModel();
                NavigationManager!.NavigateTo("/");
            }
            else
            {
                registerModel = new RegisterModel();
                NavigationManager!.NavigateTo("/login");
            }
        }
    }
}
