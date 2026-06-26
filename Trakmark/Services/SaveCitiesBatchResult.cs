namespace Trakmark.Services;

/// <summary>
/// The discriminated result of a <see cref="SaveCitiesBatchService.SaveAsync"/> call.
/// Callers pattern-match on the concrete subtype to determine the outcome.
/// </summary>
public abstract class SaveCitiesBatchResult
{
    private SaveCitiesBatchResult() { }

    /// <summary>All rows were valid, deduplicated, and persisted successfully.</summary>
    public sealed class Success : SaveCitiesBatchResult
    {
        /// <summary>The number of cities persisted in this batch.</summary>
        public int Count { get; }

        /// <summary>Initializes a new <see cref="Success"/> result.</summary>
        /// <param name="count">The number of cities persisted.</param>
        public Success(int count)
        {
            Count = count;
        }
    }

    /// <summary>
    /// At least one row failed domain validation (empty name, name too long, null state).
    /// The entire batch was rejected.
    /// </summary>
    public sealed class ValidationError : SaveCitiesBatchResult
    {
        /// <summary>A human-readable description of the first validation failure.</summary>
        public string Message { get; }

        /// <summary>Initializes a new <see cref="ValidationError"/> result.</summary>
        /// <param name="message">The validation failure message.</param>
        public ValidationError(string message)
        {
            ArgumentNullException.ThrowIfNull(message);
            Message = message;
        }
    }

    /// <summary>
    /// Two or more rows in the submitted batch represent the same city (same name and
    /// state, case-insensitive). The entire batch was rejected.
    /// </summary>
    public sealed class InBatchDuplicate : SaveCitiesBatchResult
    {
        /// <summary>The city name that appeared more than once in the batch.</summary>
        public string CityName { get; }

        /// <summary>The two-letter state abbreviation that appeared more than once in the batch.</summary>
        public string StateAbbreviation { get; }

        /// <summary>Initializes a new <see cref="InBatchDuplicate"/> result.</summary>
        /// <param name="cityName">The duplicate city name.</param>
        /// <param name="stateAbbreviation">The duplicate state abbreviation.</param>
        public InBatchDuplicate(string cityName, string stateAbbreviation)
        {
            ArgumentNullException.ThrowIfNull(cityName);
            ArgumentNullException.ThrowIfNull(stateAbbreviation);
            CityName = cityName;
            StateAbbreviation = stateAbbreviation;
        }
    }

    /// <summary>
    /// One or more rows in the batch match a city already persisted in the database
    /// (same name and state, case-insensitive). The entire batch was rejected.
    /// </summary>
    public sealed class CrossBatchDuplicate : SaveCitiesBatchResult
    {
        /// <summary>The city name that already exists in the database.</summary>
        public string CityName { get; }

        /// <summary>The two-letter state abbreviation of the duplicate city.</summary>
        public string StateAbbreviation { get; }

        /// <summary>Initializes a new <see cref="CrossBatchDuplicate"/> result.</summary>
        /// <param name="cityName">The duplicate city name.</param>
        /// <param name="stateAbbreviation">The duplicate state abbreviation.</param>
        public CrossBatchDuplicate(string cityName, string stateAbbreviation)
        {
            ArgumentNullException.ThrowIfNull(cityName);
            ArgumentNullException.ThrowIfNull(stateAbbreviation);
            CityName = cityName;
            StateAbbreviation = stateAbbreviation;
        }
    }

    /// <summary>
    /// The database rejected the batch insert with a unique-constraint violation after
    /// the pre-persist cross-batch duplicate check passed. This indicates a concurrent
    /// insert from another request landed between the check and the save. The conflicting
    /// city name and state are not identifiable from the exception; the caller should
    /// treat the entire batch as rejected and advise the user to retry.
    /// </summary>
    public sealed class ConcurrentDuplicate : SaveCitiesBatchResult
    {
    }
}
