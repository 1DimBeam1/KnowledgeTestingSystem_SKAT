namespace KnowledgeTestingSystem.Web.Data.Models;

public class Mismatch
{
    public int Step { get; set; }
    public int LineNumber { get; set; }
    public string Expected { get; set; } = string.Empty;
    public string Actual { get; set; } = string.Empty;
}
