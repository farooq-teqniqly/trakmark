using Bunit;
using Bunit.TestDoubles;
using Trakmark.Components.Layout;

namespace Trakmark.Tests.Layout;

/// <summary>bUnit tests for <see cref="TopNavMenu"/>.</summary>
public sealed class TopNavMenuTests : BunitContext
{
    [Fact]
    public void AddCities_link_renders_for_Admin_role()
    {
        // Arrange
        AddAuthorization()
            .SetAuthorized("admin@test.com")
            .SetRoles("Admin");

        // Act
        var cut = Render<TopNavMenu>();

        // Assert
        Assert.NotEmpty(cut.FindAll("a[href='admin/cities/add']"));
    }

    [Fact]
    public void AddCities_link_does_not_render_for_authenticated_non_Admin_user()
    {
        // Arrange
        AddAuthorization()
            .SetAuthorized("user@test.com");

        // Act
        var cut = Render<TopNavMenu>();

        // Assert
        Assert.Empty(cut.FindAll("a[href='admin/cities/add']"));
    }

    [Fact]
    public void AddCities_link_does_not_render_when_unauthenticated()
    {
        // Arrange
        AddAuthorization();

        // Act
        var cut = Render<TopNavMenu>();

        // Assert
        Assert.Empty(cut.FindAll("a[href='admin/cities/add']"));
    }
}
