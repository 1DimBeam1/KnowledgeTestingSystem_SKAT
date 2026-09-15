using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization; // Для JsonPropertyName
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration; // Для IConfiguration
using Microsoft.Extensions.Logging; // Для логирования

namespace KnowledgeTestingSystem.Web.Services
{
    // Модель для шага отслеживания
    public class TrackingStepViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Для @key и удаления
        public int StepNumber { get; set; } // Порядковый номер шага (1, 2, 3...)
        public string Description { get; set; } = "";
        [Range(1, int.MaxValue, ErrorMessage = "Номер строки должен быть положительным")]
        public int LineNumberInCode { get; set; } = 1;
        [Required(ErrorMessage = "Укажите хотя бы одну переменную")]
        public string VariablesToTrackRaw { get; set; } = ""; // Строка "var1, var2, arr[i]"
        public List<string> GetVariableList() =>
            VariablesToTrackRaw?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>();
    }

    // Модель для входных переменных
    public class InputValueSetupViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Для @key
        [Range(1, int.MaxValue, ErrorMessage = "Номер строки должен быть > 0")]
        public int LineNumberInCode { get; set; } = 1;
        [Required(ErrorMessage = "Укажите имя переменной")]
        public string VariableName { get; set; } = ""; // Теперь одна переменная на строку для простоты UI
        public string? Description { get; set; } // Опциональное описание
    }

    // DTO для ответа от /api/code/list
    public class CodeFileIdsResponse
    {
        [JsonPropertyName("codeIds")]
        public List<int>? CodeIds { get; set; }
    }

    // DTO для элемента списка алгоритмов (должен соответствовать AlgorithmListItemDto из API)
    public class AlgorithmListItemClientDto
    {
        [JsonPropertyName("algoId")]
        public int AlgoId { get; set; }

        [JsonPropertyName("algorithmName")]
        public string? AlgorithmName { get; set; }
    }

    // DTO для ответа от /api/code/{algoId}/info (соответствует Models.Algorithm на API)
    public class ClientAlgorithmModelDto
    {
        [JsonPropertyName("algoId")]
        public int AlgoId { get; set; }

        [JsonPropertyName("algoPath")] // Имя поля в JSON от API
        public string? AlgoPath { get; set; }

        [JsonPropertyName("picPath")]  // Имя поля в JSON от API
        public string? PicPath { get; set; }

        [JsonPropertyName("algorithmName")] // Имя поля в JSON от API
        public string? AlgorithmName { get; set; }
    }

    // DTO для ответа от /api/code/steps/{algoId}
    public class ClientApiAlgoStepDto // Соответствует тому, что возвращает ваш /steps/{algoId}
    {
        [JsonPropertyName("step")] // Убедитесь, что имена свойств совпадают с JSON от API
        public int Step { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("difficult")]
        public float Difficult { get; set; }
    }

    // DTO для ответа от /api/code/getVariables/{algoId}
    public class ClientApiTrackVariableDto // Соответствует тому, что возвращает ваш /getVariables/{algoId}
    {
        [JsonPropertyName("sequence")]
        public int Sequence { get; set; }
        [JsonPropertyName("lineNumber")]
        public int LineNumber { get; set; }
        [JsonPropertyName("varName")]
        public string? VarName { get; set; }
        [JsonPropertyName("varType")]
        public string? VarType { get; set; }
        [JsonPropertyName("step")]
        public int Step { get; set; }
    }

    // DTO для запроса на временное выполнение (соответствует CodeRequestDto на API)
    public class ClientCodeExecutionRequestDto
    {
        public string Language { get; set; } = "cs";
        public string Code { get; set; }
        public int Timeout { get; set; } = 20; // Соответствует значению по умолчанию в CodeRequestDto
    }

    // DTO для ответа от временного выполнения (соответствует CodeResponseDto на API)
    public class ClientCodeExecutionResponseDto
    {
        [JsonPropertyName("output")]
        public string? Output { get; set; }
        [JsonPropertyName("error")]
        public string? Error { get; set; }
        [JsonPropertyName("warning")]
        public string? Warning { get; set; }
        [JsonPropertyName("executionTime")]
        public long ExecutionTime { get; set; }
        [JsonPropertyName("isSuccessful")]
        public bool IsSuccessful { get; set; }
    }

    // DTO для шага (входные данные или отслеживание), как для отправки, так и для получения.
    // Соответствует AlgorithmStepDetailDto на API.
    public class ClientAlgorithmStepDetailDto
    {
        [JsonPropertyName("stepNumber")]
        public int StepNumber { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("lineNumber")]
        public int LineNumber { get; set; }
        [JsonPropertyName("variables")]
        public List<string> Variables { get; set; } = new List<string>();
        [JsonPropertyName("varType")]
        public string? VarType { get; set; }
        [JsonPropertyName("difficult")]
        public float? Difficult { get; set; }
    }

    // DTO для полной информации об алгоритме для редактора
    public class ClientAlgorithmEditorBundleDto
    {
        [JsonPropertyName("algoId")]
        public int AlgoId { get; set; }
        [JsonPropertyName("algorithmName")]
        public string? AlgorithmName { get; set; }
        [JsonPropertyName("picPath")]
        public string? PicPath { get; set; }
        [JsonPropertyName("codeContent")]
        public string? CodeContent { get; set; }
        [JsonPropertyName("allConfiguredSteps")]
        public List<ClientAlgorithmStepDetailDto> AllConfiguredSteps { get; set; } = new List<ClientAlgorithmStepDetailDto>();
    }

    // DTO для ответа при создании
    public class ClientCreateAlgorithmResponseDto
    {
        [JsonPropertyName("algoId")]
        public int AlgoId { get; set; }
    }

    public class TestDto
    {
        [JsonPropertyName("algoId")]
        public int AlgoId { get; set; }

        [JsonPropertyName("testId")]
        public int TestId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("testName")]
        public string? TestName { get; set; }

        [JsonPropertyName("difficult")]
        public float Difficult { get; set; } = 0.5f;

        [JsonPropertyName("solvedCount")]
        public int SolvedCount { get; set; } = 0;

        [JsonPropertyName("unsolvedCount")]
        public int UnsolvedCount { get; set; } = 0;
    }

    public class InputTestDataDto
    {
        [JsonPropertyName("testId")]
        public int TestId { get; set; }

        [JsonPropertyName("varName")]
        public string? VarName { get; set; }

        [JsonPropertyName("varValue")]
        public string? VarValue { get; set; }

        [JsonPropertyName("varType")]
        public string? VarType { get; set; }

        [JsonPropertyName("lineNumber")]
        public int LineNumber { get; set; }
    }

    public class TestDetailsDto
    {
        [JsonPropertyName("test")]
        public TestDto Test { get; set; }

        [JsonPropertyName("algorithm")]
        public ClientAlgorithmModelDto Algorithm { get; set; }

        [JsonPropertyName("inputTestData")]
        public List<InputTestDataDto> InputTestData { get; set; }

        [JsonPropertyName("algoSteps")]
        public List<ClientApiAlgoStepDto> AlgoSteps { get; set; }
    }

    public class VariableUpdateDto
    {
        public int LineNumber { get; set; }
        public string VariableName { get; set; }
        public string Value { get; set; }
    }

    public class CreateTestResponseDto
    {
        public int TestId { get; set; }
    }

    public class AlgorithmApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public AlgorithmApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Получаем URL API из appsettings.json
            _apiBaseUrl = configuration["ApiSettings:InterpretatorServiceUrl"]; // Порт Docker
        }

        // Метод для получения списка ID файлов (соответствует GET /api/code/list)
        // Можно в будущем использовать для проверки наличия файлов с кодом, т.к. он проверяет по директории
        public async Task<List<int>?> GetAlgorithmIdsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/code/list");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CodeFileIdsResponse>();
                    return result?.CodeIds;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error getting algorithm IDs: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAlgorithmIdsAsync: {ex.Message}");
                return null;
            }
        }

        // Метод для получения исходного кода (соответствует GET /api/code/{algoId}/source)
        public async Task<string?> GetAlgorithmSourceAsync(int algoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/code/{algoId}/source");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error getting source for algoId {algoId}: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAlgorithmSourceAsync for algoId {algoId}: {ex.Message}");
                return null;
            }
        }

        // получение списка алгоритмов (ID и Имя)
        public async Task<List<AlgorithmListItemClientDto>?> GetAlgorithmsListAsync()
        {
            try
            {
                //Console.WriteLine($"AlgorithmApiService: Requesting {_apiBaseUrl}/api/code/algorithms");
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/code/algorithms");

                if (response.IsSuccessStatusCode)
                {
                    var algorithms = await response.Content.ReadFromJsonAsync<List<AlgorithmListItemClientDto>>();
                    //Console.WriteLine($"AlgorithmApiService: Received {algorithms?.Count ?? 0} algorithms.");
                    return algorithms;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"AlgorithmApiService: Error getting algorithms list: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AlgorithmApiService: Exception in GetAlgorithmsListAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]?> GetAlgorithmPictureAsync(int algoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/code/{algoId}/picture");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error getting picture for algoId {algoId}: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAlgorithmPictureAsync for algoId {algoId}: {ex.Message}");
                return null;
            }
        }

        public async Task<ClientAlgorithmEditorBundleDto?> GetAlgorithmEditorBundleAsync(int algoId)
        {
            try
            {
                //Console.WriteLine($"AlgorithmApiService: Requesting editor bundle for algoId {algoId}");
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/code/{algoId}/editor_bundle");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ClientAlgorithmEditorBundleDto>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"AlgorithmApiService: Error getting editor bundle for {algoId}: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AlgorithmApiService: Exception in GetAlgorithmEditorBundleAsync for {algoId}: {ex.Message}");
                return null;
            }
        }


        public async Task<ClientCreateAlgorithmResponseDto?> CreateAlgorithmAsync(
    string name,
    string codeContent,
    List<InputValueSetupViewModel> clientInputSetups,
    List<TrackingStepViewModel> clientTrackingSteps,
    IBrowserFile? imageFile)
        {
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(name), "AlgorithmName");
            formData.Add(new StringContent(codeContent), "CodeContent");

            var allStepsForApi = new List<ClientAlgorithmStepDetailDto>();
            foreach (var setup in clientInputSetups.Where(s => !string.IsNullOrWhiteSpace(s.VariableName)))
            {
                allStepsForApi.Add(new ClientAlgorithmStepDetailDto
                {
                    StepNumber = 0,
                    Description = setup.Description ?? "",
                    LineNumber = setup.LineNumberInCode,
                    Variables = new List<string> { setup.VariableName }
                });
            }
            foreach (var step in clientTrackingSteps)
            {
                allStepsForApi.Add(new ClientAlgorithmStepDetailDto
                {
                    StepNumber = step.StepNumber,
                    Description = step.Description,
                    LineNumber = step.LineNumberInCode,
                    Variables = step.GetVariableList()
                });
            }
            if (allStepsForApi.Any())
            {
                formData.Add(new StringContent(JsonSerializer.Serialize(allStepsForApi)), "AllStepsJson");
            }

            if (imageFile != null)
            {
                var imageStreamContent = new StreamContent(imageFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024));
                imageStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
                formData.Add(imageStreamContent, "ImageFile", imageFile.Name);
            }

            try
            {
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/code/create_algorithm", formData);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ClientCreateAlgorithmResponseDto>();
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error creating algorithm: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateAlgorithmAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateAlgorithmAsync(
    int algoId,
    string name,
    string codeContent,
    List<InputValueSetupViewModel> clientInputSetups,
    List<TrackingStepViewModel> clientTrackingSteps,
    IBrowserFile? imageFile)
        {
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(name), "AlgorithmName");
            formData.Add(new StringContent(codeContent), "CodeContent");

            var allStepsForApi = new List<ClientAlgorithmStepDetailDto>();
            foreach (var setup in clientInputSetups.Where(s => !string.IsNullOrWhiteSpace(s.VariableName)))
            {
                allStepsForApi.Add(new ClientAlgorithmStepDetailDto
                {
                    StepNumber = 0,
                    Description = setup.Description ?? "",
                    LineNumber = setup.LineNumberInCode,
                    Variables = new List<string> { setup.VariableName }
                });
            }
            foreach (var step in clientTrackingSteps)
            {
                allStepsForApi.Add(new ClientAlgorithmStepDetailDto
                {
                    StepNumber = step.StepNumber,
                    Description = step.Description,
                    LineNumber = step.LineNumberInCode,
                    Variables = step.GetVariableList()
                });
            }
            if (allStepsForApi.Any())
            {
                formData.Add(new StringContent(JsonSerializer.Serialize(allStepsForApi)), "AllStepsJson");
            }

            if (imageFile != null)
            {
                var imageStreamContent = new StreamContent(imageFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024));
                imageStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
                formData.Add(imageStreamContent, "ImageFile", imageFile.Name);
            }

            try
            {
                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/code/{algoId}/update_algorithm", formData);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UpdateAlgorithmAsync for algoId {algoId}: {ex.Message}");
                return false;
            }
        }

        // Удаление алгоритма (из TaskManagementEditorPage)
        public async Task<bool> DeleteAlgorithmAsync(int algoId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/code/{algoId}");
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error deleting algorithm {algoId}: {response.StatusCode} - {errorContent}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in DeleteAlgorithmAsync for algoId {algoId}: {ex.Message}");
                return false;
            }
        }

        public async Task<ClientAlgorithmModelDto?> GetAlgorithmInfoAsync(int algoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/code/{algoId}/info");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ClientAlgorithmModelDto>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error getting algorithm info for {algoId}: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAlgorithmInfoAsync for {algoId}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<ClientApiAlgoStepDto>?> GetApiAlgorithmStepsAsync(int algoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/code/steps/{algoId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ClientApiAlgoStepDto>>();
                }
                else
                {
                    Console.WriteLine($"Error getting API steps for {algoId}: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetApiAlgorithmStepsAsync for {algoId}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<ClientApiTrackVariableDto>?> GetApiTrackedVariablesAsync(int algoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/code/getVariables/{algoId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ClientApiTrackVariableDto>>();
                }
                else
                {
                    Console.WriteLine($"Error getting API variables for {algoId}: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetApiTrackedVariablesAsync for {algoId}: {ex.Message}");
                return null;
            }
        }

        // Метод для проверки выполнения кода
        public async Task<ClientCodeExecutionResponseDto?> ExecuteTemporaryCodeAsync(string codeContent, string language = "cs", int timeout = 20)
        {
            if (string.IsNullOrWhiteSpace(codeContent))
            {
                return new ClientCodeExecutionResponseDto { IsSuccessful = false, Error = "Код для выполнения пуст." };
            }
            //Console.WriteLine($"AlgorithmApiService: Sending code for temporary execution. Lang: {language}, Code: {codeContent}, Timeout: {timeout}");
            try
            {
                var request = new ClientCodeExecutionRequestDto
                {
                    Language = language,
                    Code = codeContent,
                    Timeout = timeout
                };

                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/code/execute-temporary", request);

                // Пытаемся десериализовать в любом случае, т.к. API может вернуть CodeResponseDto даже при ошибке (например, BadRequest с телом)
                ClientCodeExecutionResponseDto? executionResult = null;
                if (response.Content != null && response.Content.Headers.ContentLength > 0)
                {
                    try
                    {
                        executionResult = await response.Content.ReadFromJsonAsync<ClientCodeExecutionResponseDto>();
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"AlgorithmApiService: Error deserializing temporary execution response: {jsonEx.Message}");
                        // Если не удалось распарсить, но статус ошибки, создаем DTO с ошибкой
                        if (!response.IsSuccessStatusCode)
                        {
                            var errorBody = await response.Content.ReadAsStringAsync();
                            return new ClientCodeExecutionResponseDto { IsSuccessful = false, Error = $"Ошибка сервера ({response.StatusCode}): {errorBody}" };
                        }
                    }
                }

                if (response.IsSuccessStatusCode && executionResult != null)
                {
                    //Console.WriteLine($"AlgorithmApiService: Temporary execution API call successful. Result - IsSuccessful: {executionResult.IsSuccessful}, Output: '{executionResult.Output}', Error: '{executionResult.Error}'");
                    return executionResult;
                }
                else
                {
                    // Если статус не успешный, но мы смогли распарсить тело ошибки
                    if (executionResult != null && !executionResult.IsSuccessful)
                    {
                        Console.WriteLine($"AlgorithmApiService: Temporary execution API call failed (parsed error). Status: {response.StatusCode}, Error: {executionResult.Error}");
                        return executionResult;
                    }
                    // Общая ошибка, если не удалось распарсить или нет тела
                    var errorContent = executionResult?.Error ?? await response.Content?.ReadAsStringAsync() ?? "Неизвестная ошибка";
                    Console.WriteLine($"AlgorithmApiService: Error in temporary execution. Status: {response.StatusCode}, Content: {errorContent}");
                    return new ClientCodeExecutionResponseDto { IsSuccessful = false, Error = $"Ошибка сервера ({response.StatusCode}): {errorContent}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AlgorithmApiService: Exception in ExecuteTemporaryCodeAsync: {ex.Message}");
                return new ClientCodeExecutionResponseDto { IsSuccessful = false, Error = $"Критическая ошибка при отправке на выполнение: {ex.Message}" };
            }
        }

        public async Task<List<TestDto>?> GetTestsListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/testmanagement/fetch-tests");
                if (response.IsSuccessStatusCode)
                {
                    var tests = await response.Content.ReadFromJsonAsync<List<TestDto>>();
                    return tests;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error getting tests list: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetTestsListAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteTestAsync(int testId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/testmanagement/delete-test/{testId}");
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error deleting test {testId}: {response.StatusCode} - {errorContent}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in DeleteTestAsync for testId {testId}: {ex.Message}");
                return false;
            }
        }

        public async Task<TestDetailsDto?> GetTestDetailsAsync(int testId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/testmanagement/fetch-test-details/{testId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TestDetailsDto>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error getting test details for {testId}: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetTestDetailsAsync for {testId}: {ex.Message}");
                return null;
            }
        }

        public async Task<TestDto?> CreateTestAsync(TestDto test, List<InputTestDataDto> inputData)
        {
            try
            {
                var requestBody = new { test, inputData };
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/testmanagement/create-test", requestBody);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TestDto>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error creating test: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateTestAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateTestAsync(int testId, TestDto test, List<InputTestDataDto> inputData)
        {
            try
            {
                var requestBody = new { test, inputData };
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/testmanagement/update-test/{testId}", requestBody);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error updating test {testId}: {response.StatusCode} - {errorContent}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UpdateTestAsync for {testId}: {ex.Message}");
                return false;
            }
        }

        public async Task<int?> CreateTestWithValuesAsync(int algoId, List<VariableUpdateDto> updates)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/code/update-values/{algoId}", updates);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CreateTestResponseDto>();
                    return result?.TestId;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error creating test with values: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateTestWithValuesAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateHelperAsync(int testId, TestDto test)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/testmanagement/create-helper/{testId}", test);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error updating test with create-helper {testId}: {response.StatusCode} - {errorContent}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateHelperAsync for testId {testId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ModifyTestAsync(int codeId, int testId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/code/modify-test/{codeId}/{testId}", null);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error modifying test {testId} for codeId {codeId}: {response.StatusCode} - {errorContent}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in ModifyTestAsync for codeId {codeId}, testId {testId}: {ex.Message}");
                return false;
            }
        }

        // Добавьте сюда другие методы для взаимодействия с вашим API по мере необходимости
        // Например, для загрузки алгоритма, получения картинки и т.д.
        //}

        public async Task<string?> GetAlgorithmPictureUrlAsync(int algoId)
        {
            //string requestUri = $"Code/{algoId}/picture";

            try
            {
                //HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Code/{algoId}/picture");

                if (response.IsSuccessStatusCode)
                {
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        // Определяем тип контента для правильного data URL
                        string? contentType = response.Content.Headers.ContentType?.MediaType;
                        if (string.IsNullOrEmpty(contentType))
                        {
                            // Пытаемся угадать или установить по умолчанию (например, image/png)
                            // Это важно, чтобы браузер правильно отобразил base64
                            contentType = "image/jpeg"; // Или image/jpeg, image/gif и т.д.
                        }
                        string base64Image = Convert.ToBase64String(imageBytes);
                        return $"data:{contentType};base64,{base64Image}";
                    }
                    return null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}



        //public class AlgorithmApiService
        //{
        //    private readonly HttpClient _httpClient;
        //    private readonly ILogger<AlgorithmApiService> _logger;

        //    // Внедряем IHttpClientFactory для получения именованного клиента
        //    public AlgorithmApiService(IHttpClientFactory httpClientFactory, ILogger<AlgorithmApiService> logger)
        //    {
        //        // Получаем HttpClient, настроенный в Program.cs с именем "ApiSettings"
        //        _httpClient = httpClientFactory.CreateClient("ApiSettings");
        //        _logger = logger;
        //    }

        //    // Метод для получения изображения (или пути к нему)
        //    // Возвращает string, который может быть URL или base64 строкой изображения.
        //    // Если API возвращает само изображение (байты), то тип возврата будет byte[]
        //    // или мы можем сразу преобразовать его в base64 строку.
        //    public async Task<string?> GetAlgorithmPictureUrlAsync(int algoId)
        //    {
        //        string requestUri = $"api/Code/{algoId}/picture";
        //        _logger.LogInformation("Запрос на получение изображения алгоритма: {RequestUri}", requestUri);

        //        try
        //        {
        //            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                // Вариант 1: Если API возвращает URL изображения в теле ответа (например, JSON {"imageUrl": "..."})
        //                // var result = await response.Content.ReadFromJsonAsync<AlgorithmPictureResponseDto>();
        //                // return result?.ImageUrl;

        //                // Вариант 2: Если API возвращает URL изображения как простую строку в теле
        //                // string imageUrl = await response.Content.ReadAsStringAsync();
        //                // return imageUrl.Trim('"'); // Убрать кавычки, если они есть

        //                // Вариант 3: Если API возвращает само изображение (байты)
        //                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
        //                if (imageBytes != null && imageBytes.Length > 0)
        //                {
        //                    // Определяем тип контента для правильного data URL
        //                    string? contentType = response.Content.Headers.ContentType?.MediaType;
        //                    if (string.IsNullOrEmpty(contentType))
        //                    {
        //                        // Пытаемся угадать или установить по умолчанию (например, image/png)
        //                        // Это важно, чтобы браузер правильно отобразил base64
        //                        contentType = "image/jpeg"; // Или image/jpeg, image/gif и т.д.
        //                        _logger.LogWarning("Content-Type для изображения алгоритма {AlgoId} не определен, используется по умолчанию: {ContentType}", algoId, contentType);
        //                    }
        //                    string base64Image = Convert.ToBase64String(imageBytes);
        //                    return $"data:{contentType};base64,{base64Image}";
        //                }
        //                _logger.LogWarning("Получено пустое тело ответа для изображения алгоритма {AlgoId}", algoId);
        //                return null;
        //            }
        //            else
        //            {
        //                var errorContent = await response.Content.ReadAsStringAsync();
        //                _logger.LogError("Ошибка при получении изображения для алгоритма {AlgoId}. Статус: {StatusCode}, Ответ: {ErrorContent}",
        //                    algoId, response.StatusCode, errorContent);
        //                return null;
        //            }
        //        }
        //        catch (HttpRequestException ex)
        //        {
        //            _logger.LogError(ex, "Ошибка HTTP-запроса при получении изображения для алгоритма {AlgoId}", algoId);
        //            return null;
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Непредвиденная ошибка при получении изображения для алгоритма {AlgoId}", algoId);
        //            return null;
        //        }
        //    }
