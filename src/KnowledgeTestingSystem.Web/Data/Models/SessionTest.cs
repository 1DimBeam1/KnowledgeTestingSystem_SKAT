namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class SessionTest
{
    public int SessionId { get; set; }
    public int TestId { get; set; }

    public virtual Session Session { get; set; } = null!;
}
