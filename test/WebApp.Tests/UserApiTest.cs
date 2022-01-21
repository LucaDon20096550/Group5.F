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

        public static Mock<UserManager<ApplicationUser>> MockUserManager(List<ApplicationUser> ls)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<ApplicationUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        private AdminController createController() {
            database++;
            _context = new MyContext( new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("TemporaryDatabase" + database).Options );
            _userManagerMock = MockUserManager( new List<ApplicationUser>(){
                                                new ApplicationUser(){Id = "1"},
                                                new ApplicationUser(){Id = "1"},
                                                new ApplicationUser(){Id = "1"}
                                            });
            _context.Groups.Add(new Group(){
                                            Id = 1,
                                            Name = "TestGroup1",
                                            GroupChat = new GroupChat(){
                                                Description = "Test Description 1."
                                            }, Users = new List<ApplicationUser>(),
                                            CreatedByName = "TestUser1",
                                            CreatedOn = DateTime.Now
                                           });
            _context.Groups.Add(new Group(){
                                            Id = 2,
                                            Name = "TestGroup2",
                                            GroupChat = new GroupChat(){
                                                Description = "Test Description 2."
                                            }, Users = new List<ApplicationUser>(),
                                            CreatedByName = "TestUser2",
                                            CreatedOn = DateTime.Now
                                           });
            _context.Groups.Add(new Group(){
                                            Id = 3,
                                            Name = "TestGroup3",
                                            GroupChat = new GroupChat(){
                                                Description = "Test Description 3."
                                            }, Users = new List<ApplicationUser>(),
                                            CreatedByName = "TestUser3",
                                            CreatedOn = DateTime.Now
                                           });
            _context.SaveChanges();
            return new AdminController(_context, _userManagerMock.Object);
        }

        [Fact]
        public void GetUserNameTest()
        {
            
        }
    }
}
