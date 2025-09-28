using CoreProtect.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoreProtect.Tests;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddInfrastructure_ShouldRegisterServicesWithoutTypeLoadExceptions()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var exception = Record.Exception(() => services.AddInfrastructure(configuration));

        Assert.Null(exception);
    }
}
