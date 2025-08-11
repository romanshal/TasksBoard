using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace TasksBoard.Tests.E2E.Mappings
{
    public class MapperConfigurationShould(
        WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        [Fact]
        public void BeValid()
        {
            var configurationProvider = factory.Services.GetRequiredService<IMapper>().ConfigurationProvider;
            configurationProvider.Invoking(p => p.AssertConfigurationIsValid()).Should().NotThrow();
        }
    }
}
