namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class SolutionsByUser
{
    public int SessionId { get; set; }
    public int UserId { get; set; }
    public int UserStep { get; set; }
    public int UserLineNumber { get; set; }
    public int OrderNumber { get; set; }
    public int TestId { get; set; }
    public float StepDifficult { get; set; }

    public virtual Session Session { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
