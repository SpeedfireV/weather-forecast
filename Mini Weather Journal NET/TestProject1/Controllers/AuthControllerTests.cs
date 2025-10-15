using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mini_Weather_Journal;
using Mini_Weather_Journal.Controllers;
using Mini_Weather_Journal.DTO;
using Mini_Weather_Journal.Models;

namespace TestProject1.Controllers;

public class AuthControllerTests
{
    private DatabaseContext _dbContext = null!;
        private AuthController _controller = null!;

        [SetUp]
        public void Setup()
        {
            // 1. Setup in-memory database
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "AuthTestDb")
                .Options;

            _dbContext = new DatabaseContext(options);

            // 2. Seed test user
            var hasher = new PasswordHasher<User>();
            var user = new User
            {
                Email = "login@mail.com",
                Username = "loginuser",
                PasswordHash = hasher.HashPassword(null, "mypassword")
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // 3. Fake configuration with JWT key
            var configData = new Dictionary<string, string>
            {
                { "JwtKey", "supersecretkey123456789" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            // 4. Initialize controller
            _controller = new AuthController(configuration, _dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task Register_ShouldReturnJwtToken()
        {
            var dto = new RegisterDto
            {
                Email = "newuser@mail.com",
                Username = "newuser",
                Password = "newpassword"
            };

            var result = await _controller.Register(dto);

            // Assert we got OkObjectResult
            Assert.That(result, Is.TypeOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            Assert.That(okResult!.Value, Is.Not.Null);

            // Check token exists
            dynamic value = okResult.Value!;
            Assert.That(value.token, Is.Not.Null.And.Not.Empty);

            // Check user added to DB
            var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            Assert.That(userInDb, Is.Not.Null);
        }

        [Test]
        public async Task Login_ShouldReturnJwt_ForValidEmail()
        {
            var dto = new LoginDto
            {
                Login = "login@mail.com",
                Password = "mypassword"
            };

            var result = await _controller.Login(dto);

            Assert.That(result, Is.TypeOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            Assert.That(okResult!.Value, Is.Not.Null);

            dynamic value = okResult.Value!;
            Assert.That(value.token, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Login_ShouldReturnJwt_ForValidUsername()
        {
            var dto = new LoginDto
            {
                Login = "loginuser",
                Password = "mypassword"
            };

            var result = await _controller.Login(dto);

            Assert.That(result, Is.TypeOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            Assert.That(okResult!.Value, Is.Not.Null);

            dynamic value = okResult.Value!;
            Assert.That(value.token, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Login_ShouldReturnUnauthorized_ForInvalidPassword()
        {
            var dto = new LoginDto
            {
                Login = "login@mail.com",
                Password = "wrongpassword"
            };

            var result = await _controller.Login(dto);

            Assert.That(result, Is.TypeOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task Login_ShouldReturnUnauthorized_ForNonExistingUser()
        {
            var dto = new LoginDto
            {
                Login = "nonexist@mail.com",
                Password = "whatever"
            };

            var result = await _controller.Login(dto);

            Assert.That(result, Is.TypeOf<UnauthorizedObjectResult>());
        }
    }