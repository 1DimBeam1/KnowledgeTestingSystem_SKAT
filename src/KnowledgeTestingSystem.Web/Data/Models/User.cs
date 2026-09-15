namespace KnowledgeTestingSystem.Web.Data.Models;

public enum UserRole { Student, Teacher, Administrator }
public enum SessionType { Exam, Practice }

public partial class User
{
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string IconPath { get; set; } = string.Empty;

    public virtual Group? Group { get; set; }
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public virtual ICollection<SolutionsByUser> SolutionsByUsers { get; set; } = new List<SolutionsByUser>();
}
