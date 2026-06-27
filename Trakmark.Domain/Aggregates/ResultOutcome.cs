using Trakmark.Domain.Catalog;

namespace Trakmark.Domain.Aggregates;

/// <summary>
/// Groups the three outcome fields of a <see cref="Result"/> whose invariants are
/// enforced together: the <see cref="Status"/> governs whether <see cref="Mark"/>
/// and <see cref="Place"/> are present.
/// </summary>
internal sealed record ResultOutcome(ResultStatus Status, Performance? Mark, Placement? Place);
