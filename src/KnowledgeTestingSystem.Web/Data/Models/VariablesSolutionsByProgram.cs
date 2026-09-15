namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class VariablesSolutionsByProgram
{
    public int ProgramStep { get; set; }
    public int ProgramLineNumber { get; set; }
    public int OrderNumber { get; set; }
    public int TestId { get; set; }
    public string VarName { get; set; } = string.Empty;
    public string VarValue { get; set; } = string.Empty;
}
