namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class TestInputData
{
    public int TestId { get; set; }
    public string VarName { get; set; } = string.Empty;
    public string VarValue { get; set; } = string.Empty;
    public string VarType { get; set; } = string.Empty;
    public int LineNumber { get; set; }
}
