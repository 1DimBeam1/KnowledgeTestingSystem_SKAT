namespace KnowledgeTestingSystem.Web.Data.Models;

public partial class Group
{
    public int GroupId { get; set; }
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
