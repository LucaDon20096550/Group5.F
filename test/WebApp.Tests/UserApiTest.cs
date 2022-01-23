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
using System.IO;

namespace WebApp.Tests
{
    public class UserApiTest
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

        private static UserApi CreateController() {
            database++;
            _context = new MyContext( new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("TemporaryDatabase" + database).Options );
            _userManagerMock = MockUserManager();
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "1" });
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "2" });
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "3" });

            _context.SaveChanges();
            return new UserApi(_context, _userManagerMock.Object);
        }

        [Fact]
        public async void GetUserNameTest()
        {
            var userApi = CreateController();
            _context.Users.Add(new ApplicationUser(){Id = "4"});
            // ApplicationUser user = _context.Users.Single(u => u.Id == "4");

            // Console.Write(user + "USERRRRRRRRRRRRR");
            // Console.Write(user.ToString());

            _context.Chats.Add(new PrivateChat() {Id = 1, Name = "chat1"});
            PrivateChat privateChat = (PrivateChat) await _context.Chats.Where(c => c.Id == 1).SingleOrDefaultAsync();

            // privateChat.Users.Add(user);

            // privateChat.Messages.Add(new Message(){
            //     Id = 1, Text = "test text", DateTimeSent = new DateTime(2022, 1, 23), Sender = user, Chat = privateChat });

            // Console.Write( await userApi.GetUserName(user.Id, privateChat.Id) + "---------------------------------");

            // Assert.Equals("");

        }
    }
}
