namespace KnowledgeTestingSystem.Web.Data.Models;

public class CodeModel
{
    public int CodeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string AlgorithmName { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public string StandardOutput { get; set; } = string.Empty;
    public string ErrorOutput { get; set; } = string.Empty;
    public string WarningOutput { get; set; } = string.Empty;
    public string OutputFilePath { get; set; } = string.Empty;
    public string ErrorFilePath { get; set; } = string.Empty;
    public string WarningFilePath { get; set; } = string.Empty;
}
