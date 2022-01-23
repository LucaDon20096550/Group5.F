using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Areas.Identity.Data
{
    public class MyContext : IdentityDbContext<ApplicationUser>
    {
        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {
        }
        
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<OverviewFile> OverviewFiles { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Setting up keys
            builder.Entity<Appointment>()
                .HasKey(a => a.Id);

            builder.Entity<OverviewFile>()
                .HasKey(f => f.Id);

            builder.Entity<File>()
                .HasKey(f => f.Id);

            builder.Entity<Group>()
                .HasKey(g => g.Id);

            builder.Entity<Message>()
                .HasKey(m => m.Id);
            

            // Setting up relations
            builder.Entity<ApplicationUser>()
                .HasMany(c => c.Appointments)
                .WithMany(a => a.Users);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.OverviewFile)
                .WithOne(f => f.User);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Files)
                .WithOne(f => f.User);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Guides);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Groups)
                .WithMany(g => g.Users);

            builder.Entity<Group>()
                .HasOne(g => g.GroupChat)
                .WithOne(c => c.Group);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.PrivateChats)
                .WithMany(c => c.Users);

            builder.Entity<Chat>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Chat);

            builder.Entity<Message>()
                .HasOne(m => m.Sender);
        }
    }
}
