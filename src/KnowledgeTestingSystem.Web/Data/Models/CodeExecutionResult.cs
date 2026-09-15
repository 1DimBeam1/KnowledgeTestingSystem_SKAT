namespace KnowledgeTestingSystem.Web.Data.Models;

public class CodeExecutionResult
{
    public bool IsSuccess { get; set; }
    public string Output { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public int ExitCode { get; set; }
}
