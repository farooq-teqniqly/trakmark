using Trakmark.Domain.Ids;

namespace Trakmark.Domain.Tests.Ids;

/// <summary>Tests for <see cref="StudentId"/> parse, round-trip, and empty sentinel.</summary>
public sealed class StudentIdTests
{
    [Fact]
    public void ParseStudentId_RejectsWrongPrefix()
    {
        // Arrange / Act / Assert
        Assert.False(StudentId.TryParse("MEET-7F3K9M", out _));
    }

    [Theory]
    [InlineData("STU-7F3K90")]
    [InlineData("STU-7F3K9O")]
    [InlineData("STU-7F3K91")]
    [InlineData("STU-7F3K9I")]
    [InlineData("STU-7F3K9L")]
    public void ParseStudentId_RejectsOutOfCharsetBody(string value)
    {
        // Arrange / Act / Assert
        Assert.False(StudentId.TryParse(value, out _));
    }

    [Theory]
    [InlineData("STU-7F3K9")]
    [InlineData("STU-7F3K9MM")]
    [InlineData("STU-")]
    public void ParseStudentId_RejectsWrongLengthBody(string value)
    {
        // Arrange / Act / Assert
        Assert.False(StudentId.TryParse(value, out _));
    }

    [Fact]
    public void StudentId_RoundTrips()
    {
        // Arrange
        var original = StudentId.NewId();

        // Act
        var roundTripped = StudentId.Parse(original.ToString());

        // Assert
        Assert.Equal(original, roundTripped);
    }

    [Fact]
    public void Parse_ThrowsForIllFormedValue()
    {
        // Arrange / Act / Assert
        Assert.Throws<FormatException>(() => StudentId.Parse("MEET-7F3K9M"));
    }

    [Fact]
    public void StudentId_Empty_IsDistinctFromNewId()
    {
        // Arrange / Act
        var empty = StudentId.Empty;
        var newId = StudentId.NewId();

        // Assert
        Assert.NotEqual(empty, newId);
    }
}
