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
    public class AdminControllerTest
    {
        private static int database = 0;
        private static MyContext _context;
        private static Mock<UserManager<ApplicationUser>> _userManagerMock;

        private static Mock<UserManager<ApplicationUser>> MockUserManager()
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

        private static AdminController CreateController() {
            database++;
            _context = new MyContext( new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("TemporaryDatabase" + database).Options );
            _userManagerMock = MockUserManager();
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "1" });
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "2" });
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "3" });
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
        public void IndexTest()
        {
            var adminController = CreateController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(adminController.Index());
            Xunit.Assert.Null(viewResult.Model);
        }

        [Fact]
        public async void UserIndexTest()
        {
            var adminController = CreateController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(adminController.UserIndex());
            var viewModel = Xunit.Assert.IsType<List<ApplicationUser>>(viewResult.Model);
            Xunit.Assert.Equal(3, viewModel.Count());
        }
        
        [Theory]
        // [InlineData(null, false)]
        [InlineData("0", false)]
        [InlineData("1", true)]
        [InlineData("2", true)]
        [InlineData("3", true)]
        [InlineData("4", false)]
        public async void UserDetailsTest(string id, bool exists)
        {
            var adminController = CreateController();
            var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.UserDetails(id));

            // if (exists) {
            //     var viewResult = Xunit.Assert.IsType<ViewResult>(iActionResult);
            //     Xunit.Assert.Equal(null, viewResult.ViewName);
            //     Xunit.Assert.Equal(new List<ApplicationUser>(), viewResult.Model);
            // } else Xunit.Assert.IsType<NotFoundResult>(iActionResult);

            _userManagerMock.Verify(u => u.FindByIdAsync(It.Is<string>(s => s == id)), Times.Once());
        }

        [Fact]
        public void UserCreateGetTest()
        {
            var adminController = CreateController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(adminController.UserCreate());
            Xunit.Assert.Null(viewResult.Model);
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("2", true)]
        [InlineData("3", true)]
        [InlineData("4", false)]
        [InlineData("5", false)]
        [InlineData("6", false)]
        public async void UserCreatePostTest(string id, bool exists) {
            var adminController = CreateController();
            var user = new ApplicationUser(){ Id = id };
            var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.UserCreate(user));

            // if (exists) {
            //     var viewResult = Xunit.Assert.IsType<ViewResult>(iActionResult);
            //     Xunit.Assert.Equal(null, viewResult.ViewName);
            //     Xunit.Assert.Equal(user, viewResult.Model);
            // } else {
                var redirectToActionResult = Xunit.Assert.IsType<RedirectToActionResult>(iActionResult);
                Xunit.Assert.Equal("UserIndex", redirectToActionResult.ActionName);
                _userManagerMock.Verify(u => u.CreateAsync(It.Is<ApplicationUser>(us => us == user)), Times.Once());
            // }
        }

        [Theory]
        // [InlineData(null, false)]
        [InlineData("0", false)]
        [InlineData("1", true)]
        [InlineData("2", true)]
        [InlineData("3", true)]
        [InlineData("4", false)]
        public async void UserEditGetTest(string id, bool exists)
        {
            var adminController = CreateController();
            var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.UserEdit(id));

            // if (exists) {
            //     var viewResult = Xunit.Assert.IsType<ViewResult>(iActionResult);
            //     Xunit.Assert.Equal(null, viewResult.ViewName);
            //     Xunit.Assert.Equal(new List<ApplicationUser>(), viewResult.Model);
            // } else Xunit.Assert.IsType<NotFoundResult>(iActionResult);

            _userManagerMock.Verify(u => u.FindByIdAsync(It.Is<string>(s => s == id)), Times.Once());
        }

        [Theory]
        // [InlineData(null, null, false)]
        [InlineData("1", "1", true)]
        [InlineData("2", "3", false)]
        [InlineData("3", "3", true)]
        // [InlineData("4", "4", false)]
        public async void UserEditPostTest(string id, string UserId, bool expected)
        {
            var user = new ApplicationUser(){ Id = UserId };
            var adminController = CreateController();
            var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.UserEdit(id, user));

            if (expected) {
                var redirectToActionResult = Xunit.Assert.IsType<RedirectToActionResult>(iActionResult);
                Xunit.Assert.Equal("UserIndex", redirectToActionResult.ActionName);
                _userManagerMock.Verify(u => u.UpdateAsync(It.Is<ApplicationUser>(us => us == user)), Times.Once());
            } else Xunit.Assert.IsType<NotFoundResult>(iActionResult);
        }

        [Theory]
        // [InlineData(null, false)]
        [InlineData("0", false)]
        [InlineData("1", true)]
        [InlineData("2", true)]
        [InlineData("3", true)]
        [InlineData("4", false)]
        public async void UserDeleteTest(string id, bool exists)
        {
            var adminController = CreateController();
            var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.UserDelete(id));

            // if (exists) {
            //     var viewResult = Xunit.Assert.IsType<ViewResult>(iActionResult);
            //     Xunit.Assert.Equal(null, viewResult.ViewName);
            //     Xunit.Assert.Equal(new List<ApplicationUser>(), viewResult.Model);
            // } else Xunit.Assert.IsType<NotFoundResult>(iActionResult);

            _userManagerMock.Verify(u => u.FindByIdAsync(It.Is<string>(s => s == id)), Times.Once());
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("0", false)]
        [InlineData("1", true)]
        [InlineData("2", true)]
        [InlineData("3", true)]
        [InlineData("4", false)]
        public async void UserDeleteConfirmedTest(string id, bool exists)
        {
            var adminController = CreateController();
            var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.UserDeleteConfirmed(id));
            _userManagerMock.Verify(u => u.FindByIdAsync(It.Is<string>(s => s == id)), Times.Once());

            // if (exists) {
                var redirectToActionResult = Xunit.Assert.IsType<RedirectToActionResult>(iActionResult);
                Xunit.Assert.Equal("UserIndex", redirectToActionResult.ActionName);

                var user = await _userManagerMock.Object.FindByIdAsync(id);

                _userManagerMock.Verify(u => u.DeleteAsync(It.Is<ApplicationUser>(us => us == user)), Times.Once());
            // } else Xunit.Assert.Throws();
        }

        [Fact]
        public async void GroupIndexTest()
        {
            var adminController = CreateController();
            var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.GroupIndex());
            var viewResult = Xunit.Assert.IsType<ViewResult>(iActionResult);
            var viewModel = Xunit.Assert.IsType<List<Group>>(viewResult.Model);
            Xunit.Assert.Equal(3, viewModel.Count());
        }
        
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, false)]
        public async void GroupDetailsTest(int id, bool exists)
        {
            var adminController = CreateController();
            var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.GroupDetails(id));

            if (exists) {
                var viewResult = Xunit.Assert.IsType<ViewResult>(iActionResult);
                Xunit.Assert.Null(viewResult.ViewName);
                Xunit.Assert.Equal(await _context.Groups.FindAsync(id), viewResult.Model);
            } else Xunit.Assert.IsType<NotFoundResult>(iActionResult);
        }

        [Fact]
        public void GroupCreateGetTest()
        {
            var adminController = CreateController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(adminController.GroupCreate());
            Xunit.Assert.Null(viewResult.Model);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, false)]
        public async void GroupCreatePostTest(int id, bool exists) {
            var adminController = CreateController();
            var group = new Group(){ Id = id,
                                     Name = "TestGroup",
                                     GroupChat = new GroupChat(){
                                         Description = "Test Description."
                                     }, Users = new List<ApplicationUser>(),
                                     CreatedByName = "TestUser",
                                     CreatedOn = DateTime.Now};

            if (exists) {
                var iActionResult = Xunit.Assert.IsType<Task<IActionResult>>(adminController.GroupCreate(group));
                // Xunit.Assert.Throws<System.InvalidOperationException>(async () => await iActionResult);
            } else {
                var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.GroupCreate(group));
                var redirectToActionResult = Xunit.Assert.IsType<RedirectToActionResult>(iActionResult);
                Xunit.Assert.Equal("GroupIndex", redirectToActionResult.ActionName);
            }
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, false)]
        public async void GroupEditGetTest(int id, bool exists)
        {
            var group = _context.Groups.FindAsync(id);
            var adminController = CreateController();
            var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.GroupEdit(id));

            if (exists) {
                var viewResult = Xunit.Assert.IsType<ViewResult>(iActionResult);
                Xunit.Assert.Null(viewResult.ViewName);
                Xunit.Assert.IsType<Group>(viewResult.Model);
            } else Xunit.Assert.IsType<NotFoundResult>(iActionResult);
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(1, 1, true)]
        [InlineData(2, 3, false)]
        [InlineData(3, 3, true)]
        [InlineData(4, 4, false)]
        public async void GroupEditPostTest(int id, int groupId, bool expected)
        { 
            var adminController = CreateController();

            if (expected) {
                var group = _context.Groups.Find(id);
                group.Name = "TestGroup";
                var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.GroupEdit(id, group));
                var redirectToActionResult = Xunit.Assert.IsType<RedirectToActionResult>(iActionResult);
                Xunit.Assert.Equal("GroupIndex", redirectToActionResult.ActionName);
            } else {
                var group = new Group(){ Id = id,
                                     Name = "TestGroup",
                                     GroupChat = new GroupChat(){
                                         Description = "Test Description."
                                     }, Users = new List<ApplicationUser>(),
                                     CreatedByName = "TestUser",
                                     CreatedOn = DateTime.Now};
                var iActionResult = Xunit.Assert.IsType<Task<IActionResult>>(adminController.GroupCreate(group));
                // Xunit.Assert.Throws<System.InvalidOperationException>(async () => await iActionResult);
            }
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, false)]
        public async void GroupDeleteTest(int id, bool exists)
        {
            var group = _context.Groups.FindAsync(id);
            var adminController = CreateController();
            var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.GroupDelete(id));

            if (exists) {
                var viewResult = Xunit.Assert.IsType<ViewResult>(iActionResult);
                Xunit.Assert.Null(viewResult.ViewName);
                Xunit.Assert.IsType<Group>(viewResult.Model);
            } else Xunit.Assert.IsType<NotFoundResult>(iActionResult);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, false)]
        public async void GroupDeleteConfirmedTest(int id, bool exists)
        {
            var adminController = CreateController();

            if (exists) {
                var iActionResult = await Xunit.Assert.IsType<Task<IActionResult>>(adminController.GroupDeleteConfirmed(id));
                var redirectToActionResult = Xunit.Assert.IsType<RedirectToActionResult>(iActionResult);
                Xunit.Assert.Equal("GroupIndex", redirectToActionResult.ActionName);
            }// else Xunit.Assert.Throws();
        }
    }
}
