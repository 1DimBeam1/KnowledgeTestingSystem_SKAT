namespace MarkSubsystem.DTO;

// DTO для входного запроса
public class EvaluationRequestDto
{
    public int UserId { get; set; }
    public int SessionId { get; set; }
    public int Type { get; set; }
    public List<TestSubmissionDto> Tests { get; set; } = new List<TestSubmissionDto>();
}

public class TestSubmissionDto
{
    public int TestId { get; set; }
    public List<VariableSubmissionDto> Variables { get; set; } = new List<VariableSubmissionDto>();
}

public class VariableSubmissionDto
{
    public int Sequence { get; set; }
    public int Step { get; set; }
    public string VariableName { get; set; } = string.Empty;
    public string VariableValue { get; set; } = string.Empty;
}

// DTO для ответа
public class EvaluationResponseDto
{
    public List<ErrorDto>? Errors { get; set; }
    public float? Score { get; set; }
}

public class ErrorDto
{
    public int? TestId { get; set; }
    public int? Sequence { get; set; }
    public int? Step { get; set; }
    public string? VariableName { get; set; }
    public string Message { get; set; } = string.Empty;
}

// DTO для ответа от getVariables
public class AlgoVariableDto
{
    public int Sequence { get; set; }
    public int LineNumber { get; set; }
    public string VarName { get; set; } = string.Empty;
    public string VarType { get; set; } = string.Empty;
    public int Step { get; set; }
}