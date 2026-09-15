using System.Net.Http;
using System.Net.Http.Json; // Для ReadFromJsonAsync и PostAsJsonAsync
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
// Добавьте using для ваших DTO (моделей запроса/ответа)

namespace KnowledgeTestingSystem.Web.Services
{
    // --- DTO для запроса на оценку ---
    public class EvaluationUpdateRequestDto // Назовем его так для ясности
    {
        [JsonPropertyName("userId")] // Атрибуты для соответствия JSON-ключам, если они отличаются от имен свойств
        public int UserId { get; set; }

        [JsonPropertyName("sessionId")]
        public int SessionId { get; set; }

        [JsonPropertyName("type")] // "0" для какого типа оценки? (обучающая, контрольная?)
        public int Type { get; set; } // Соответствует вашему TestMode enum?

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

    public class VariableSubmissionDto
    {
        [JsonPropertyName("sequence")] // Если sequence важен для сервиса оценки
        public int Sequence { get; set; } // Возможно, это DisplayOrder из ActiveStepInstance

        [JsonPropertyName("step")]
        public int Step { get; set; } // OriginalStepNumber из ActiveStepInstance

        [JsonPropertyName("variableName")]
        public string VariableName { get; set; } = string.Empty;

        [JsonPropertyName("variableValue")]
        public string VariableValue { get; set; } = string.Empty; // Сюда будем писать "1,2,3" или "1,2;3,4"
    }

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

    public class StepEvaluationResultDto
    {
        // public int TestId { get; set; } // Если ответ идет по тестам
        public int StepDisplayOrder { get; set; } // Или StepNumber, чтобы сопоставить с отправленным
        public bool IsCorrect { get; set; }
        public string? Feedback { get; set; }
        // public Dictionary<string, bool> VariableCorrectness { get; set; } // Если есть по переменным
    }


    public class EvaluationApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EvaluationApiService> _logger;

        public EvaluationApiService(IHttpClientFactory httpClientFactory, ILogger<EvaluationApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("EvaluationApiClient");
            _logger = logger;
        }

        // Имя метода теперь соответствует эндпоинту
        public async Task<EvaluationResponseDto?> UpdateEvaluationAsync(EvaluationUpdateRequestDto requestData)
        {
            try
            {
                _logger.LogInformation("Отправка запроса на оценку на /Evaluation/upload");
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(requestData);
                _logger.LogInformation("Отправляемый JSON: {JsonPayload}", jsonPayload);

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Evaluation/upload", requestData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<EvaluationResponseDto>();
                    jsonPayload = System.Text.Json.JsonSerializer.Serialize(result);
                    _logger.LogInformation(jsonPayload);
                    _logger.LogInformation("Оценка получена успешно. Score: {Score}, Errors Count: {ErrorsCount}", result?.Score, result?.Errors?.Count ?? 0);
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Ошибка при оценке. Статус: {StatusCode}, Ответ: {ErrorContent}", response.StatusCode, errorContent);
                    // Возвращаем объект с ошибками, если API вернул структурированную ошибку,
                    // или создаем новый с сообщением об ошибке HTTP.
                    // Попробуем распарсить errorContent как EvaluationResponseDto, если это возможно.
                    try
                    {
                        var errorResult = System.Text.Json.JsonSerializer.Deserialize<EvaluationResponseDto>(errorContent);
                        if (errorResult != null && (errorResult.Errors != null || errorResult.Score.HasValue)) // Если удалось распарсить в нашу структуру
                        {
                            return errorResult;
                        }
                    }
                    catch { /* Не удалось распарсить, значит это просто текстовая ошибка */ }

                    // Если не удалось распарсить, создаем "ручную" ошибку
                    return new EvaluationResponseDto
                    {
                        Errors = new List<ErrorDto> { new ErrorDto { Message = $"Ошибка сервера оценки: {response.ReasonPhrase} - {errorContent}" } }
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка HTTP-запроса к сервису оценивания.");
                return new EvaluationResponseDto { Errors = new List<ErrorDto> { new ErrorDto { Message = "Не удалось связаться с сервисом оценивания." } } };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Ошибка десериализации ответа от сервиса оценивания.");
                return new EvaluationResponseDto { Errors = new List<ErrorDto> { new ErrorDto { Message = "Некорректный формат ответа от сервиса оценивания." } } };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка при взаимодействии с сервисом оценивания.");
                return new EvaluationResponseDto { Errors = new List<ErrorDto> { new ErrorDto { Message = "Произошла непредвиденная ошибка." } } };
            }
        }
    }
}