using KnowledgeTestingSystem.Web.Data.Models;

namespace InterpretatorService.Interfaces
{
    public interface IInterpreterService
    {
        Task<CodeModel> ExecuteCodeAsync(string codeFilePath);
    }
}