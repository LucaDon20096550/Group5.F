using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Areas.Identity.Data;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MyContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MyContext>>()))
            {
                // Look for any movies.
                if (context.Roles.Any())
                {
                    return;   // DB has been seeded
                }

                context.Roles.AddRange(
                    new IdentityRole("Client"),
                    new IdentityRole("Caretaker"),
                    new IdentityRole("Employee"),
                    new IdentityRole("Administrator")
                );
                context.SaveChanges();
            }
        }
    }
}