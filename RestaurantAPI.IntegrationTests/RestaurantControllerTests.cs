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
        private HttpClient _client;

        public RestaurantControllerTests()
        {
            var factory = new WebApplicationFactory<Program>()
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

            _client = factory.CreateClient();
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
                City= "TestCity",
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
