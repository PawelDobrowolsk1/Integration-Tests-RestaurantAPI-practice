using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantAPI.Entities;
using RestaurantAPI.IntegrationTests.Helpers;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using Xunit;

namespace RestaurantAPI.IntegrationTests
{
    public class AccountControllerTests
    {
        private HttpClient _client;
        private Mock<IAccountService> _accountServiceMock = new Mock<IAccountService>();

        public AccountControllerTests()
        {
            _client = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services
                            .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                        services.Remove(dbContextOptions);

                        services.AddSingleton<IAccountService>(_accountServiceMock.Object);

                        services
                            .AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
                    });
                })
                .CreateClient();
        }

        [Fact]
        public async Task Login_ForRegisteredUser_ReturnsOk()
        {
            // arrange
            _accountServiceMock.Setup(e => e.GenerateJwt(It.IsAny<LoginDto>()))
                .Returns("jwt");

            var loginDto = new LoginDto()
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var httpContent = loginDto.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/account/login", httpContent);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task RegisterUser_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange
            var user = new RegisterUserDto()
            {
                Password = "password123",
                ConfirmPassword = "123",
                DateOfBirth = new DateTime(1990 - 01 - 01),
                Nationality = "Test"
            };

            var httpContent = user.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/account/register", httpContent);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterUser_WithValidModel_ReturnsOk()
        {
            // arrange
            var user = new RegisterUserDto()
            {
                Email = "test@test.com",
                Password = "password123",
                ConfirmPassword = "password123",
                DateOfBirth = new DateTime(1990 - 01 - 01),
                Nationality = "Test"
            };

            var httpContent = user.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/account/register", httpContent);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
