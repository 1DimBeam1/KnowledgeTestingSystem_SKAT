namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class Algorithm
{
    public int AlgoId { get; set; }
    public string SrcPath { get; set; } = string.Empty;
    public string PicPath { get; set; } = string.Empty;
    public string AlgoName { get; set; } = string.Empty;

    public virtual ICollection<AlgorithmStep>? AlgorithmSteps { get; set; }
    public virtual ICollection<Test>? Tests { get; set; }
}
