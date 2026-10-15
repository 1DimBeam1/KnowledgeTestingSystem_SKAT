using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using InterpretatorService.Interfaces;
using KnowledgeTestingSystem.Web.Data;

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
        private readonly KnowledgeTestingDbContext _context;
        private readonly IInterpreterService _interpreterService;
        private readonly ILogger<AlgorithmApiService> _logger;

        private static readonly string StorageDirectory = Path.Combine(
            Directory.GetCurrentDirectory(), "code_files");

        public AlgorithmApiService(
            KnowledgeTestingDbContext context,
            IInterpreterService interpreterService,
            ILogger<AlgorithmApiService> logger)
        {
            _context = context;
            _interpreterService = interpreterService;
            _logger = logger;
        }

        // Метод для получения списка ID файлов (заменён с GET /api/code/list)
        public async Task<List<int>?> GetAlgorithmIdsAsync()
        {
            try
            {
                var ids = await _context.Algorithms.Select(a => a.AlgoId).ToListAsync();
                _logger.LogInformation($"Fetched {ids.Count} algorithm IDs");
                return ids;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting algorithm IDs");
                return null;
            }
        }

        // Метод для получения исходного кода (заменён с GET /api/code/{algoId}/source)
        public async Task<string?> GetAlgorithmSourceAsync(int algoId)
        {
            try
            {
                var codeFilePath = Path.Combine(StorageDirectory, $"{algoId}.cs");
                if (!System.IO.File.Exists(codeFilePath))
                {
                    _logger.LogWarning($"Source file not found for algoId {algoId}: {codeFilePath}");
                    return null;
                }
                var source = await System.IO.File.ReadAllTextAsync(codeFilePath);
                _logger.LogInformation($"Fetched source for algoId {algoId}, length: {source.Length}");
                return source;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting source for algoId {AlgoId}", algoId);
                return null;
            }
        }

        // получение списка алгоритмов (ID и Имя) (заменён с GET /api/code/algorithms)
        public async Task<List<AlgorithmListItemClientDto>?> GetAlgorithmsListAsync()
        {
            try
            {
                var algorithms = await _context.Algorithms
                    .OrderBy(a => a.AlgoName)
                    .Select(a => new AlgorithmListItemClientDto
                    {
                        AlgoId = a.AlgoId,
                        AlgorithmName = a.AlgoName
                    })
                    .ToListAsync();
                
                _logger.LogInformation($"Fetched {algorithms.Count} algorithms");
                return algorithms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting algorithms list");
                return null;
            }
        }

        public async Task<byte[]?> GetAlgorithmPictureAsync(int algoId)
        {
            try
            {
                var algorithm = await _context.Algorithms.FindAsync(algoId);
                if (algorithm == null || string.IsNullOrEmpty(algorithm.PicPath))
                {
                    _logger.LogWarning($"No picture found for algoId {algoId}");
                    return null;
                }

                var picturePath = algorithm.PicPath;
                if (!System.IO.File.Exists(picturePath))
                {
                    _logger.LogWarning($"Picture file not found for algoId {algoId}: {picturePath}");
                    return null;
                }

                var pictureBytes = await System.IO.File.ReadAllBytesAsync(picturePath);
                _logger.LogInformation($"Fetched picture for algoId {algoId}, size: {pictureBytes.Length} bytes");
                return pictureBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting picture for algoId {AlgoId}", algoId);
                return null;
            }
        }

        public async Task<ClientAlgorithmEditorBundleDto?> GetAlgorithmEditorBundleAsync(int algoId)
        {
            try
            {
                var algorithm = await _context.Algorithms
                    .Include(a => a.AlgorithmSteps)
                    .FirstOrDefaultAsync(a => a.AlgoId == algoId);
                
                if (algorithm == null)
                {
                    _logger.LogWarning($"Algorithm not found for editor bundle: {algoId}");
                    return null;
                }

                // Читаем исходный код из файла
                var codeFilePath = Path.Combine(StorageDirectory, $"{algoId}.cs");
                var codeContent = System.IO.File.Exists(codeFilePath)
                    ? await System.IO.File.ReadAllTextAsync(codeFilePath)
                    : string.Empty;

                // Формируем шаги из БД
                var configuredSteps = await _context.AlgorithmSteps
                    .Where(a => a.AlgoId == algoId)
                    .OrderBy(a => a.AlgoStep)
                    .Include(a => a.TrackedVariables)
                    .ToListAsync();

                var allConfiguredSteps = new List<ClientAlgorithmStepDetailDto>();
                foreach (var step in configuredSteps)
                {
                    var trackedVars = await _context.TrackedVariables
                        .Where(tv => tv.AlgoId == algoId && tv.AlgoStep == step.AlgoStep)
                        .OrderBy(tv => tv.Sequence)
                        .ToListAsync();

                    allConfiguredSteps.Add(new ClientAlgorithmStepDetailDto
                    {
                        StepNumber = step.AlgoStep,
                        Description = step.Description,
                        LineNumber = trackedVars.Any() ? trackedVars.First().LineNumber : 0,
                        Variables = trackedVars.Select(tv => tv.VarName).ToList(),
                        VarType = trackedVars.Any() ? trackedVars.First().VarType : null,
                        Difficult = step.Difficult
                    });
                }

                var result = new ClientAlgorithmEditorBundleDto
                {
                    AlgoId = algorithm.AlgoId,
                    AlgorithmName = algorithm.AlgoName,
                    PicPath = algorithm.PicPath,
                    CodeContent = codeContent,
                    AllConfiguredSteps = allConfiguredSteps
                };

                _logger.LogInformation($"Fetched editor bundle for algoId {algoId}, steps: {allConfiguredSteps.Count}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting editor bundle for algoId {AlgoId}", algoId);
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
            try
            {
                // Создаём алгоритм в БД
                var algorithm = new Data.Models.Algorithm
                {
                    AlgoName = name,
                    SrcPath = Path.Combine(StorageDirectory, $"{Guid.NewGuid()}.cs"),
                    PicPath = string.Empty
                };

                _context.Algorithms.Add(algorithm);
                await _context.SaveChangesAsync();

                int algoId = algorithm.AlgoId;

                // Сохраняем файл кода
                var codeFilePath = Path.Combine(StorageDirectory, $"{algoId}.cs");
                await System.IO.File.WriteAllTextAsync(codeFilePath, codeContent);
                algorithm.SrcPath = codeFilePath;

                // Сохраняем картинку, если передана
                if (imageFile != null)
                {
                    var imageFileName = $"{algoId}_{Guid.NewGuid()}{System.IO.Path.GetExtension(imageFile.Name)}";
                    var imageFilePath = Path.Combine(StorageDirectory, "images", imageFileName);
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(imageFilePath)!);
                    using var imageStream = imageFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
                    using var fileStream = new System.IO.FileStream(imageFilePath, System.IO.FileMode.Create);
                    await imageStream.CopyToAsync(fileStream);
                    algorithm.PicPath = imageFilePath;
                }

                // Сохраняем шаги и переменные
                int stepNumber = 0;
                
                // Входные переменные (StepNumber = 0 — это входные данные)
                foreach (var setup in clientInputSetups.Where(s => !string.IsNullOrWhiteSpace(s.VariableName)))
                {
                    stepNumber++;
                    var algoStep = new Data.Models.AlgorithmStep
                    {
                        AlgoId = algoId,
                        AlgoStep = stepNumber,
                        Description = setup.Description ?? "",
                        Difficult = 0.5f
                    };
                    _context.AlgorithmSteps.Add(algoStep);

                    var trackedVar = new Data.Models.TrackedVariable
                    {
                        AlgoId = algoId,
                        AlgoStep = stepNumber,
                        Sequence = stepNumber,
                        LineNumber = setup.LineNumberInCode,
                        VarName = setup.VariableName!,
                        VarType = string.Empty // Тип определяется при выполнении
                    };
                    _context.TrackedVariables.Add(trackedVar);
                }

                // Шаги отслеживания
                foreach (var step in clientTrackingSteps)
                {
                    stepNumber++;
                    var algoStep = new Data.Models.AlgorithmStep
                    {
                        AlgoId = algoId,
                        AlgoStep = step.StepNumber,
                        Description = step.Description,
                        Difficult = 0.5f
                    };
                    _context.AlgorithmSteps.Add(algoStep);

                    var variables = step.GetVariableList();
                    for (int i = 0; i < variables.Count; i++)
                    {
                        var trackedVar = new Data.Models.TrackedVariable
                        {
                            AlgoId = algoId,
                            AlgoStep = step.StepNumber,
                            Sequence = i + 1,
                            LineNumber = step.LineNumberInCode,
                            VarName = variables[i],
                            VarType = string.Empty
                        };
                        _context.TrackedVariables.Add(trackedVar);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created algorithm {algoId} with name '{name}', steps: {stepNumber}");
                return new ClientCreateAlgorithmResponseDto { AlgoId = algoId };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating algorithm {Name}", name);
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
            try
            {
                // Проверяем существование алгоритма (без отслеживания)
                var algorithmExists = await _context.Algorithms.AnyAsync(a => a.AlgoId == algoId);
                if (!algorithmExists)
                {
                    _logger.LogWarning($"Algorithm not found for update: {algoId}");
                    return false;
                }

                // Обновляем имя алгоритма (через ExecuteAsync для избежания отслеживания)
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE algorithms SET algo_name = {name}, src_path = '{Path.Combine(StorageDirectory, $"{algoId}.cs")}' WHERE algo_id = {algoId}");

                // Сохраняем файл кода
                var codeFilePath = Path.Combine(StorageDirectory, $"{algoId}.cs");
                await System.IO.File.WriteAllTextAsync(codeFilePath, codeContent);

                // Сохраняем картинку, если передана
                if (imageFile != null)
                {
                    var imageFileName = $"{algoId}_{Guid.NewGuid()}{System.IO.Path.GetExtension(imageFile.Name)}";
                    var imageFilePath = Path.Combine(StorageDirectory, "images", imageFileName);
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(imageFilePath)!);
                    using var imageStream = imageFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
                    using var fileStream = new System.IO.FileStream(imageFilePath, System.IO.FileMode.Create);
                    await imageStream.CopyToAsync(fileStream);

                    // Обновляем путь к картинке
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE algorithms SET pic_path = '{imageFilePath}' WHERE algo_id = {algoId}");
                }

                // Удаляем старые шаги и переменные (через SQL для избежания отслеживания)
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM trackedvariables WHERE algo_id = {algoId}");
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM algorithmsteps WHERE algo_id = {algoId}");

                // Сохраняем новые шаги через SQL (полностью избегаем EF Core tracking)
                int stepNumber = 0;
                
                // Входные переменные
                foreach (var setup in clientInputSetups.Where(s => !string.IsNullOrWhiteSpace(s.VariableName)))
                {
                    stepNumber++;
                    
                    // Вставка AlgorithmStep через SQL
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"INSERT INTO algorithmsteps (algo_id, algo_step, description, difficult) VALUES ({algoId}, {stepNumber}, '{setup.Description?.Replace("'", "''") ?? ""}', 0.5)");
                    
                    // Вставка TrackedVariable через SQL
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"INSERT INTO trackedvariables (sequence, line_number, var_type, var_name, algo_step, algo_id) VALUES ({stepNumber}, {setup.LineNumberInCode}, '', '{setup.VariableName?.Replace("'", "''")}', {stepNumber}, {algoId})");
                }

                // Шаги отслеживания
                foreach (var step in clientTrackingSteps)
                {
                    stepNumber++;
                    
                    // Вставка AlgorithmStep через SQL
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"INSERT INTO algorithmsteps (algo_id, algo_step, description, difficult) VALUES ({algoId}, {step.StepNumber}, '{step.Description?.Replace("'", "''")}', 0.5)");
                    
                    var variables = step.GetVariableList();
                    for (int i = 0; i < variables.Count; i++)
                    {
                        // Вставка TrackedVariable через SQL
                        await _context.Database.ExecuteSqlInterpolatedAsync(
                            $"INSERT INTO trackedvariables (sequence, line_number, var_type, var_name, algo_step, algo_id) VALUES ({i + 1}, {step.LineNumberInCode}, '', '{variables[i]?.Replace("'", "''")}', {step.StepNumber}, {algoId})");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating algorithm {AlgoId}", algoId);
                return false;
            }
        }

        // Удаление алгоритма (из TaskManagementEditorPage) (заменён с DELETE /api/code/{id})
        public async Task<bool> DeleteAlgorithmAsync(int algoId)
        {
            try
            {
                var algorithm = await _context.Algorithms.FindAsync(algoId);
                if (algorithm == null)
                {
                    _logger.LogWarning($"Algorithm not found for deletion: {algoId}");
                    return false;
                }

                // Удаляем связанные данные
                var steps = _context.AlgorithmSteps.Where(a => a.AlgoId == algoId);
                var vars = _context.TrackedVariables.Where(tv => tv.AlgoId == algoId);
                var tests = _context.Tests.Where(t => t.AlgoId == algoId);

                _context.TrackedVariables.RemoveRange(vars);
                _context.AlgorithmSteps.RemoveRange(steps);
                _context.Tests.RemoveRange(tests);
                _context.Algorithms.Remove(algorithm);

                await _context.SaveChangesAsync();

                // Удаляем файлы
                var codeFilePath = Path.Combine(StorageDirectory, $"{algoId}.cs");
                if (System.IO.File.Exists(codeFilePath))
                    System.IO.File.Delete(codeFilePath);

                if (!string.IsNullOrEmpty(algorithm.PicPath) && System.IO.File.Exists(algorithm.PicPath))
                    System.IO.File.Delete(algorithm.PicPath);

                _logger.LogInformation($"Deleted algorithm {algoId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting algorithm {AlgoId}", algoId);
                return false;
            }
        }

        public async Task<ClientAlgorithmModelDto?> GetAlgorithmInfoAsync(int algoId)
        {
            try
            {
                var algorithm = await _context.Algorithms.FindAsync(algoId);
                if (algorithm == null)
                {
                    _logger.LogWarning($"Algorithm not found for info: {algoId}");
                    return null;
                }

                var result = new ClientAlgorithmModelDto
                {
                    AlgoId = algorithm.AlgoId,
                    AlgoPath = algorithm.SrcPath,
                    PicPath = algorithm.PicPath,
                    AlgorithmName = algorithm.AlgoName
                };

                _logger.LogInformation($"Fetched info for algorithm {algoId}: {algorithm.AlgoName}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting algorithm info for {AlgoId}", algoId);
                return null;
            }
        }

        public async Task<List<ClientApiAlgoStepDto>?> GetApiAlgorithmStepsAsync(int algoId)
        {
            try
            {
                var steps = await _context.AlgorithmSteps
                    .Where(a => a.AlgoId == algoId)
                    .OrderBy(a => a.AlgoStep)
                    .Select(a => new ClientApiAlgoStepDto
                    {
                        Step = a.AlgoStep,
                        Description = a.Description,
                        Difficult = a.Difficult
                    })
                    .ToListAsync();

                _logger.LogInformation($"Fetched {steps.Count} steps for algoId {algoId}");
                return steps;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting API steps for algoId {AlgoId}", algoId);
                return null;
            }
        }

        public async Task<List<ClientApiTrackVariableDto>?> GetApiTrackedVariablesAsync(int algoId)
        {
            try
            {
                var variables = await _context.TrackedVariables
                    .Where(tv => tv.AlgoId == algoId)
                    .OrderBy(tv => tv.Sequence)
                    .Select(tv => new ClientApiTrackVariableDto
                    {
                        Sequence = tv.Sequence,
                        LineNumber = tv.LineNumber,
                        VarName = tv.VarName,
                        VarType = tv.VarType,
                        Step = tv.AlgoStep
                    })
                    .ToListAsync();

                _logger.LogInformation($"Fetched {variables.Count} tracked variables for algoId {algoId}");
                return variables;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting API variables for algoId {AlgoId}", algoId);
                return null;
            }
        }

        // Метод для проверки выполнения кода (заменён с POST /api/code/execute-temporary)
        public async Task<ClientCodeExecutionResponseDto?> ExecuteTemporaryCodeAsync(string codeContent, string language = "cs", int timeout = 20)
        {
            if (string.IsNullOrWhiteSpace(codeContent))
            {
                return new ClientCodeExecutionResponseDto { IsSuccessful = false, Error = "Код для выполнения пуст." };
            }

            try
            {
                // Создаем директорию если не существует
                if (!System.IO.Directory.Exists(StorageDirectory))
                    System.IO.Directory.CreateDirectory(StorageDirectory);

                // Сохраняем временный файл
                var tempFileName = $"temp_{Guid.NewGuid()}.cs";
                var tempFilePath = Path.Combine(StorageDirectory, tempFileName);
                await System.IO.File.WriteAllTextAsync(tempFilePath, codeContent);

                // Выполняем код через InterpreterService
                var codeModel = await _interpreterService.ExecuteCodeAsync(tempFilePath);

                // Удаляем временный файл
                try
                {
                    System.IO.File.Delete(tempFilePath);
                    System.IO.File.Delete(Path.Combine(StorageDirectory, $"{tempFileName}output.txt"));
                    System.IO.File.Delete(Path.Combine(StorageDirectory, $"{tempFileName}errors.txt"));
                    System.IO.File.Delete(Path.Combine(StorageDirectory, $"{tempFileName}warnings.txt"));
                }
                catch { /* Игнорируем ошибки удаления временных файлов */ }

                // Маппинг CodeModel → ClientCodeExecutionResponseDto
                return new ClientCodeExecutionResponseDto
                {
                    Output = codeModel.StandardOutput,
                    Error = codeModel.ErrorOutput,
                    Warning = codeModel.WarningOutput,
                    ExecutionTime = 0, // Время выполнения нужно считать отдельно
                    IsSuccessful = codeModel.IsSuccessful
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in temporary code execution");
                return new ClientCodeExecutionResponseDto { IsSuccessful = false, Error = $"Критическая ошибка при выполнении: {ex.Message}" };
            }
        }

        public async Task<List<TestDto>?> GetTestsListAsync()
        {
            try
            {
                var tests = await _context.Tests
                    .OrderBy(t => t.TestId)
                    .Select(t => new TestDto
                    {
                        TestId = t.TestId,
                        AlgoId = t.AlgoId,
                        Description = t.Description,
                        TestName = t.TestName,
                        Difficult = t.Difficult,
                        SolvedCount = t.SolvedCount,
                        UnsolvedCount = t.UnsolvedCount
                    })
                    .ToListAsync();

                _logger.LogInformation($"Fetched {tests.Count} tests");
                return tests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tests list");
                return null;
            }
        }

        public async Task<bool> DeleteTestAsync(int testId)
        {
            try
            {
                var test = await _context.Tests.FindAsync(testId);
                if (test == null)
                {
                    _logger.LogWarning($"Test not found for deletion: {testId}");
                    return false;
                }

                // Удаляем связанные данные
                var inputData = _context.TestInputData.Where(t => t.TestId == testId);
                var stepResponses = _context.TestStepResponses.Where(t => t.TestId == testId);
                
                _context.TestInputData.RemoveRange(inputData);
                _context.TestStepResponses.RemoveRange(stepResponses);
                _context.Tests.Remove(test);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Deleted test {testId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting test {TestId}", testId);
                return false;
            }
        }

        public async Task<TestDetailsDto?> GetTestDetailsAsync(int testId)
        {
            try
            {
                // Загружаем тест с связанными данными
                var test = await _context.Tests
                    .Include(t => t.Algo)
                    .FirstOrDefaultAsync(t => t.TestId == testId);

                if (test == null)
                {
                    _logger.LogWarning($"Test not found for details: {testId}");
                    return null;
                }

                // Загружаем входные данные теста
                var inputData = await _context.TestInputData
                    .Where(t => t.TestId == testId)
                    .ToListAsync();

                // Загружаем шаги алгоритма
                var algoSteps = await _context.AlgorithmSteps
                    .Where(a => a.AlgoId == test.AlgoId)
                    .OrderBy(a => a.AlgoStep)
                    .ToListAsync();

                var result = new TestDetailsDto
                {
                    Test = new TestDto
                    {
                        TestId = test.TestId,
                        AlgoId = test.AlgoId,
                        Description = test.Description,
                        TestName = test.TestName,
                        Difficult = test.Difficult,
                        SolvedCount = test.SolvedCount,
                        UnsolvedCount = test.UnsolvedCount
                    },
                    Algorithm = new ClientAlgorithmModelDto
                    {
                        AlgoId = test.Algo.AlgoId,
                        AlgoPath = test.Algo.SrcPath,
                        PicPath = test.Algo.PicPath,
                        AlgorithmName = test.Algo.AlgoName
                    },
                    InputTestData = inputData.Select(t => new InputTestDataDto
                    {
                        TestId = t.TestId,
                        VarName = t.VarName,
                        VarValue = t.VarValue,
                        VarType = t.VarType,
                        LineNumber = t.LineNumber
                    }).ToList(),
                    AlgoSteps = algoSteps.Select(a => new ClientApiAlgoStepDto
                    {
                        Step = a.AlgoStep,
                        Description = a.Description,
                        Difficult = a.Difficult
                    }).ToList()
                };

                _logger.LogInformation($"Fetched details for test {testId}: {test.TestName}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting test details for {TestId}", testId);
                return null;
            }
        }

        public async Task<TestDto?> CreateTestAsync(TestDto test, List<InputTestDataDto> inputData)
        {
            try
            {
                // Создаём тест в БД
                var testEntity = new Data.Models.Test
                {
                    AlgoId = test.AlgoId,
                    Description = test.Description ?? "",
                    TestName = test.TestName ?? "",
                    Difficult = test.Difficult,
                    SolvedCount = test.SolvedCount,
                    UnsolvedCount = test.UnsolvedCount
                };

                _context.Tests.Add(testEntity);
                await _context.SaveChangesAsync();

                int testId = testEntity.TestId;

                // Сохраняем входные данные
                if (inputData != null && inputData.Any())
                {
                    foreach (var input in inputData)
                    {
                        var testData = new Data.Models.TestInputData
                        {
                            TestId = testId,
                            VarName = input.VarName ?? "",
                            VarValue = input.VarValue ?? "",
                            VarType = input.VarType ?? "",
                            LineNumber = input.LineNumber
                        };
                        _context.TestInputData.Add(testData);
                    }
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation($"Created test {testId} for algoId {test.AlgoId}");
                return new TestDto
                {
                    TestId = testId,
                    AlgoId = test.AlgoId,
                    Description = test.Description,
                    TestName = test.TestName,
                    Difficult = test.Difficult,
                    SolvedCount = test.SolvedCount,
                    UnsolvedCount = test.UnsolvedCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test for algoId {AlgoId}", test?.AlgoId);
                return null;
            }
        }

        public async Task<bool> UpdateTestAsync(int testId, TestDto test, List<InputTestDataDto> inputData)
        {
            try
            {
                var testEntity = await _context.Tests.FindAsync(testId);
                if (testEntity == null)
                {
                    _logger.LogWarning($"Test not found for update: {testId}");
                    return false;
                }

                // Обновляем данные теста
                testEntity.AlgoId = test.AlgoId;
                testEntity.Description = test.Description ?? "";
                testEntity.TestName = test.TestName ?? "";
                testEntity.Difficult = test.Difficult;
                testEntity.SolvedCount = test.SolvedCount;
                testEntity.UnsolvedCount = test.UnsolvedCount;

                // Удаляем старые входные данные
                var existingInput = _context.TestInputData.Where(t => t.TestId == testId);
                _context.TestInputData.RemoveRange(existingInput);

                // Сохраняем новые входные данные
                if (inputData != null && inputData.Any())
                {
                    foreach (var input in inputData)
                    {
                        var testData = new Data.Models.TestInputData
                        {
                            TestId = testId,
                            VarName = input.VarName ?? "",
                            VarValue = input.VarValue ?? "",
                            VarType = input.VarType ?? "",
                            LineNumber = input.LineNumber
                        };
                        _context.TestInputData.Add(testData);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated test {testId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating test {TestId}", testId);
                return false;
            }
        }

        public async Task<int?> CreateTestWithValuesAsync(int algoId, List<VariableUpdateDto> updates)
        {
            try
            {
                // Проверка входных данных
                if (updates == null || !updates.Any())
                {
                    _logger.LogWarning("Empty updates list for CreateTestWithValuesAsync");
                    return null;
                }

                // Проверка уникальности VariableName
                var duplicateVarNames = updates.GroupBy(u => u.VariableName)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);
                if (duplicateVarNames.Any())
                {
                    _logger.LogWarning($"Duplicate variable names: {string.Join(", ", duplicateVarNames)}");
                    return null;
                }

                // Проверка существования алгоритма
                var algorithm = await _context.Algorithms.FindAsync(algoId);
                if (algorithm == null)
                {
                    _logger.LogWarning($"Algorithm not found for CreateTestWithValuesAsync: {algoId}");
                    return null;
                }

                var codeFilePath = Path.Combine(StorageDirectory, $"{algoId}.cs");
                if (!System.IO.File.Exists(codeFilePath))
                {
                    _logger.LogWarning($"Code file not found: {codeFilePath}");
                    return null;
                }

                var codeLines = await System.IO.File.ReadAllLinesAsync(codeFilePath);
                var typeMap = new Dictionary<string, (string Type, int Rank, int Line, int LineCount)>();

                // Определяем типы переменных
                for (int i = 0; i < codeLines.Length; i++)
                {
                    var line = codeLines[i].Trim();
                    if (line.Contains("=") && !line.StartsWith("//"))
                    {
                        var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 0)
                        {
                            var decl = parts[0].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (decl.Length >= 2)
                            {
                                var type = decl[0];
                                var name = decl[1].TrimEnd(';');
                                int rank = type.EndsWith("[]") ? 1 : type.EndsWith("[,]") ? 2 : 0;
                                type = type.Replace("[]", string.Empty).Replace("[,]", string.Empty);
                                int lineCount = 1;

                                if (rank > 0)
                                {
                                    int j = i;
                                    int braceCount = codeLines[i].Count(c => c == '{') - codeLines[i].Count(c => c == '}');
                                    bool foundSemicolon = codeLines[i].Trim().EndsWith(";");
                                    while (j < codeLines.Length && (!foundSemicolon || braceCount > 0))
                                    {
                                        j++;
                                        if (j < codeLines.Length)
                                        {
                                            braceCount += codeLines[j].Count(c => c == '{') - codeLines[j].Count(c => c == '}');
                                            foundSemicolon |= codeLines[j].Trim().EndsWith(";");
                                        }
                                    }
                                    lineCount = j - i + (foundSemicolon ? 1 : 0);
                                }

                                typeMap[name] = (type, rank, i + 1, lineCount);
                            }
                        }
                    }
                }

                // Копируем строки для модификации
                var newLines = codeLines.ToList();
                foreach (var updateGroup in updates.GroupBy(u => u.LineNumber))
                {
                    int lineNumber = updateGroup.Key;
                    if (lineNumber > codeLines.Length)
                    {
                        _logger.LogWarning($"Line {lineNumber} out of bounds");
                        return null;
                    }

                    foreach (var update in updateGroup)
                    {
                        if (!typeMap.TryGetValue(update.VariableName, out var typeInfo))
                        {
                            _logger.LogWarning($"Variable {update.VariableName} not found in code");
                            return null;
                        }

                        if (typeInfo.Line != lineNumber)
                        {
                            _logger.LogWarning($"Variable {update.VariableName} not initialized at line {lineNumber}");
                            return null;
                        }

                        string newValue;
                        if (typeInfo.Rank == 0)
                        {
                            if (typeInfo.Type == "string")
                                newValue = $"\"{update.Value}\"";
                            else if (typeInfo.Type == "char")
                                newValue = $"'{update.Value}'";
                            else
                                newValue = update.Value;
                        }
                        else if (typeInfo.Rank == 1)
                        {
                            newValue = $"{{ {update.Value} }}";
                        }
                        else
                        {
                            var rows = updateGroup.Where(u => u.VariableName == update.VariableName)
                                .Select(u => u.Value).ToList();
                            newValue = $"{{ {{ {string.Join("}, {", rows)} }} }}";
                        }

                        var originalLine = codeLines[typeInfo.Line - 1];
                        var assignIndex = originalLine.IndexOf('=');
                        if (assignIndex < 0)
                        {
                            _logger.LogWarning($"Assignment operator not found for {update.VariableName}");
                            return null;
                        }

                        newLines[typeInfo.Line - 1] = originalLine.Substring(0, assignIndex + 1) + $" {newValue};";

                        for (int i = 1; i < typeInfo.LineCount; i++)
                        {
                            if (typeInfo.Line - 1 + i < newLines.Count)
                                newLines[typeInfo.Line - 1 + i] = "";
                        }
                    }
                }

                while (newLines.Count < codeLines.Length)
                    newLines.Add("");

                // Начинаем транзакцию
                await using var transaction = await _context.Database.BeginTransactionAsync();

                // Сохраняем временный тестовый файл для проверки
                var tryExecuteFilePath = Path.Combine(StorageDirectory, $"test{algoId}_-1.cs");
                await System.IO.File.WriteAllTextAsync(tryExecuteFilePath, string.Join("\n", newLines));

                var codeModel = await _interpreterService.ExecuteCodeAsync(tryExecuteFilePath);

                var filesToDelete = new List<string>
                {
                    Path.Combine(StorageDirectory, $"test{algoId}_-1.cs"),
                    Path.Combine(StorageDirectory, $"test{algoId}_-1output.txt"),
                    Path.Combine(StorageDirectory, $"test{algoId}_-1errors.txt"),
                    Path.Combine(StorageDirectory, $"test{algoId}_-1warnings.txt"),
                };

                if (!codeModel.IsSuccessful)
                {
                    foreach (var file in filesToDelete)
                        if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
                    
                    _logger.LogWarning($"Test execution failed: {codeModel.ErrorOutput}");
                    return null;
                }

                try
                {
                    // Создаём запись в таблице tests
                    var test = new Data.Models.Test
                    {
                        AlgoId = algoId,
                        Description = "",
                        TestName = "",
                        Difficult = 0.5f,
                        SolvedCount = 0,
                        UnsolvedCount = 0
                    };

                    _context.Tests.Add(test);
                    await _context.SaveChangesAsync();

                    int testId = test.TestId;

                    // Создаём записи в таблице testinputdata
                    foreach (var update in updates)
                    {
                        if (!typeMap.TryGetValue(update.VariableName, out var typeInfo))
                            continue;
                        string type = typeInfo.Type + string.Concat(Enumerable.Repeat("[]", typeInfo.Rank));
                        var testData = new Data.Models.TestInputData
                        {
                            TestId = testId,
                            VarName = update.VariableName,
                            VarValue = update.Value,
                            VarType = type,
                            LineNumber = update.LineNumber
                        };
                        _context.TestInputData.Add(testData);
                    }

                    await _context.SaveChangesAsync();

                    // Сохраняем тестовый файл
                    var testFilePath = Path.Combine(StorageDirectory, $"test{algoId}_{testId}.cs");
                    await System.IO.File.WriteAllTextAsync(testFilePath, string.Join("\n", newLines));

                    // Фиксируем транзакцию
                    await transaction.CommitAsync();

                    // Удаляем временные файлы
                    foreach (var file in filesToDelete)
                        if (System.IO.File.Exists(file)) System.IO.File.Delete(file);

                    _logger.LogInformation($"Created test {testId} for algoId {algoId}");
                    return testId;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateTestWithValuesAsync for algoId {AlgoId}", algoId);
                return null;
            }
        }

        public async Task<bool> CreateHelperAsync(int testId, TestDto test)
        {
            try
            {
                var testEntity = await _context.Tests.FindAsync(testId);
                if (testEntity == null)
                {
                    _logger.LogWarning($"Test not found for CreateHelperAsync: {testId}");
                    return false;
                }

                // Обновляем данные теста
                testEntity.AlgoId = test.AlgoId;
                testEntity.Description = test.Description ?? "";
                testEntity.TestName = test.TestName ?? "";
                testEntity.Difficult = test.Difficult;
                testEntity.SolvedCount = test.SolvedCount;
                testEntity.UnsolvedCount = test.UnsolvedCount;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated test {testId} via CreateHelperAsync");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateHelperAsync for testId {TestId}", testId);
                return false;
            }
        }

        public async Task<bool> ModifyTestAsync(int codeId, int testId)
        {
            try
            {
                // Проверяем существование теста и алгоритма
                var test = await _context.Tests.FindAsync(testId);
                if (test == null)
                {
                    _logger.LogWarning($"Test not found for ModifyTestAsync: {testId}");
                    return false;
                }

                var algorithm = await _context.Algorithms.FindAsync(codeId);
                if (algorithm == null)
                {
                    _logger.LogWarning($"Algorithm not found for ModifyTestAsync: {codeId}");
                    return false;
                }

                // Проверяем существование модифицированного файла
                var modifiedCodePath = Path.Combine(StorageDirectory, $"test{codeId}_{testId}modified.cs");
                if (!System.IO.File.Exists(modifiedCodePath))
                {
                    _logger.LogWarning($"Modified code file not found: {modifiedCodePath}");
                    return false;
                }

                // Выполняем модифицированный код
                var codeModel = await _interpreterService.ExecuteCodeAsync(modifiedCodePath);
                
                if (!codeModel.IsSuccessful)
                {
                    _logger.LogWarning($"ModifyTestAsync execution failed for test {testId}: {codeModel.ErrorOutput}");
                    return false;
                }

                _logger.LogInformation($"Successfully modified test {testId} for codeId {codeId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ModifyTestAsync for codeId {CodeId}, testId {TestId}", codeId, testId);
                return false;
            }
        }

        // Добавьте сюда другие методы для взаимодействия с вашим API по мере необходимости
        // Например, для загрузки алгоритма, получения картинки и т.д.
        //}

        public async Task<string?> GetAlgorithmPictureUrlAsync(int algoId)
        {
            try
            {
                var algorithm = await _context.Algorithms.FindAsync(algoId);
                if (algorithm == null || string.IsNullOrEmpty(algorithm.PicPath))
                {
                    return null;
                }

                var picturePath = algorithm.PicPath;
                if (!System.IO.File.Exists(picturePath))
                {
                    _logger.LogWarning($"Picture file not found for algoId {algoId}: {picturePath}");
                    return null;
                }

                var imageBytes = await System.IO.File.ReadAllBytesAsync(picturePath);
                if (imageBytes == null || imageBytes.Length == 0)
                {
                    return null;
                }

                // Определяем тип контента по расширению файла
                string contentType;
                var extension = System.IO.Path.GetExtension(picturePath).ToLowerInvariant();
                switch (extension)
                {
                    case ".png": contentType = "image/png"; break;
                    case ".gif": contentType = "image/gif"; break;
                    case ".bmp": contentType = "image/bmp"; break;
                    case ".webp": contentType = "image/webp"; break;
                    default: contentType = "image/jpeg"; break;
                }

                string base64Image = Convert.ToBase64String(imageBytes);
                return $"data:{contentType};base64,{base64Image}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting picture URL for algoId {AlgoId}", algoId);
                return null;
            }
        }
    }
}
