using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization.Policy;
using RestaurantAPI.IntegrationTests.Helpers;

namespace RestaurantAPI.IntegrationTests
{
    public class RestaurantControllerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public RestaurantControllerTests()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services
                            .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                        services.Remove(dbContextOptions);

                        services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluatro>();

                        services.AddMvc(options => options.Filters.Add(new FakeUserFilter()));

                        services
                            .AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
                    });
                });

            _client = _factory.CreateClient();
        }

        private void SeedRestaurant(Restaurant restaurant)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<RestaurantDbContext>();

            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task Delete_ForNonRestaurantOwner_ReturnsForbidden()
        {
            // arrange
            var restaurant = new Restaurant()
            {
                CreatedById = 999,
                Name = "Test",
                Description = "DescriptionTest",
                Category = "Test",
                HasDelivery = true,
                ContactEmail = "Test",
                ContactNumber = "Test"
            };

            SeedRestaurant(restaurant);

            // act
            var response = await _client.DeleteAsync("/api/restaurant/" + restaurant.Id);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_ForRestaurantOwner_ReturnsNoContent()
        {
            // arrange
            var restaurant = new Restaurant()
            {
                CreatedById = 1,
                Name = "Test",
                Description = "DescriptionTest",
                Category= "Test",
                HasDelivery = true,
                ContactEmail = "Test",
                ContactNumber = "Test"
            };

            SeedRestaurant(restaurant);

            // act
            var response = await _client.DeleteAsync("/api/restaurant/" + restaurant.Id);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        }

        [Fact]
        public async Task Delete_ForNonExisitingRestaurant_ReturnsNotFound()
        {
            // act 
            var response = await _client.DeleteAsync("/api/restaurant/497");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateRestaurant_WithValidModel_ReturnsCreatedStatus()
        {
            // arrange
            var model = new CreateRestaurantDto()
            {
                Name = "TestRestaurant",
                Description = "Description",
                Category = "Test",
                HasDelivery = true,
                ContactEmail = "TestEmail",
                ContactNumber = "TestNubmer",
                City = "TestCity",
                Street = "TestStreet",
                PostalCode = "TestPostalCode"
            };

            var httpContent = model.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync("/api/restaurant", httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateRestaurant_WithInvalidModel_ReturnsBadRequest()
        {
            //arrange
            var model = new CreateRestaurantDto()
            {
                Description = "DescriptionTest",
                Category = "Test",
                HasDelivery = true,
                PostalCode = "TestPostalCode"
            };


            var httpContent = model.ToJsonHttpContent();
            // act
            var response = await _client.PostAsync("/api/restaurant", httpContent);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }


        [Theory]
        [InlineData("pageSize=5&pageNumber=1")]
        [InlineData("pageSize=10&pageNumber=2")]
        [InlineData("pageSize=15&pageNumber=1")]
        public async Task GetAll_WithQueryParameters_ReturnsOkResult(string queryParams)
        {
            //arrange

            //act

            var response = await _client.GetAsync($"api/restaurant?{queryParams}");
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("pageSize=110&pageNumber=1")]
        [InlineData("pageSize=10&pageNumber=-2")]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetAll_WithInvalidQueryParams_ReturnsBadRequest(string queryParams)
        {
            //arrange

            //act

            var response = await _client.GetAsync($"api/restaurant?{queryParams}");
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
