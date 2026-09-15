using Microsoft.EntityFrameworkCore;
using KnowledgeTestingSystem.Web.Data;
using KnowledgeTestingSystem.Web.Data.Models;
using System.Threading.Tasks;

namespace KnowledgeTestingSystem.Web.Services
{
    public class AuthService
    {
        private readonly KnowledgeTestingDbContext _context;

        public AuthService(KnowledgeTestingDbContext context)
        {
            _context = context;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == username);

            if (user != null)
            {
                if (user.Password == password)
                {
                    return user;
                }
            }
            return null;
        }
    }
}