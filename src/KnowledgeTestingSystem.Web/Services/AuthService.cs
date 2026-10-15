using Microsoft.EntityFrameworkCore;
using KnowledgeTestingSystem.Web.Data;
using KnowledgeTestingSystem.Web.Data.Models;
using System.Threading.Tasks;

namespace KnowledgeTestingSystem.Web.Services
{
    public class AuthService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AuthService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KnowledgeTestingDbContext>();
            
            var user = await context.Users.FirstOrDefaultAsync(u => u.Login == username);

            if (user != null && user.Password == password)
                return user;
            
            return null;
        }
    }

}