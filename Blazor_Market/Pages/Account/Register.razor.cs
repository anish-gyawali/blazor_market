using Blazor_Market.API.Model;
using Blazor_Market.Services;
using Microsoft.AspNetCore.Components;

namespace Blazor_Market.Pages.Account
{
    public partial class Register
    {
        
        private bool isRegistering = false;
        private bool registrationSuccessful = false;
        private readonly NavigationManager? navigationManager;
        private RegisterModel registerModel = new RegisterModel();
        

        [Inject]
        private AccountService? AccountService { get; set; }
        private async Task HandleValidSubmit()
        {
            // Start the registration process
            isRegistering = true;

            //send the registration request to the server
            bool registrationResult = await AccountService!.RegisterAsync(registerModel); 

            // Update the registration state
            registrationSuccessful = registrationResult;

            // Stop the registration process
            isRegistering = false;

            if (registrationResult)
            {
                // Registration was successful, clear the form and redirect to the home page
                registerModel = new RegisterModel();
                navigationManager!.NavigateTo("/home");
            }
        }
    }
}
