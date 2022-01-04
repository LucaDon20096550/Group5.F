using System.ComponentModel.DataAnnotations.Schema;

public class GroupChat : Chat
{
    public string Description { get; set; }
    [ForeignKey("Group")]
    public int GroupId { get; set; }
    public Group Group { get; set; }
}