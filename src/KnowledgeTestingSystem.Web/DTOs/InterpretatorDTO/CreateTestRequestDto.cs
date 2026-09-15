using KnowledgeTestingSystem.Web.Data.Models;

namespace InterpretatorService.DTOs
{
    public class CreateTestRequestDto
    {
        public Test Test { get; set; }
        public List<TestInputData> InputData { get; set; }
    }
}
