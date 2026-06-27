using Bunit;
using Bunit.TestDoubles;
using Trakmark.Components.Layout;

namespace Trakmark.Tests.Layout;

/// <summary>bUnit tests for <see cref="TopNavMenu"/>.</summary>
public sealed class TopNavMenuTests : BunitContext
{
    /// <summary>Provides auth setup actions and expected Add Cities link counts for the theory.</summary>
    public static TheoryData<Action<BunitAuthorizationContext>, int> AddCitiesLinkVisibilityData() =>
        new()
        {
            { auth => auth.SetAuthorized("admin@test.com").SetRoles("Admin"), 1 },
            { auth => auth.SetAuthorized("user@test.com"), 0 },
            { _ => { }, 0 },
        };

    [Theory]
    [MemberData(nameof(AddCitiesLinkVisibilityData))]
    public void AddCitiesLink_AuthRole_ControlsVisibility(Action<BunitAuthorizationContext> setupAuth, int expectedCount)
    {
        // Arrange
        var auth = AddAuthorization();
        setupAuth(auth);

        // Act
        var cut = Render<TopNavMenu>();

        // Assert
        Assert.Equal(expectedCount, cut.FindAll("a[href='admin/cities/add']").Count);
    }
}
