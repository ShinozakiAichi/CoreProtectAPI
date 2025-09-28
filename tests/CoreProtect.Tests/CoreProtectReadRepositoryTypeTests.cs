using CoreProtect.Application.Abstractions;
using CoreProtect.Infrastructure.Data;
using Xunit;

namespace CoreProtect.Tests;

public sealed class CoreProtectReadRepositoryTypeTests
{
    [Fact]
    public void CoreProtectReadRepository_ShouldImplementReadRepositoryInterface()
    {
        var type = typeof(CoreProtectReadRepository);
        Assert.True(typeof(ICoreProtectReadRepository).IsAssignableFrom(type));
    }

    [Fact]
    public void CoreProtectReadRepository_ShouldExposeArtMapQuery()
    {
        var interfaceMap = typeof(CoreProtectReadRepository).GetInterfaceMap(typeof(ICoreProtectReadRepository));
        Assert.Contains(interfaceMap.InterfaceMethods, method => method.Name == nameof(ICoreProtectReadRepository.GetArtMapAsync));
    }
}
