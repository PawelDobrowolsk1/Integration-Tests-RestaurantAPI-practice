using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.IntegrationTests.Helpers;
using RestaurantAPI.Models;
using Xunit;

namespace RestaurantAPI.IntegrationTests
{
    public class AccountControllerTests
    {
        private HttpClient _client;

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

                        services
                            .AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
                    });
                })
                .CreateClient();
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
