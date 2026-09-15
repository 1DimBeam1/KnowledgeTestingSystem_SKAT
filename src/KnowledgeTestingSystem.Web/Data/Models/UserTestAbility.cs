namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class UserTestAbility
{
    public int UserId { get; set; }
    public int TestId { get; set; }
    public float Ability { get; set; } = 0.0f;
}
