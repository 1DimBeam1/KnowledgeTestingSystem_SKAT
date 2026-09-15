namespace KnowledgeTestingSystem.Web.Data.Models;

public class MetaData
{
    public int TestId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float Difficult { get; set; }
}
