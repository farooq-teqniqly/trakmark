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
    private static readonly string TestAccountId = Guid.NewGuid().ToString();
    private static readonly RegisteredUserId TestUserId = RegisteredUserId.NewId();

    private void SetupAdminAuth()
    {
        AddAuthorization()
            .SetAuthorized("admin@test.com")
            .SetRoles("Admin")
            .SetClaims(new Claim(ClaimTypes.NameIdentifier, TestAccountId));
    }

    private void SetupLookupService()
    {
        var mockLookup = Substitute.For<IRegisteredUserLookupService>();
        mockLookup.GetByAccountIdAsync(TestAccountId).Returns(TestUserId);
        Services.AddSingleton(mockLookup);
    }

    private CurrentUserContext SetupUserContext()
    {
        var userContext = new CurrentUserContext();
        Services.AddSingleton<ICurrentUserContext>(userContext);
        return userContext;
    }

    /// <summary>Initializes a new instance of <see cref="AddCitiesTests"/>.</summary>
    public AddCitiesTests()
    {
        SetupAdminAuth();
    }

    [Fact]
    public void OnInitializedAsync_WithAuthenticatedUser_SetsCurrentUserContextUserId()
    {
        // Arrange
        var userContext = SetupUserContext();
        SetupLookupService();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());

        // Act
        Render<AddCities>();

        // Assert
        Assert.Equal(TestUserId, userContext.UserId);
    }

    [Fact]
    public void InitialRender_OnLoad_ShowsOneRowWithNameInputAndStateDropdown()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());

        // Act
        var cut = Render<AddCities>();

        // Assert
        Assert.Single(cut.FindAll("input[type='text']"));
        Assert.Single(cut.FindAll("select"));
    }

    [Fact]
    public async Task AddRowButton_Clicked_IncreasesRowCount()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());
        var cut = Render<AddCities>();

        // Act
        await cut.Find("#add-row-btn").ClickAsync(new MouseEventArgs());

        // Assert
        Assert.Equal(2, cut.FindAll("input[type='text']").Count);
    }

    [Fact]
    public async Task AddRowButton_100RowsExist_IsDisabled()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
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
    public void SaveButton_NameIsEmpty_IsDisabled()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());

        // Act
        var cut = Render<AddCities>();

        // Assert
        Assert.True(cut.Find("#save-btn").HasAttribute("disabled"));
    }

    [Fact]
    public async Task SaveButton_NoStateSelected_IsDisabled()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());
        var cut = Render<AddCities>();

        // Act
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });

        // Assert
        Assert.True(cut.Find("#save-btn").HasAttribute("disabled"));
    }

    [Fact]
    public async Task SaveButton_AllRowsHaveNameAndState_IsEnabled()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
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
    public async Task ValidationMessage_NameIsEmptyOrWhitespace_Appears(
        string name,
        string expectedMessage
    )
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());
        var cut = Render<AddCities>();

        // Act
        await cut.Find("input[type='text']").ChangeAsync(new ChangeEventArgs { Value = "x" });
        await cut.Find("input[type='text']").ChangeAsync(new ChangeEventArgs { Value = name });

        // Assert
        Assert.Contains(expectedMessage, cut.Markup);
    }

    [Fact]
    public async Task ValidationMessage_NameExceeds100Characters_Appears()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());
        var cut = Render<AddCities>();
        var longName = new string('A', 101);

        // Act
        await cut.Find("input[type='text']").ChangeAsync(new ChangeEventArgs { Value = longName });

        // Assert
        Assert.Contains("100 characters", cut.Markup);
    }

    [Fact]
    public async Task ValidationMessage_NoStateSelected_Appears()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        Services.AddSingleton(Substitute.For<ISaveCitiesBatchService>());
        var cut = Render<AddCities>();

        // Act
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });

        // Assert
        Assert.Contains("State is required.", cut.Markup);
    }

    [Fact]
    public async Task Save_ValidInput_CallsServiceShowsSuccessToastAndClearsForm()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        var mockService = Substitute.For<ISaveCitiesBatchService>();
        mockService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>())
            .Returns(new SaveCitiesBatchResult.Success(1));
        Services.AddSingleton(mockService);

        var cut = Render<AddCities>();
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Act
        await cut.Find("#save-btn").ClickAsync(new MouseEventArgs());

        // Assert
        await mockService
            .Received(1)
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>());
        Assert.NotEmpty(cut.FindAll("#success-alert"));
        Assert.Single(cut.FindAll("input[type='text']"));
    }

    [Fact]
    public async Task Save_ValidationFailure_ShowsErrorAlert()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        var mockService = Substitute.For<ISaveCitiesBatchService>();
        mockService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>())
            .Returns(new SaveCitiesBatchResult.ValidationError("City name is invalid."));
        Services.AddSingleton(mockService);

        var cut = Render<AddCities>();
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Act
        await cut.Find("#save-btn").ClickAsync(new MouseEventArgs());

        // Assert
        Assert.NotEmpty(cut.FindAll("#error-alert"));
        Assert.Empty(cut.FindAll("#success-alert"));
        Assert.Contains("City name is invalid.", cut.Markup);
    }

    [Fact]
    public async Task Save_CrossBatchDuplicate_ShowsErrorAlert()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        var mockService = Substitute.For<ISaveCitiesBatchService>();
        mockService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>())
            .Returns(new SaveCitiesBatchResult.CrossBatchDuplicate("Springfield", "IL"));
        Services.AddSingleton(mockService);

        var cut = Render<AddCities>();
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Act
        await cut.Find("#save-btn").ClickAsync(new MouseEventArgs());

        // Assert
        Assert.NotEmpty(cut.FindAll("#error-alert"));
        Assert.Empty(cut.FindAll("#success-alert"));
        Assert.Contains("Springfield", cut.Markup);
    }

    [Fact]
    public async Task Save_InBatchDuplicate_ShowsErrorAlert()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        var mockService = Substitute.For<ISaveCitiesBatchService>();
        mockService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>())
            .Returns(new SaveCitiesBatchResult.InBatchDuplicate("Springfield", "IL"));
        Services.AddSingleton(mockService);

        var cut = Render<AddCities>();
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Act
        await cut.Find("#save-btn").ClickAsync(new MouseEventArgs());

        // Assert
        Assert.NotEmpty(cut.FindAll("#error-alert"));
        Assert.Empty(cut.FindAll("#success-alert"));
        Assert.Contains("Springfield", cut.Markup);
    }

    [Fact]
    public async Task Save_ConcurrentDuplicate_ShowsErrorAlert()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        var mockService = Substitute.For<ISaveCitiesBatchService>();
        mockService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>())
            .Returns(new SaveCitiesBatchResult.ConcurrentDuplicate());
        Services.AddSingleton(mockService);

        var cut = Render<AddCities>();
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Act
        await cut.Find("#save-btn").ClickAsync(new MouseEventArgs());

        // Assert
        Assert.NotEmpty(cut.FindAll("#error-alert"));
        Assert.Empty(cut.FindAll("#success-alert"));
        Assert.Contains("duplicate was detected during save", cut.Markup);
    }

    [Fact]
    public async Task Cancel_Clicked_NavigatesToHomeWithoutCallingSave()
    {
        // Arrange
        SetupUserContext();
        SetupLookupService();
        var mockService = Substitute.For<ISaveCitiesBatchService>();
        Services.AddSingleton(mockService);

        var cut = Render<AddCities>();

        // Act
        await cut.Find("#cancel-btn").ClickAsync(new MouseEventArgs());

        // Assert
        var navManager = (BunitNavigationManager)Services.GetRequiredService<NavigationManager>();
        Assert.NotEmpty(navManager.History);
        Assert.EndsWith("/", navManager.History.Last().Uri);
        await mockService
            .DidNotReceive()
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>());
    }
}
