using System;
using Xunit;
using WebApp.Controllers;
using WebApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;

namespace WebApp.Tests
{
    public class HomeControllerTest
    {
        private static int database = 0;

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

        public static Mock<SignInManager<ApplicationUser>> MockSignInManager(UserManager<ApplicationUser> userManager){
            return new Mock<SignInManager<ApplicationUser>>(
                        userManager,
                        new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                        new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                        new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>().Object,
                        new Mock<Microsoft.Extensions.Logging.ILogger<SignInManager<ApplicationUser>>>().Object,
                        new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>().Object,
                        new Mock<IUserConfirmation<ApplicationUser>>().Object
            );
        }

        private HomeController createController() {
            database++;
            var userManager = MockUserManager(new List<ApplicationUser>());
            var signInManager = MockSignInManager(userManager.Object);
            return new HomeController(
                new MyContext( new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("TemporaryDatabase" + database).Options ),
                userManager.Object,
                signInManager.Object);
        }

        [Fact]
        public void IndexTest()
        {
            var homeController = createController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(homeController.Index());
            Xunit.Assert.Equal(null, viewResult.ContentType);
            Xunit.Assert.Equal(null, viewResult.Model);
        }

        [Fact]
        public void ChatTest()
        {
            var homeController = createController();
        }
    }
}
