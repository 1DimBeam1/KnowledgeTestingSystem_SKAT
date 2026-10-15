
using MarkSubsystem.DTO;

namespace KnowledgeTestingSystem.Web.Services;

public interface IEvaluationEngine
{
    Task<EvaluationResponseDto> EvaluateAsync(EvaluationRequestDto request);
}
