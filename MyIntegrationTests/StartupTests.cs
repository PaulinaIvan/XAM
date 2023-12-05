using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;
using XAM;
using XAM.Models;
using MaxMind.GeoIP2;

namespace MyIntegrationTests
{
    public class StartupTests
    {
        [Fact]
        public void ConfigureServices_ShouldAddServices()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            var services = new ServiceCollection();
            var startup = new Startup(configuration.Object);

            // Act
            startup.ConfigureServices(services);

            // Assert
            Assert.Contains(services, service => service.ServiceType == typeof(ErrorViewModel));
            Assert.Contains(services, service => service.ServiceType == typeof(DatabaseReader));
            Assert.Contains(services, service => service.ServiceType == typeof(XamDbContext));
        }
    }
}
