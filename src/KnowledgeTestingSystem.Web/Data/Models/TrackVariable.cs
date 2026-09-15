namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class TrackVariable
{
    public int Sequence { get; set; }
    public int AlgoId { get; set; }
    public int LineNumber { get; set; }
    public string VarType { get; set; } = null!;
    public string VarName { get; set; } = null!;
    public int Step { get; set; }

    public virtual AlgoStep AlgorithmStep { get; set; } = null!;
}
