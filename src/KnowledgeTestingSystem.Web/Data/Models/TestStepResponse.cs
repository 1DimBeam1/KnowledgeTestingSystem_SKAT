namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class TestStepResponse
{
    public int TestId { get; set; }
    public int AlgoId { get; set; }
    public int AlgoStep { get; set; }
    public int CorrectCount { get; set; }
    public int IncorrectCount { get; set; }
}
