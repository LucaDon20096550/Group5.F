using System;
using System.Collections.Generic;

public class Appointment
{
	public int Id { get; set; }
    public DateTime Date { get; set; }
    public List<ApplicationUser> Users { get; set; }
}