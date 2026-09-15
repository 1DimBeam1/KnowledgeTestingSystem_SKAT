namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class SolutionsByProgram
{
    public int SessionId { get; set; }
    public int TestId { get; set; }
    public int ProgramStep { get; set; }
    public int ProgramLineNumber { get; set; }
    public int OrderNumber { get; set; }
    public float StepDifficult { get; set; }

    public virtual Session Session { get; set; } = null!;
}
