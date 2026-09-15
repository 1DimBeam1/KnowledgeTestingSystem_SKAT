using KnowledgeTestingSystem.Web.Data.Models;

namespace MarkSubsystem.DTO
{
    public class SubstituteValuesResponseDto
    {
        public CodeModel CodeModel { get; set; } = new();
        public List<VariableValue> Values { get; set; } = new();
        public List<Mismatch> Mismatches { get; set; } = new();
        public MetaData MetaData { get; set; } = new();
    }
}
