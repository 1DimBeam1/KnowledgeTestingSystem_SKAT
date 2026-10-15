using InterpretatorService.Interfaces;
using KnowledgeTestingSystem.Web.Data.Models;
using Microsoft.Extensions.Logging;

namespace KnowledgeTestingSystem.Web.Services;

public class ValueSubstitutionService : IValueSubstitutionService
{
    private static readonly string StorageDirectory = Path.Combine(
        Directory.GetCurrentDirectory(), "code_files");
    private readonly IInterpreterService _interpreterService;
    private readonly ILogger<ValueSubstitutionService> _logger;

    public ValueSubstitutionService(
        IInterpreterService interpreterService,
        ILogger<ValueSubstitutionService> logger)
    {
        _interpreterService = interpreterService;
        _logger = logger;
    }

    public async Task<SubstituteValueResult> SubstituteValuesAsync(
        int codeId,
        int testId,
        List<(int Sequence, int LineNumber, string VariableName, string VariableValue)> userData)
    {
        var result = new SubstituteValueResult();

        try
        {
            // Создаем директорию если не существует
            if (!System.IO.Directory.Exists(StorageDirectory))
                System.IO.Directory.CreateDirectory(StorageDirectory);

            // Формируем пути к файлам
            string userDataPath = Path.Combine(StorageDirectory, $"{codeId}_{testId}_userdata.txt");
            string valuesPath = Path.Combine(StorageDirectory, $"{codeId}_{testId}values.txt");
            string mismatchesPath = Path.Combine(StorageDirectory, $"{codeId}_{testId}mismatches.txt");
            string modifiedCodePath = Path.Combine(StorageDirectory, $"test{codeId}_{testId}modified.cs");

            // Проверяем существование модифицированного файла
            if (!System.IO.File.Exists(modifiedCodePath))
            {
                var error = $"Modified code file not found: {modifiedCodePath}";
                _logger.LogWarning(error);
                result.IsSuccessful = false;
                result.ErrorOutput = error;
                return result;
            }

            // Проверяем входные данные
            if (userData == null || !userData.Any())
            {
                var error = "User data is empty.";
                _logger.LogWarning(error);
                result.IsSuccessful = false;
                result.ErrorOutput = error;
                return result;
            }

            // Сохраняем пользовательские данные в файл
            var lines = userData.Select(u => $"{u.Sequence} {u.LineNumber} {u.VariableName} {u.VariableValue}");
            await System.IO.File.WriteAllLinesAsync(userDataPath, lines);

            // Создаём/очищаем файлы values.txt и mismatches.txt
            System.IO.File.WriteAllText(valuesPath, string.Empty);
            System.IO.File.WriteAllText(mismatchesPath, string.Empty);

            // Выполняем модифицированный код с ограничением по времени (15 секунд)
            var executionTask = _interpreterService.ExecuteCodeAsync(modifiedCodePath);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(15));
            var completedTask = await Task.WhenAny(executionTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                var error = "Code execution exceeded timeout (15 seconds).";
                _logger.LogWarning(error);
                result.IsSuccessful = false;
                result.ErrorOutput = error;
                return result;
            }

            var executionResult = await executionTask;

            // Проверяем успешность выполнения
            if (!executionResult.IsSuccessful)
            {
                var error = $"Code execution failed: {executionResult.ErrorOutput ?? "Unknown error"}";
                _logger.LogWarning(error);
                result.IsSuccessful = false;
                result.ErrorOutput = error;
                return result;
            }

            // Читаем values.txt
            var values = new List<SubstitutedVariableValue>();
            var programSteps = new List<int>();

            if (System.IO.File.Exists(valuesPath))
            {
                string[] valueLines = await System.IO.File.ReadAllLinesAsync(valuesPath);
                foreach (var line in valueLines)
                {
                    var parts = line.Split("//", StringSplitOptions.None);
                    if (parts.Length >= 6 &&
                        int.TryParse(parts[0], out int step) &&
                        int.TryParse(parts[4], out int rank) &&
                        int.TryParse(parts[1], out int trackerHitId))
                    {
                        values.Add(new SubstitutedVariableValue
                        {
                            Step = step,
                            TrackerHitId = trackerHitId,
                            VariableName = parts[2],
                            Type = parts[3],
                            Rank = rank,
                            Value = parts[5]
                        });
                        programSteps.Add(step);
                    }
                }
            }
            programSteps = programSteps.Distinct().ToList();

            // Читаем mismatches.txt
            var mismatches = new List<Mismatch>();
            if (System.IO.File.Exists(mismatchesPath))
            {
                string[] mismatchLines = await System.IO.File.ReadAllLinesAsync(mismatchesPath);
                foreach (var line in mismatchLines)
                {
                    var parts = line.Split("//", StringSplitOptions.None);
                    if (parts.Length >= 5 &&
                        int.TryParse(parts[0], out int step) &&
                        int.TryParse(parts[1], out int lineNumber) &&
                        int.TryParse(parts[2], out int trackerNumber))
                    {
                    mismatches.Add(new Mismatch
                    {
                        Step = step,
                        LineNumber = lineNumber,
                        Expected = parts.Length >= 6 ? parts[4] : (parts.Length >= 5 ? parts[3] : string.Empty),
                        Actual = parts.Length >= 6 ? parts[5] : (parts.Length >= 5 ? parts[4] : string.Empty)
                    });
                    }
                }
            }

            // Анализируем шаги
            var userSteps = userData.Select(u => u.Sequence).Distinct().OrderBy(s => s).ToList();

            result.IsSuccessful = true;
            result.Values = values;
            result.Mismatches = mismatches;
            result.ProgramSteps = programSteps;
            result.UserSteps = userSteps;

            _logger.LogInformation(
                "Substitute values completed for codeId={CodeId}, testId={TestId}: {ValueCount} values, {MismatchCount} mismatches",
                codeId, testId, values.Count, mismatches.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in substitute values for codeId={CodeId}, testId={TestId}", codeId, testId);
            result.IsSuccessful = false;
            result.ErrorOutput = ex.Message;
        }

        return result;
    }
}
