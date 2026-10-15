using MarkSubsystem.DTO;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace KnowledgeTestingSystem.Web.Services;

public class EvaluationUpdateRequestDto
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("sessionId")]
    public int SessionId { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("tests")]
    public List<TestAttemptDto> Tests { get; set; } = new List<TestAttemptDto>();
}

public class TestAttemptDto
{
    [JsonPropertyName("testId")]
    public int TestId { get; set; }

    [JsonPropertyName("variables")]
    public List<VariableSubmissionDto> Variables { get; set; } = new List<VariableSubmissionDto>();
}

public class EvaluationApiService
{
    private readonly IEvaluationEngine _engine;
    private readonly ILogger<EvaluationApiService> _logger;

    public EvaluationApiService(IEvaluationEngine engine, ILogger<EvaluationApiService> logger)
    {
        _engine = engine;
        _logger = logger;
    }

    public async Task<EvaluationResponseDto?> UpdateEvaluationAsync(EvaluationUpdateRequestDto requestData)
    {
        try
        {
            _logger.LogInformation("Отправка запроса на оценку для userId={UserId}, sessionId={SessionId}",
                requestData.UserId, requestData.SessionId);

            var request = new EvaluationRequestDto
            {
                UserId = requestData.UserId,
                SessionId = requestData.SessionId,
                Type = requestData.Type,
                Tests = requestData.Tests.Select(t => new TestSubmissionDto
                {
                    TestId = t.TestId,
                    Variables = t.Variables.Select(v => new MarkSubsystem.DTO.VariableSubmissionDto
                    {
                        Sequence = v.Sequence,
                        Step = v.Step,
                        VariableName = v.VariableName,
                        VariableValue = v.VariableValue
                    }).ToList()
                }).ToList()
            };

            var result = await _engine.EvaluateAsync(request);

            _logger.LogInformation("Оценка получена. Score: {Score}, Errors Count: {ErrorsCount}",
                result?.Score, result?.Errors?.Count ?? 0);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при оценке для userId={UserId}, sessionId={SessionId}",
                requestData.UserId, requestData.SessionId);
            return new EvaluationResponseDto
            {
                Errors = new List<ErrorDto>
                {
                    new() { Message = $"Произошла непредвиденная ошибка: {ex.Message}" }
                }
            };
        }
    }
}
