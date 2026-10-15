using Microsoft.EntityFrameworkCore;
using KnowledgeTestingSystem.Web.Data;
using KnowledgeTestingSystem.Web.Data.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static KnowledgeTestingSystem.Web.Shared.SessionListComponent;
using KnowledgeTestingSystem.Web.Shared;

namespace KnowledgeTestingSystem.Web.Services
{
    public class SessionService
    {
        private readonly KnowledgeTestingDbContext _dbContext;

        public SessionService(KnowledgeTestingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TestSessionView>> GetSessionsForUserAsync(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                return new List<TestSessionView>();
            }

            var userName = claimsPrincipal.Identity.Name;
            var user = await _dbContext.Users
                                 .Select(u => new { u.Login, u.Role, u.GroupId })
                                 .FirstOrDefaultAsync(u => u.Login == userName);

            if (user == null)
            {
                return new List<TestSessionView>();
            }

            IQueryable<Session> sessionsQuery;

            if (claimsPrincipal.IsInRole("Преподаватель"))
            {
                sessionsQuery = _dbContext.Sessions;
            }
            else if (claimsPrincipal.IsInRole("Обучающийся"))
            {
                var userGroupId = user.GroupId;
                sessionsQuery = _dbContext.Sessions
                    .Where(s => s.Groups.Any(g => g.GroupId == userGroupId));
            }
            else
            {
                return new List<TestSessionView>();
            }

            var sessionsRaw = await sessionsQuery
                                    .OrderByDescending(s => s.DateStart)
                                    .Select(s => new
                                    {
                                        s.SessionId,
                                        s.SessionType,
                                        s.DateStart,
                                        SessionStatus = DetermineSessionStatus(s)
                                    })
                                    .ToListAsync();

            var sessions = sessionsRaw.Select(s => new TestSessionView
            {
                Id = s.SessionId,
                Name = $"{s.SessionType?.ToString() ?? "Сессия"} от {s.DateStart:dd.MM.yyyy}",
                Status = s.SessionStatus
            }).ToList();
            return sessions;
        }

        private static SessionListComponent.SessionStatus DetermineSessionStatus(Session session)
        {
            var now = DateTime.UtcNow;
            if (now < session.DateStart)
            {
                return SessionListComponent.SessionStatus.NotStarted;
            }
            else if (now >= session.DateStart && now <= session.DateFinish)
            {
                return SessionListComponent.SessionStatus.InProgress;
            }
            else
            {
                return SessionListComponent.SessionStatus.Completed;
            }
        }
    }
}
