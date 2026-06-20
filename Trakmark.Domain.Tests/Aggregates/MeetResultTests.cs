using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Catalog;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Aggregates;

/// <summary>
/// Tests for section 6: Meet aggregate and result recording.
/// Covers scenarios from the record-meet-results spec.
/// </summary>
public sealed class MeetResultTests
{
    private static readonly Event TfRunEvent = new(Discipline.Run(100), Sport.TrackAndField);
    private static readonly Event TfJumpEvent = new(
        Discipline.Jump("Long Jump"),
        Sport.TrackAndField
    );
    private static readonly Event TfRelayEvent = new(Discipline.RelayRun(400), Sport.TrackAndField);
    private static readonly Event TfPlaceOnlyEvent = new(
        Discipline.PlaceOnly("Combined Points"),
        Sport.TrackAndField
    );
    private static readonly Event XcRunEvent = new(Discipline.Run(5000), Sport.CrossCountry);

    private static Meet CreateTrackMeet() =>
        Meet.Create(
            new MeetName("Spring Invitational"),
            new MeetDate(new DateOnly(2026, 4, 15)),
            CompetitionLevel.HighSchool,
            Sport.TrackAndField
        );

    // ── Create a meet ─────────────────────────────────────────────────────────

    [Fact]
    public void Meet_Create_AssignsNewIdAndHasNoResults()
    {
        // Arrange / Act
        var meet = CreateTrackMeet();

        // Assert
        Assert.NotEqual(MeetId.Empty, meet.Id);
        Assert.Empty(meet.Results);
    }

    // ── Tier defaults to Open ─────────────────────────────────────────────────

    [Fact]
    public void RecordResult_NoTierSupplied_DefaultsToOpen()
    {
        // Arrange
        var meet = CreateTrackMeet();
        var studentId = StudentId.NewId();
        var mark = new TimeMark(11500);
        var place = new Placement(1);

        // Act
        meet.RecordResult(studentId, TfRunEvent, ResultStatus.Finished, mark, place, tier: null);

        // Assert
        Assert.Equal(Tier.Open, meet.Results.Single().Tier);
    }

    // ── Record a result with a competitive tier ───────────────────────────────

    [Theory]
    [InlineData("Varsity")]
    [InlineData("JV")]
    [InlineData("Open")]
    public void RecordResult_WithExplicitTier_StoresThatTier(string tierName)
    {
        // Arrange
        var meet = CreateTrackMeet();
        var studentId = StudentId.NewId();
        var tier = tierName switch
        {
            "Varsity" => Tier.Varsity,
            "JV" => Tier.JV,
            "Open" => Tier.Open,
            _ => throw new ArgumentOutOfRangeException(nameof(tierName)),
        };
        var mark = new TimeMark(11500);
        var place = new Placement(1);

        // Act
        meet.RecordResult(studentId, TfRunEvent, ResultStatus.Finished, mark, place, tier);

        // Assert
        Assert.Equal(tier, meet.Results.Single().Tier);
    }

    // ── A finished result requires a mark and a place ─────────────────────────

    [Theory]
    [InlineData("no_mark")]
    [InlineData("no_place")]
    public void RecordResult_Finished_MissingRequiredField_Throws(string missingField)
    {
        // Arrange
        var meet = CreateTrackMeet();
        Performance? mark = missingField == "no_mark" ? null : new TimeMark(11500);
        Placement? place = missingField == "no_place" ? null : new Placement(1);

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() =>
            meet.RecordResult(
                StudentId.NewId(),
                TfRunEvent,
                ResultStatus.Finished,
                mark,
                place,
                null
            )
        );
    }

    // ── A non-finished result carries neither mark nor place ──────────────────

    [Theory]
    [InlineData(ResultStatus.DidNotFinish)]
    [InlineData(ResultStatus.Disqualified)]
    [InlineData(ResultStatus.DidNotStart)]
    [InlineData(ResultStatus.NoMark)]
    public void RecordResult_NonFinished_WithNoMarkOrPlace_Succeeds(ResultStatus status)
    {
        // Arrange
        var meet = CreateTrackMeet();

        // Act
        meet.RecordResult(StudentId.NewId(), TfRunEvent, status, mark: null, place: null, null);

        // Assert
        var result = meet.Results.Single();
        Assert.Null(result.Mark);
        Assert.Null(result.Place);
    }

    [Theory]
    [InlineData(ResultStatus.DidNotFinish)]
    [InlineData(ResultStatus.Disqualified)]
    [InlineData(ResultStatus.DidNotStart)]
    [InlineData(ResultStatus.NoMark)]
    public void RecordResult_NonFinished_WithMark_Throws(ResultStatus status)
    {
        // Arrange
        var meet = CreateTrackMeet();

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() =>
            meet.RecordResult(
                StudentId.NewId(),
                TfRunEvent,
                status,
                new TimeMark(11500),
                place: null,
                null
            )
        );
    }

    [Theory]
    [InlineData(ResultStatus.DidNotFinish)]
    [InlineData(ResultStatus.Disqualified)]
    [InlineData(ResultStatus.DidNotStart)]
    [InlineData(ResultStatus.NoMark)]
    public void RecordResult_NonFinished_WithPlace_Throws(ResultStatus status)
    {
        // Arrange
        var meet = CreateTrackMeet();

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() =>
            meet.RecordResult(
                StudentId.NewId(),
                TfRunEvent,
                status,
                mark: null,
                new Placement(1),
                null
            )
        );
    }

    // ── Results preserve entry order ──────────────────────────────────────────

    [Fact]
    public void RecordResult_MultipleResultsForSameStudent_AssignsEntryOrder()
    {
        // Arrange
        var meet = CreateTrackMeet();
        var studentId = StudentId.NewId();

        // Act
        meet.RecordResult(
            studentId,
            TfRunEvent,
            ResultStatus.Finished,
            new TimeMark(11500),
            new Placement(1),
            null
        );
        meet.RecordResult(
            studentId,
            TfJumpEvent,
            ResultStatus.Finished,
            new DistanceMark(450),
            new Placement(2),
            null
        );

        // Assert
        var results = meet
            .Results.Where(r => r.StudentId == studentId)
            .OrderBy(r => r.Order)
            .ToList();
        Assert.Equal(2, results.Count);
        Assert.Equal(1, results[0].Order);
        Assert.Equal(2, results[1].Order);
    }

    [Fact]
    public void RecordResult_OrderIsPerStudent_NotGlobal()
    {
        // Arrange
        var meet = CreateTrackMeet();
        var student1 = StudentId.NewId();
        var student2 = StudentId.NewId();

        // Act
        meet.RecordResult(
            student1,
            TfRunEvent,
            ResultStatus.Finished,
            new TimeMark(11500),
            new Placement(1),
            null
        );
        meet.RecordResult(
            student2,
            TfRunEvent,
            ResultStatus.Finished,
            new TimeMark(12000),
            new Placement(2),
            null
        );
        meet.RecordResult(
            student1,
            TfJumpEvent,
            ResultStatus.Finished,
            new DistanceMark(450),
            new Placement(1),
            null
        );

        // Assert
        var s1Results = meet
            .Results.Where(r => r.StudentId == student1)
            .OrderBy(r => r.Order)
            .ToList();
        var s2Results = meet.Results.Where(r => r.StudentId == student2).ToList();
        Assert.Equal(1, s1Results[0].Order);
        Assert.Equal(2, s1Results[1].Order);
        Assert.Equal(1, s2Results[0].Order);
    }

    // ── Reject a mismatched mark ──────────────────────────────────────────────

    [Fact]
    public void RecordResult_DistanceMarkForTimeEvent_Throws()
    {
        // Arrange
        var meet = CreateTrackMeet();

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() =>
            meet.RecordResult(
                StudentId.NewId(),
                TfRunEvent,
                ResultStatus.Finished,
                new DistanceMark(450),
                new Placement(1),
                null
            )
        );
    }

    [Fact]
    public void RecordResult_TimeMarkForDistanceEvent_Throws()
    {
        // Arrange
        var meet = CreateTrackMeet();

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() =>
            meet.RecordResult(
                StudentId.NewId(),
                TfJumpEvent,
                ResultStatus.Finished,
                new TimeMark(11500),
                new Placement(1),
                null
            )
        );
    }

    // ── A finished result for a distance discipline requires a mark ───────────

    [Fact]
    public void RecordResult_Finished_DistanceDiscipline_NoMark_Throws()
    {
        // Arrange
        var meet = CreateTrackMeet();

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() =>
            meet.RecordResult(
                StudentId.NewId(),
                TfJumpEvent,
                ResultStatus.Finished,
                mark: null,
                new Placement(1),
                null
            )
        );
    }

    // ── A place-only discipline takes a place and no mark ─────────────────────

    [Fact]
    public void RecordResult_PlaceOnly_Finished_WithPlaceAndNoMark_Succeeds()
    {
        // Arrange
        var meet = CreateTrackMeet();

        // Act
        meet.RecordResult(
            StudentId.NewId(),
            TfPlaceOnlyEvent,
            ResultStatus.Finished,
            mark: null,
            new Placement(1),
            null
        );

        // Assert
        var result = meet.Results.Single();
        Assert.Null(result.Mark);
        Assert.NotNull(result.Place);
    }

    [Fact]
    public void RecordResult_PlaceOnly_Finished_WithMark_Throws()
    {
        // Arrange
        var meet = CreateTrackMeet();

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() =>
            meet.RecordResult(
                StudentId.NewId(),
                TfPlaceOnlyEvent,
                ResultStatus.Finished,
                new TimeMark(11500),
                new Placement(1),
                null
            )
        );
    }

    [Fact]
    public void RecordResult_PlaceOnly_Finished_WithNoPlace_Throws()
    {
        // Arrange
        var meet = CreateTrackMeet();

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() =>
            meet.RecordResult(
                StudentId.NewId(),
                TfPlaceOnlyEvent,
                ResultStatus.Finished,
                mark: null,
                place: null,
                null
            )
        );
    }

    // ── Reject a cross-sport event ─────────────────────────────────────────────

    [Fact]
    public void RecordResult_CrossSportEvent_Throws()
    {
        // Arrange
        var meet = CreateTrackMeet();

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() =>
            meet.RecordResult(
                StudentId.NewId(),
                XcRunEvent,
                ResultStatus.Finished,
                new TimeMark(1200000),
                new Placement(1),
                null
            )
        );
    }

    // ── Each relay leg's student gets the team time ────────────────────────────

    [Fact]
    public void RecordResult_Relay_OneResultPerStudentWithSharedTime()
    {
        // Arrange
        var meet = CreateTrackMeet();
        var students = new[]
        {
            StudentId.NewId(),
            StudentId.NewId(),
            StudentId.NewId(),
            StudentId.NewId(),
        };
        var teamTime = new TimeMark(185000);

        // Act
        foreach (var student in students)
        {
            meet.RecordResult(
                student,
                TfRelayEvent,
                ResultStatus.Finished,
                teamTime,
                new Placement(1),
                null
            );
        }

        // Assert
        Assert.Equal(4, meet.Results.Count);
        Assert.All(
            meet.Results,
            r =>
            {
                var timeMark = Assert.IsType<TimeMark>(r.Mark);
                Assert.Equal(185000, timeMark.Milliseconds);
            }
        );
        var studentIds = meet.Results.Select(r => r.StudentId).ToHashSet();
        Assert.Equal(4, studentIds.Count);
    }
}
