using System.Security.Claims;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Trakmark.Components.Pages;
using Trakmark.Domain.Ids;
using Trakmark.Services;

namespace Trakmark.Tests.Pages;

/// <summary>bUnit tests for the <see cref="AddCities"/> Blazor component.</summary>
public sealed class AddCitiesTests : BunitContext
{
    private static readonly RegisteredUserId TestUserId = RegisteredUserId.Empty;

    private void SetupAdminAuth()
    {
        AddAuthorization()
            .SetAuthorized("admin@test.com")
            .SetRoles("Admin")
            .SetClaims(new Claim(ClaimTypes.NameIdentifier, TestUserId.Value));
    }

    // -----------------------------------------------------------------------
    // Task 6.3 — form shape and validation
    // -----------------------------------------------------------------------

    [Fact]
    public void Initial_render_shows_one_row_with_name_input_and_state_dropdown()
    {
        // Arrange
        SetupAdminAuth();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());

        // Act
        var cut = Render<AddCities>();

        // Assert
        Assert.Single(cut.FindAll("input[type='text']"));
        Assert.Single(cut.FindAll("select"));
    }

    [Fact]
    public async Task Add_row_button_increases_row_count()
    {
        // Arrange
        SetupAdminAuth();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());
        var cut = Render<AddCities>();

        // Act
        await cut.Find("#add-row-btn").ClickAsync(new MouseEventArgs());

        // Assert
        Assert.Equal(2, cut.FindAll("input[type='text']").Count);
    }

    [Fact]
    public async Task Add_row_button_is_disabled_when_100_rows_exist()
    {
        // Arrange
        SetupAdminAuth();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());
        var cut = Render<AddCities>();

        // Act
        for (var i = 0; i < 99; i++)
        {
            await cut.Find("#add-row-btn").ClickAsync(new MouseEventArgs());
        }

        // Assert
        Assert.Equal(100, cut.FindAll("input[type='text']").Count);
        Assert.True(cut.Find("#add-row-btn").HasAttribute("disabled"));
    }

    [Fact]
    public void Save_button_is_disabled_when_name_is_empty()
    {
        // Arrange
        SetupAdminAuth();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());

        // Act
        var cut = Render<AddCities>();

        // Assert
        Assert.True(cut.Find("#save-btn").HasAttribute("disabled"));
    }

    [Fact]
    public async Task Save_button_is_disabled_when_no_state_selected()
    {
        // Arrange
        SetupAdminAuth();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());
        var cut = Render<AddCities>();

        // Act — enter a name but leave state unselected
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });

        // Assert
        Assert.True(cut.Find("#save-btn").HasAttribute("disabled"));
    }

    [Fact]
    public async Task Save_button_is_enabled_when_all_rows_have_name_and_state()
    {
        // Arrange
        SetupAdminAuth();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());
        var cut = Render<AddCities>();

        // Act
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Assert
        Assert.False(cut.Find("#save-btn").HasAttribute("disabled"));
    }

    [Theory]
    [InlineData("", "Name is required.")]
    [InlineData("   ", "Name is required.")]
    public async Task Validation_message_appears_when_name_is_empty_or_whitespace(
        string name,
        string expectedMessage
    )
    {
        // Arrange
        SetupAdminAuth();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());
        var cut = Render<AddCities>();

        // Act — type something then clear to trigger validation
        await cut.Find("input[type='text']").ChangeAsync(new ChangeEventArgs { Value = "x" });
        await cut.Find("input[type='text']").ChangeAsync(new ChangeEventArgs { Value = name });

        // Assert
        Assert.Contains(expectedMessage, cut.Markup);
    }

    [Fact]
    public async Task Validation_message_appears_when_name_exceeds_100_characters()
    {
        // Arrange
        SetupAdminAuth();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());
        var cut = Render<AddCities>();
        var longName = new string('A', 101);

        // Act
        await cut.Find("input[type='text']").ChangeAsync(new ChangeEventArgs { Value = longName });

        // Assert
        Assert.Contains("100 characters", cut.Markup);
    }

    [Fact]
    public async Task Validation_message_appears_when_no_state_selected()
    {
        // Arrange
        SetupAdminAuth();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());
        var cut = Render<AddCities>();

        // Act — type a name to trigger showing state error
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });

        // Assert
        Assert.Contains("State is required.", cut.Markup);
    }

    // -----------------------------------------------------------------------
    // Task 6.5 — Save wiring
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Save_calls_service_and_shows_success_toast_then_clears_form()
    {
        // Arrange
        SetupAdminAuth();
        var mockService = Substitute.For<ISaveCitiesBatchService>();
        mockService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>(), Arg.Any<RegisteredUserId>())
            .Returns(new SaveCitiesBatchResult.Success(1));
        Services.AddSingleton(mockService);

        var cut = Render<AddCities>();
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Act
        await cut.Find("#save-btn").ClickAsync(new MouseEventArgs());

        // Assert — service called
        await mockService
            .Received(1)
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>(), Arg.Any<RegisteredUserId>());

        // Assert — success alert shown
        Assert.NotEmpty(cut.FindAll("#success-alert"));

        // Assert — form cleared (back to 1 empty row)
        Assert.Single(cut.FindAll("input[type='text']"));
    }

    [Fact]
    public async Task Save_shows_error_alert_on_validation_failure()
    {
        // Arrange
        SetupAdminAuth();
        var mockService = Substitute.For<ISaveCitiesBatchService>();
        mockService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>(), Arg.Any<RegisteredUserId>())
            .Returns(new SaveCitiesBatchResult.ValidationError("City name is invalid."));
        Services.AddSingleton(mockService);

        var cut = Render<AddCities>();
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Act
        await cut.Find("#save-btn").ClickAsync(new MouseEventArgs());

        // Assert — error alert shown, no success alert
        Assert.NotEmpty(cut.FindAll("#error-alert"));
        Assert.Empty(cut.FindAll("#success-alert"));
        Assert.Contains("City name is invalid.", cut.Markup);
    }

    [Fact]
    public async Task Save_shows_error_alert_on_duplicate_failure()
    {
        // Arrange
        SetupAdminAuth();
        var mockService = Substitute.For<ISaveCitiesBatchService>();
        mockService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>(), Arg.Any<RegisteredUserId>())
            .Returns(new SaveCitiesBatchResult.CrossBatchDuplicate("Springfield", "IL"));
        Services.AddSingleton(mockService);

        var cut = Render<AddCities>();
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Act
        await cut.Find("#save-btn").ClickAsync(new MouseEventArgs());

        // Assert — error alert shown
        Assert.NotEmpty(cut.FindAll("#error-alert"));
        Assert.Empty(cut.FindAll("#success-alert"));
        Assert.Contains("Springfield", cut.Markup);
    }

    [Fact]
    public async Task Save_shows_error_alert_on_in_batch_duplicate()
    {
        // Arrange
        SetupAdminAuth();
        var mockService = Substitute.For<ISaveCitiesBatchService>();
        mockService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>(), Arg.Any<RegisteredUserId>())
            .Returns(new SaveCitiesBatchResult.InBatchDuplicate("Springfield", "IL"));
        Services.AddSingleton(mockService);

        var cut = Render<AddCities>();
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Act
        await cut.Find("#save-btn").ClickAsync(new MouseEventArgs());

        // Assert — error alert shown, no success alert
        Assert.NotEmpty(cut.FindAll("#error-alert"));
        Assert.Empty(cut.FindAll("#success-alert"));
        Assert.Contains("Springfield", cut.Markup);
    }

    [Fact]
    public async Task Save_shows_error_alert_on_concurrent_duplicate()
    {
        // Arrange
        SetupAdminAuth();
        var mockService = Substitute.For<ISaveCitiesBatchService>();
        mockService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>(), Arg.Any<RegisteredUserId>())
            .Returns(new SaveCitiesBatchResult.ConcurrentDuplicate());
        Services.AddSingleton(mockService);

        var cut = Render<AddCities>();
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Act
        await cut.Find("#save-btn").ClickAsync(new MouseEventArgs());

        // Assert — error alert shown, no success alert
        Assert.NotEmpty(cut.FindAll("#error-alert"));
        Assert.Empty(cut.FindAll("#success-alert"));
        Assert.Contains("duplicate was detected during save", cut.Markup);
    }

    // -----------------------------------------------------------------------
    // Task 6.7 — Cancel
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Cancel_navigates_to_home_without_calling_save_service()
    {
        // Arrange
        SetupAdminAuth();
        var mockService = Substitute.For<ISaveCitiesBatchService>();
        Services.AddSingleton(mockService);

        var cut = Render<AddCities>();

        // Act
        await cut.Find("#cancel-btn").ClickAsync(new MouseEventArgs());

        // Assert — navigated to Home
        var navManager = (BunitNavigationManager)Services.GetRequiredService<NavigationManager>();
        Assert.NotEmpty(navManager.History);
        Assert.EndsWith("/", navManager.History.Last().Uri);

        // Assert — service NOT called
        await mockService
            .DidNotReceive()
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>(), Arg.Any<RegisteredUserId>());
    }
}
