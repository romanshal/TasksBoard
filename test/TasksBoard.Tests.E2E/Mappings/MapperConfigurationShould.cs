using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace TasksBoard.Tests.E2E.Mappings
{
    public class MapperConfigurationShould(
        TasksBoardApiApllicationFactory factory) : IClassFixture<TasksBoardApiApllicationFactory>
    {
        [Fact]
        public void BeValid()
        {
            var configurationProvider = factory.Services.GetRequiredService<IMapper>().ConfigurationProvider;
            configurationProvider.Invoking(p => p.AssertConfigurationIsValid()).Should().NotThrow();
        }
    }
}
