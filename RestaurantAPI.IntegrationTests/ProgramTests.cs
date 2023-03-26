using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantAPI.IntegrationTests
{
    public class ProgramTests
    {
        private readonly List<Type> _controllerTypes;
        private readonly WebApplicationFactory<Program> _factory;

        public ProgramTests()
        {
            _controllerTypes = typeof(Program)
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ControllerBase)))
                .ToList();

            _factory  = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        _controllerTypes.ForEach(c => services.AddScoped(c));
                    });
                });
        }
        [Fact]
        public void ConfigureServices_ForControllers_RegistersAllDependencies()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            // assert
            _controllerTypes.ForEach(t =>
            {
                var controller = scope.ServiceProvider.GetService(t);
                controller.Should().NotBeNull();
            });

        }
    }
}
