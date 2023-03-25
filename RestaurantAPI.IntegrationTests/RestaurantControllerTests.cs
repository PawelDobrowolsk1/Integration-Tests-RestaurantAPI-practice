using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;

namespace RestaurantAPI.IntegrationTests 
{
    public class RestaurantControllerTests
    {
        private readonly HttpClient _client;

        public RestaurantControllerTests()
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
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
