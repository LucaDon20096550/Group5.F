using System;
using Xunit;
using WebApp.Controllers;
using WebApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp.Tests
{
    public class MessageApiTest
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
            mgr.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_context.Users.Single(u => u.Id == "1"));

            return mgr;
        }

        private static MessageApi CreateController() {
            database++;
            _context = new MyContext(new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase("TemporaryDatabase" + database).Options);
            _userManagerMock = MockUserManager();
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "1" });
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "2" });
            _context.Users.Single(u => u.Id == "1")
                .PrivateChats
                .Add(new PrivateChat()
                {
                    Id = 1,
                    Name = "TestChat",
                    Users = _context.Users.ToList(),
                    Messages = new List<Message>()
                    {
                        new Message()
                        {
                            DateTimeSent = DateTime.Now,
                            Sender = _context.Users.Single(u => u.Id == "1"),
                            Text = "Test1"
                        },
                        new Message()
                        {
                            DateTimeSent = DateTime.Now,
                            Sender = _context.Users.Single(u => u.Id == "2"),
                            Text = "Test2"
                        }
                    }
                });
            return new MessageApi(
                _context,
                _userManagerMock.Object);
        }
        
        [Fact]
        public void GetMessagesTest()
        {
            var controller = CreateController();
            var list = Xunit.Assert.IsType<List<Message>>(controller.GetMessages(1));
            Xunit.Assert.Equal(2, list.Count);
        }
        
        [Fact]
        public void PostMessageTest()
        {
            var controller = CreateController();
            var message3 = Xunit.Assert.IsType<Message>(controller.PostMessage(1, "Test3"));
            Xunit.Assert.Equal(1, message3.Chat.Id);
            Xunit.Assert.Equal("Test3", message3.Text);
            Xunit.Assert.Equal(3, _context.Users.Single(u => u.Id == "1").PrivateChats.Single(c => c.Id == 1).Messages.Count);
            
            var message4 = Xunit.Assert.IsType<Message>(controller.PostMessage(1, "Test4"));
            Xunit.Assert.Equal(1, message4.Chat.Id);
            Xunit.Assert.Equal("Test4", message4.Text);
            Xunit.Assert.Equal(4, _context.Users.Single(u => u.Id == "1").PrivateChats.Single(c => c.Id == 1).Messages.Count);
            
            var message5 = Xunit.Assert.IsType<Message>(controller.PostMessage(1, "Test5"));
            Xunit.Assert.Equal(1, message5.Chat.Id);
            Xunit.Assert.Equal("Test5", message5.Text);
            Xunit.Assert.Equal(5, _context.Users.Single(u => u.Id == "1").PrivateChats.Single(c => c.Id == 1).Messages.Count);
        }
    }
}
