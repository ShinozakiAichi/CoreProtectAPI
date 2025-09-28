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
}
