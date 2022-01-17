using System;
using System.Collections.Generic;

public class Group
{
	public int Id { get; set; }
    public string Name { get; set; }
    public GroupChat GroupChat { get; set; }
    public List<ApplicationUser> Users { get; set; }
    public string CreatedByName { get; set; }
    public DateTime CreatedOn { get; set; }
}