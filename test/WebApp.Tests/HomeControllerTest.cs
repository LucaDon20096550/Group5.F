using System;
using Xunit;
using WebApp.Controllers;
using WebApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApp.Tests
{
    public class HomeControllerTest
    {
        private static int database = 0;
        private static MyContext _context;
        private static Mock<UserManager<ApplicationUser>> _userManager;
        private static Mock<SignInManager<ApplicationUser>> _signInManager;

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

        public static Mock<SignInManager<ApplicationUser>> MockSignInManager(UserManager<ApplicationUser> userManager){
            var mgr = new Mock<SignInManager<ApplicationUser>>(
                        userManager,
                        new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                        new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                        new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>().Object,
                        new Mock<Microsoft.Extensions.Logging.ILogger<SignInManager<ApplicationUser>>>().Object,
                        new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>().Object,
                        new Mock<IUserConfirmation<ApplicationUser>>().Object
            );

            _signInManager = mgr;
            return mgr;
        }

        private HomeController createController() {
            database++;
            _context = new MyContext(new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase("TemporaryDatabase" + database).Options);
            var userManager = MockUserManager();
            var signInManager = MockSignInManager(userManager.Object);
            return new HomeController(
                _context,
                userManager.Object,
                signInManager.Object);
        }

        [Fact]
        public void IndexTest()
        {
            var homeController = createController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(homeController.Index());
            Xunit.Assert.Equal(null, viewResult.Model);
        }

        [Fact]
        public async void ChatTest()
        {
            var homeController = createController();

            var iActionResult = Xunit.Assert.IsType<Task<IActionResult>>(homeController.Chat());
            var viewResult = Xunit.Assert.IsType<ViewResult>(await iActionResult);
            Xunit.Assert.Equal(null, viewResult.Model);

            _userManager.Verify(p => p.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()), Times.Once());
        }

        [Fact]
        public void OverOnsTest()
        {
            var homeController = createController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(homeController.OverOns());
            Xunit.Assert.Equal(null, viewResult.Model);
        }

        [Fact]
        public void ZelfhulpgroepenTest()
        {
            var homeController = createController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(homeController.Zelfhulpgroepen(null, null, 0, 0));
            var viewModel = Xunit.Assert.IsType<List<Group>>(viewResult.Model);
            Xunit.Assert.Equal(0, viewModel.Count);
        }

        [Fact]
        public void KlachteninformatieTest()
        {
            var homeController = createController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(homeController.Klachteninformatie());
            Xunit.Assert.Equal(null, viewResult.Model);
        }

        [Fact]
        public void OnsTeamTest()
        {
            var homeController = createController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(homeController.OnsTeam());
            Xunit.Assert.Equal(null, viewResult.Model);
        }

        [Fact]
        public void AanmeldenTest()
        {
            var homeController = createController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(homeController.Aanmelden());
            Xunit.Assert.Equal(null, viewResult.Model);
        }

        [Fact]
        public void AlgemeneVoorwaardenTest()
        {
            var homeController = createController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(homeController.AlgemeneVoorwaarden());
            Xunit.Assert.Equal(null, viewResult.Model);
        }

        [Fact]
        public void PrivacyPolicyTest()
        {
            var homeController = createController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(homeController.PrivacyPolicy());
            Xunit.Assert.Equal(null, viewResult.Model);
        }

        [Fact]
        public void OrthopedagoogProfielTest()
        {
            var homeController = createController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(homeController.OrthopedagoogProfiel());
            Xunit.Assert.Equal(null, viewResult.Model);
        }

        [Fact]
        public void ExtraInformatieTest()
        {
            var homeController = createController();
            var viewResult = Xunit.Assert.IsType<ViewResult>(homeController.ExtraInformatie());
            Xunit.Assert.Equal(null, viewResult.Model);
        }

        [Theory]
        [InlineData(1, "Test1", "IBAN1", 11)]
        [InlineData(2, "Test2", "IBAN2", 22)]
        [InlineData(3, "Test3", "IBAN3", 33)]
        public async void RegisterProfileTest(int id, string fullName, string iban, int bsn)
        {
            DateTime dateOfBirth;
            switch(id)
            {
                case 1:
                    dateOfBirth = new DateTime(1234, 5, 6);
                    break;
                case 2:
                    dateOfBirth = new DateTime(2345, 6, 7);
                    break;
                case 3:
                    dateOfBirth = new DateTime(3456, 7, 8);
                    break;
                default:
                    dateOfBirth = new DateTime(0000, 0, 0);
                    break;
            }

            HttpClient client = new HttpClient();
            var homeController = createController();

            var viewResult = Xunit.Assert.IsType<ViewResult>(await homeController.RegisterProfile(id, fullName, iban, bsn, dateOfBirth));
            //Xunit.Assert.Equal(true, viewResult.Model);

            //var responseGet = await client.GetAsync("https://orthopedagogie-zmdh.herokuapp.com/clienten?sleutel=775610609&clientid=" + id);
            //Xunit.Assert.Equal(null, responseGet);

            if ((bool)viewResult.Model) {
                var viewResult2 = Xunit.Assert.IsType<ViewResult>(await homeController.RegisterProfile(id, fullName, iban, bsn, dateOfBirth));
                Xunit.Assert.Equal(false, viewResult.Model);

                var responseDel = await client.DeleteAsync("https://orthopedagogie-zmdh.herokuapp.com/clienten?sleutel=775610609&clientid=" + id);
                Xunit.Assert.Equal(400, ((int)responseDel.StatusCode));
            }
        }
    }
}
