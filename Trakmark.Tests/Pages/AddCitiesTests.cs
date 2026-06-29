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

    private readonly CurrentUserContext _userContext;
    private readonly IRegisteredUserLookupService _mockLookup;
    private readonly ISaveCitiesBatchService _mockBatchService;

    /// <summary>Initializes a new instance of <see cref="AddCitiesTests"/>.</summary>
    public AddCitiesTests()
    {
        _userContext = new CurrentUserContext();
        _mockLookup = Substitute.For<IRegisteredUserLookupService>();
        _mockLookup.GetByAccountIdAsync(TestAccountId).Returns(TestUserId);
        _mockBatchService = Substitute.For<ISaveCitiesBatchService>();

        AddAuthorization()
            .SetAuthorized("admin@test.com")
            .SetRoles("Admin")
            .SetClaims(new Claim(ClaimTypes.NameIdentifier, TestAccountId));
        Services.AddLogging();
        Services.AddSingleton<ICurrentUserContext>(_userContext);
        Services.AddSingleton(_mockLookup);
        Services.AddSingleton(_mockBatchService);
        SetRendererInfo(new RendererInfo("Server", isInteractive: true));
    }

    [Fact]
    public void Render_WithAuthenticatedUser_SetsCurrentUserContextUserId()
    {
        // Arrange

        // Act
        Render<AddCities>();

        // Assert
        Assert.Equal(TestUserId, _userContext.UserId);
    }

    [Theory]
    [InlineData("InvalidOperation")]
    [InlineData("HttpRequest")]
    public void Render_WhenLookupThrows_ShowsErrorMessage(string exceptionKey)
    {
        // Arrange
        var exception = exceptionKey switch
        {
            "InvalidOperation" => (Exception)new InvalidOperationException("User not found"),
            "HttpRequest" => new HttpRequestException("Network failure"),
            _ => throw new ArgumentOutOfRangeException(nameof(exceptionKey))
        };
        _mockLookup
            .GetByAccountIdAsync(TestAccountId)
            .Returns(Task.FromException<RegisteredUserId>(exception));

        // Act
        var cut = Render<AddCities>();

        // Assert
        Assert.NotEmpty(cut.FindAll("#error-alert"));
        _mockBatchService.DidNotReceive().SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>());
    }

    [Fact]
    public void InitialRender_OnLoad_ShowsOneRowWithNameInputAndStateDropdown()
    {
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
        // Act
        var cut = Render<AddCities>();

        // Assert
        Assert.True(cut.Find("#save-btn").HasAttribute("disabled"));
    }

    [Fact]
    public async Task SaveButton_NoStateSelected_IsDisabled()
    {
        // Arrange
        var cut = Render<AddCities>();

        // Act
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });

        // Assert
        Assert.True(cut.Find("#save-btn").HasAttribute("disabled"));
    }

    [Fact]
    public async Task SaveButton_UserIdNullAfterLookupFailure_IsDisabledEvenWhenFormValid()
    {
        // Arrange
        _mockLookup
            .GetByAccountIdAsync(TestAccountId)
            .Returns(Task.FromException<RegisteredUserId>(new InvalidOperationException("lookup failed")));

        var cut = Render<AddCities>();

        // Act
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Assert
        Assert.True(cut.Find("#save-btn").HasAttribute("disabled"));
    }

    [Fact]
    public async Task SaveButton_AllRowsHaveNameAndState_IsEnabled()
    {
        // Arrange
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
        _mockBatchService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>())
            .Returns(new SaveCitiesBatchResult.Success(1));

        var cut = Render<AddCities>();
        await cut.Find("input[type='text']")
            .ChangeAsync(new ChangeEventArgs { Value = "Springfield" });
        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "IL" });

        // Act
        await cut.Find("#save-btn").ClickAsync(new MouseEventArgs());

        // Assert
        await _mockBatchService
            .Received(1)
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>());
        Assert.NotEmpty(cut.FindAll("#success-alert"));
        Assert.Single(cut.FindAll("input[type='text']"));
    }

    [Fact]
    public async Task Save_ValidationFailure_ShowsErrorAlert()
    {
        // Arrange
        _mockBatchService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>())
            .Returns(new SaveCitiesBatchResult.ValidationError("City name is invalid."));

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
        _mockBatchService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>())
            .Returns(new SaveCitiesBatchResult.CrossBatchDuplicate("Springfield", "IL"));

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
        _mockBatchService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>())
            .Returns(new SaveCitiesBatchResult.InBatchDuplicate("Springfield", "IL"));

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
        _mockBatchService
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>())
            .Returns(new SaveCitiesBatchResult.ConcurrentDuplicate());

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
        var cut = Render<AddCities>();

        // Act
        await cut.Find("#cancel-btn").ClickAsync(new MouseEventArgs());

        // Assert
        var navManager = (BunitNavigationManager)Services.GetRequiredService<NavigationManager>();
        Assert.NotEmpty(navManager.History);
        Assert.EndsWith("/", navManager.History.Last().Uri);
        await _mockBatchService
            .DidNotReceive()
            .SaveAsync(Arg.Any<IReadOnlyList<SaveCityRow>>());
    }
}
