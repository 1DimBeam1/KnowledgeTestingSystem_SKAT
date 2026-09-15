using System.Collections.Generic;

namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class AlgoStep
{
    public int AlgoId { get; set; }
    public int Step { get; set; }
    public float Difficult { get; set; } = 0.5f;
    public string Description { get; set; } = null!;

    public virtual Algorithm Algorithm { get; set; } = null!;
    public virtual ICollection<TrackVariable> TrackVariables { get; set; } = new List<TrackVariable>();
    public virtual ICollection<TestStepResponse> TestStepResponses { get; set; } = new List<TestStepResponse>();
}
