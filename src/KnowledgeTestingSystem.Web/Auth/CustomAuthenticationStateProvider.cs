using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using KnowledgeTestingSystem.Web.Data.Models;
using KnowledgeTestingSystem.Web.Data;
using Microsoft.EntityFrameworkCore;
// using Microsoft.JSInterop; // Не нужен здесь напрямую, если ловим InvalidOperationException
// using Microsoft.AspNetCore.Components; // Не нужен здесь напрямую IHostEnvironmentAuthenticationStateProvider

namespace KnowledgeTestingSystem.Web.Auth // Используем ваш namespace
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly KnowledgeTestingDbContext _dbContext;
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity()); // Изначально аноним
        private bool _authenticationStateLoadedAttemptedInInteractiveMode = false;

        public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage, KnowledgeTestingDbContext dbContext)
        {
            _sessionStorage = sessionStorage;
            _dbContext = dbContext;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Если мы уже успешно загрузили состояние в интерактивном режиме (после логина или при первом интерактивном рендере),
            // и пользователь аутентифицирован, просто возвращаем текущее состояние.
            if (_authenticationStateLoadedAttemptedInInteractiveMode && (_currentUser.Identity?.IsAuthenticated ?? false))
            {
                return new AuthenticationState(_currentUser);
            }

            UserSession? userSession = null;
            try
            {
                // Попытка загрузить сессию. Сработает только в интерактивном режиме.
                var userSessionResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                userSession = userSessionResult.Success ? userSessionResult.Value : null;
                // Если мы дошли сюда без исключения, значит мы в интерактивном режиме
                // и попытка чтения из хранилища была (успешной или нет - не важно для флага).
                _authenticationStateLoadedAttemptedInInteractiveMode = true;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop calls cannot be issued at this time"))
            {
                // Это пререндеринг. JS interop (и _sessionStorage) недоступен.
                // _currentUser остается анонимным. Флаг _authenticationStateLoadedAttemptedInInteractiveMode остается false.
                Console.WriteLine("CustomAuthenticationStateProvider (Prerendering): JS interop unavailable for GetAuthenticationStateAsync. Returning anonymous state.");
                return new AuthenticationState(_currentUser); // Возвращаем текущий (анонимный)
            }
            catch (Exception ex)
            {
                // Другие возможные ошибки при доступе к sessionStorage
                Console.WriteLine($"CustomAuthenticationStateProvider: Error accessing sessionStorage in GetAuthenticationStateAsync: {ex.Message}");
                _authenticationStateLoadedAttemptedInInteractiveMode = true; // Отмечаем попытку, даже если она неудачна
                return new AuthenticationState(_currentUser); // Возвращаем текущий (анонимный)
            }

            if (userSession == null || string.IsNullOrEmpty(userSession.UserId))
            {
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity()); // Аноним
            }
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userSession.UserId),
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Role, userSession.Role)
                };
                if (!string.IsNullOrEmpty(userSession.FullName))
                {
                    claims.Add(new Claim("FullName", userSession.FullName));
                }
                if (!string.IsNullOrEmpty(userSession.GroupName))
                {
                    claims.Add(new Claim("GroupName", userSession.GroupName));
                }
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));
            }

            return new AuthenticationState(_currentUser);
        }

        public async Task MarkUserAsAuthenticated(User authenticatedUser)
        {
            User? userWithGroup = await _dbContext.Users
                                    .Include(u => u.Group)
                                    .FirstOrDefaultAsync(u => u.UserId == authenticatedUser.UserId);

            // Если userWithGroup все еще null (не должно быть, если authenticatedUser валиден),
            // используем данные из authenticatedUser, но группа может быть не загружена.
            if (userWithGroup == null)
            {
                Console.WriteLine($"CustomAuthenticationStateProvider: User with ID {authenticatedUser.UserId} not found with group info in MarkUserAsAuthenticated. Using passed user object.");
                userWithGroup = authenticatedUser; // Используем то, что передали, но Group может быть null
            }

            // Используем Name из модели User для FullName
            string fullName = userWithGroup.Name; // У вас в модели User есть свойство Name
            // Используем Group.Name, если группа загружена
            string groupName = userWithGroup.Group?.Name ?? (userWithGroup.Role == "Обучающийся" ? "Группа не определена" : "");

            var userSession = new UserSession
            {
                UserId = authenticatedUser.UserId.ToString(),
                UserName = authenticatedUser.Login, // Логин пользователя
                Role = authenticatedUser.Role,
                FullName = fullName,
                GroupName = groupName
            };

            try
            {
                await _sessionStorage.SetAsync("UserSession", userSession);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop calls cannot be issued at this time"))
            {
                Console.WriteLine("CustomAuthenticationStateProvider: JS interop unavailable during MarkUserAsAuthenticated. This should not happen as login is interactive.");
                // Это очень маловероятно, так как MarkUserAsAuthenticated вызывается из LoginPage,
                // который должен быть интерактивным. Если это произошло, что-то не так с режимами рендеринга LoginPage.
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authenticatedUser.UserId.ToString()),
                new Claim(ClaimTypes.Name, authenticatedUser.Login),
                new Claim(ClaimTypes.Role, authenticatedUser.Role),
                new Claim("FullName", fullName),
                new Claim("GroupName", groupName)
            };
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));
            _authenticationStateLoadedAttemptedInInteractiveMode = true; // Состояние успешно установлено

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            try
            {
                await _sessionStorage.DeleteAsync("UserSession");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop calls cannot be issued at this time"))
            {
                Console.WriteLine("CustomAuthenticationStateProvider: JS interop unavailable during MarkUserAsLoggedOut.");
                // Аналогично MarkUserAsAuthenticated, это не должно происходить в нормальном потоке
            }

            _currentUser = new ClaimsPrincipal(new ClaimsIdentity()); // Аноним
            _authenticationStateLoadedAttemptedInInteractiveMode = true; // Состояние сброшено, попытка (обновления) была

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }

    public class UserSession
    {
        public string? UserId { get; set; } // Сделал nullable на всякий случай, хотя после логина он должен быть
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? GroupName { get; set; }
        // Убрал 'internal set' для UserId, т.к. он устанавливается при создании UserSession
    }
}