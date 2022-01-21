using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public int ClientId { get; set; }
    public List<ApplicationUser> Guides { get; set; }
    public List<ApplicationUser> GuidedBy { get; set; }
    public List<Appointment> Appointments { get; set; }
    public OverviewFile OverviewFile { get; set; }
    public List<File> Files { get; set; }
    public List<Group> Groups { get; set; }
    public List<PrivateChat> PrivateChats { get; set; }
}