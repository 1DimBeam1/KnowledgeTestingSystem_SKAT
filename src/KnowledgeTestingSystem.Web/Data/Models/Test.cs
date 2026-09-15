namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class Test
{
    public int TestId { get; set; }
    public int AlgoId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public float Difficult { get; set; }
    public int SolvedCount { get; set; }
    public int UnsolvedCount { get; set; }

    public virtual Algorithm Algo { get; set; } = null!;
}
