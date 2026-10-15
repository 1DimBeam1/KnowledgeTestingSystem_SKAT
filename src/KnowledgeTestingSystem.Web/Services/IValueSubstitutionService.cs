using InterpretatorService.Interfaces;
using KnowledgeTestingSystem.Web.Data.Models;

namespace KnowledgeTestingSystem.Web.Services;

/// <summary>
/// Результат выполнения substitute-values
/// </summary>
public class SubstituteValueResult
{
    public bool IsSuccessful { get; set; }
    public string? ErrorOutput { get; set; }
    public List<SubstitutedVariableValue> Values { get; set; } = new();
    public List<Mismatch> Mismatches { get; set; } = new();
    public List<int> ProgramSteps { get; set; } = new();
    public List<int> UserSteps { get; set; } = new();
}

/// <summary>
/// Заменённое значение переменной
/// </summary>
public class SubstitutedVariableValue
{
    public int Step { get; set; }
    public int TrackerHitId { get; set; }
    public string VariableName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Rank { get; set; }
    public string Value { get; set; } = string.Empty;
}

public interface IValueSubstitutionService
{
    Task<SubstituteValueResult> SubstituteValuesAsync(
        int codeId,
        int testId,
        List<(int Sequence, int LineNumber, string VariableName, string VariableValue)> userData);
}
