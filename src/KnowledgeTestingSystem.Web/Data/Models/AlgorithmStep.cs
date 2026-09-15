namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class AlgorithmStep
{
    public int AlgoId { get; set; }
    public int AlgoStep { get; set; }
    public string Description { get; set; } = string.Empty;
    public float Difficult { get; set; }

    public virtual Algorithm Algo { get; set; } = null!;
    public virtual ICollection<TrackedVariable> TrackedVariables { get; set; } = new List<TrackedVariable>();
}
