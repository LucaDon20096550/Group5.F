using System;
using Xunit;
using WebApp.Controllers;
using WebApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Moq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;

namespace WebApp.Tests
{
    public class UserApiTest
    {
        private static int database = 0;
        private static MyContext _context;
        private static Mock<UserManager<ApplicationUser>> _userManagerMock;

        public static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<ApplicationUser, string>((x, y) => { _context.Users.Add(x); _context.SaveChanges(); });
            mgr.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.Users).Returns(_context.Users);

            return mgr;
        }

        private AdminController createController() {
            database++;
            _context = new MyContext( new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("TemporaryDatabase" + database).Options );
            _userManagerMock = MockUserManager();
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "1" });
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "2" });
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "3" });
            _context.SaveChanges();
            return new AdminController(_context, _userManagerMock.Object);
        }

        [Fact]
        public void GetUserNameTest()
        {
            
        }
    }
}
