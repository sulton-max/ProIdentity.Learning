namespace IdentityStart.Models.Entities;

public class TodoItem
{
    public long Id { get; set; }
    public string Task { get; set; }
    public bool Complete { get; set; }
    public string OwnerId { get; set; }
}