namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class TrackedVariable
{
    public int Sequence { get; set; }
    public int LineNumber { get; set; }
    public string VarType { get; set; } = string.Empty;
    public string VarName { get; set; } = string.Empty;
    public int AlgoStep { get; set; }
    public int AlgoId { get; set; }

    public virtual AlgorithmStep? AlgorithmStep { get; set; }
}
