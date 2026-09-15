namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class VariablesSolutionsByUser
{
    public int UserStep { get; set; }
    public int UserLineNumber { get; set; }
    public int OrderNumber { get; set; }
    public int TestId { get; set; }
    public string VarName { get; set; } = string.Empty;
    public string VarValue { get; set; } = string.Empty;
    public int UserId { get; set; }
}
