using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using System.Text.Json;
using System.Text;
using BasicSealBlazor.Services;
using Microsoft.AspNetCore.Components;
using BasicSealBlazor.Services.Interfaces;

namespace BasicSealBlazor
{
    public class BasicSealAuthProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage;
        private readonly UserAuthStateService _userAuthState;
        private readonly NavigationManager _navigationManager;
        private readonly ICipherService _cipherService;

        private User user { get; set; }

        public BasicSealAuthProvider(ISessionStorageService sessionStorage, UserAuthStateService userAuthState, 
            NavigationManager navigationManager, ICipherService cipherService)
        {
            _sessionStorage = sessionStorage;
            _userAuthState = userAuthState;
            _navigationManager = navigationManager;
            _cipherService = cipherService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            User userSession = await GetUserSession();

            if (userSession != null)
            {
                if(DateTime.Compare(DateTime.UtcNow, userSession.expireTime) <= 0)
                {
                    return await AuthenticateUser(userSession);
                } 
            }

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
        }

        private Task<AuthenticationState> AuthenticateUser(User _user)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, _user.userEmail),
                new Claim(ClaimTypes.AuthenticationMethod, _user.userToken),
                new Claim(ClaimTypes.Expiration, _user.expireTime.ToString())
            }, "BasicSealExternalAuthentication");

            _userAuthState.isLoggedIn = true;
            _userAuthState.email = user.userEmail;
            _userAuthState.token = user.userToken;

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public async Task LoginAsync(User user)
        {
            user.expireTime = DateTime.Now.AddDays(1);
            await SetUserSession(user);

            NotifyAuthenticationStateChanged(AuthenticateUser(user));
        }

        public async Task LogoutAsync()
        {
            await SetUserSession(null);

            _userAuthState.isLoggedIn = false;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
        }

        private async Task<User> GetUserSession()
        {
            try
            {
                if (user != null)
                { return user; }

                string storedUser = await _sessionStorage.GetItemAsync<string>("user");

                if (string.IsNullOrEmpty(storedUser))
                { return null; }

                string encryptedData = Encoding.Unicode.GetString(Convert.FromBase64String(storedUser));
                string decryptedData = _cipherService.Decrypt(encryptedData);
                var session = RefreshUserSession(JsonSerializer.Deserialize<User>(decryptedData));

                if (session == null) 
                {
                    _userAuthState.isLoggedIn = false;
                }
                else
                {
                    _userAuthState.isLoggedIn = true;
                    _userAuthState.email = user.userEmail;
                    _userAuthState.token = user.userToken;
                }

                return session;
            }
            catch
            {
                _userAuthState.isLoggedIn = false;
                await LogoutAsync();
                _navigationManager.NavigateTo("/login");
                return null;
            }
        }

        private async Task SetUserSession(User user)
        {
            RefreshUserSession(user);

            if(user == null)
            {
                await _sessionStorage.RemoveItemAsync("user");
            }
            else
            {
                string encryptedJson = _cipherService.Encrypt(JsonSerializer.Serialize(user));
                string userBase64 = Convert.ToBase64String(Encoding.Unicode.GetBytes(encryptedJson));
                await _sessionStorage.SetItemAsync("user", userBase64);
            }
        }

        private User RefreshUserSession(User _user) => user = _user;
    }

    public class User
    {
        public string userEmail { get; set; }
        public string userToken { get; set; }
        public DateTime expireTime { get; set; }
    }
}
