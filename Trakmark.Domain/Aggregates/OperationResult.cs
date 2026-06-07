namespace Trakmark.Domain.Aggregates;

/// <summary>
/// A lightweight result type for domain operations that may succeed or fail
/// with a reason, without throwing exceptions.
/// </summary>
public readonly record struct OperationResult
{
    /// <summary>Whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Human-readable reason for failure, or <see langword="null"/> on success.</summary>
    public string? FailureReason { get; }

    private OperationResult(bool isSuccess, string? failureReason)
    {
        IsSuccess = isSuccess;
        FailureReason = failureReason;
    }

    /// <summary>Creates a successful result.</summary>
    public static OperationResult Success() => new(true, null);

    /// <summary>Creates a failure result with the supplied <paramref name="reason"/>.</summary>
    public static OperationResult Failure(string reason) => new(false, reason);
}
