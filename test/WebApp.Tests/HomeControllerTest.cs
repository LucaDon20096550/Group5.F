using System;
using Xunit;
using WebApp.Controllers;
using WebApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Tests
{
    public class HomeControllerTest
    {
        private static int database = 0;
        private HomeController createController() {
            database++;
            
            return new HomeController(
                new MyContext(
                    new DbContextOptionsBuilder()
                    .UseInMemoryDatabase("TemporaryDatabase" + database)
                    .Options),
                new UserManager<ApplicationUser>());
        }

        [Fact]
        public void Test1()
        {
            createController();
        }
    }
}
