using System.ComponentModel.DataAnnotations.Schema;

public class File
{
	public int Id { get; set; }
    public string FilePath { get; set; }
    [ForeignKey("User")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}