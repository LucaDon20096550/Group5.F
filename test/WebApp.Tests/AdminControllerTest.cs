using System;
using Xunit;
using WebApp.Controllers;
using WebApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Tests
{
    public class AdminControllerTest
    {
        /*private AdminController createController(int database) {
            return new AdminController(new MyContext(new DbContextOptionsBuilder().UseInMemoryDatabase("TemporaryDatabase" + database).Options));
        }*/

        [Fact]
        public void Test1()
        {
            //createController(0);
        }
    }
}
