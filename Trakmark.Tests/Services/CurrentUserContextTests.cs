using Trakmark.Domain.Ids;
using Trakmark.Services;

namespace Trakmark.Tests.Services;

/// <summary>Unit tests for <see cref="CurrentUserContext"/>.</summary>
public sealed class CurrentUserContextTests
{
    [Fact]
    public void UserId_OnConstruction_IsNull()
    {
        // Arrange / Act
        var context = new CurrentUserContext();

        // Assert
        Assert.Null(context.UserId);
    }

    [Fact]
    public void UserId_AfterSet_ReturnsSetValue()
    {
        // Arrange
        var context = new CurrentUserContext();
        var id = RegisteredUserId.NewId();

        // Act
        context.UserId = id;

        // Assert
        Assert.Equal(id, context.UserId);
    }
}
