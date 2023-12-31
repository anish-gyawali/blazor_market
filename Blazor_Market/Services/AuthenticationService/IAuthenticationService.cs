﻿using Blazor_Market.API.Model.AccountModel;

namespace Blazor_Market.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(LoginModel loginModel);
        Task<bool> RegisterAsync(RegisterModel registerModel);
        public Task Logout();
    }
}